using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NeuroSharp.Extensions;
using System.Reflection;
using System.Security;

namespace NeuroSharp.NEAT
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T">The class of <see cref="INeatNetwork"/> that should be used for the networks created by this controller</typeparam>
    public class SpeciesController<T> where T : class, INeatNetwork, new()
    {
        public int InputNodes { get; private set; } = 1;
        public int OutputNodes { get; private set; } = 1;

        /// <summary>
        /// Max networks in any given generation
        /// </summary>
        public int MaxPopulation { get; set; } = 100;

        /// <summary>
        /// The threashold at which a individual network is considered a different species when compared to another network.
        /// </summary>
        public double CompatibilityThreashold { get; set; } = 3.0f;

        public INeatNetwork[] Generation { get; private set; } = Array.Empty<INeatNetwork>();

        public INeatNetworkComparer NetworkComparer { get; set; } = new DefaultNetworkComparer();

        public IReproductionHandler<OrganismStruct> ReproductionHandler { get; set; } = new DefaultReproductionHandler<OrganismStruct>();

        public IFitnessFunction<double[], double> FitnessFunction { get; set; } = new DefaultFitnessFunction();

        /// <summary>
        /// Holds the classification of each species. Each entry in the array is an array of integers. Where each integer represents the index of a network inside the <see cref="Generation"/> array.
        /// Array scheme:
        /// <code>
        /// {
        ///     "species 1":
        ///     {
        ///         1(the index for the network in this species), 2, 3, 4
        ///     }
        /// }
        /// </code>
        /// </summary>
        public int[][] Species
        {
            get => _Species;
            set => _Species = value;
        }

        internal int[][] _Species = Array.Empty<int[]>();

        public IDictionary<int, int> SpeciesDictionary { get; set; } = new Dictionary<int, int>();

        public SpeciesController(int InputNodes, int OutputNodes)
        {
            this.InputNodes = InputNodes;
            this.OutputNodes = OutputNodes;
            ValidateConstructor();
        }

        /// <summary>
        /// Holds the index of the representatives of each species, where the integer at index <c>i</c> is the index in <see cref="Generation"/> that the network that represents species <c>i</c>. 
        /// <para>
        /// Example: to get the representative for species 6.
        /// <code>
        /// INeatNetwork species6 = Generation [ SpeciesRepresentatives [6] ];
        /// </code>
        /// </para>
        /// </summary>
        public int[] SpeciesRepresentatives
        {
            get => _SpeciesRepresentatives;
            set => _SpeciesRepresentatives = value;
        }
        internal int[] _SpeciesRepresentatives = Array.Empty<int>();

        /// <summary>
        /// Contains the indicies that are available to be populated in the generation array
        /// </summary>
        private Queue<int> AvailableGenerationIndices = new();

        private readonly SemaphoreSlim SpeciesLock = new(1, 1);
        private readonly SemaphoreSlim GenerationLock = new(1, 1);

        /// <summary>
        /// Creates the first sets of populations with default node and connections.
        /// </summary>
        /// <returns></returns>
        public async Task<GenerationCreationResult> CreateInitialPopulation(int InputNodes, int OutputNodes)
        {
            this.InputNodes = InputNodes;
            this.OutputNodes = OutputNodes;

            Generation = new INeatNetwork[MaxPopulation];

            for (int i = 0; i < MaxPopulation; i++)
            {
                Generation[i] = InstantiateNewNetwork(InputNodes, OutputNodes);
            }

            await SpeciateGeneration();

            return GenerationCreationResult.success;
        }

        public async Task SpeciateGeneration()
        {
            // clear the old values
            _SpeciesRepresentatives = Array.Empty<int>();
            _Species = Array.Empty<int[]>();

            // iterate through the generation and speciate them
            for (int i = 1; i < Generation.Length; i++)
            {
                SpeciateOrganism(ref i);

                await Task.Yield();
            }

            // create the species dictionary
            SpeciesDictionary = CreateSpeciesDictionary(ref _Species);

            // this is to remove the warning without pragma becuase i will forget otherwise
            await Task.Yield();
        }

        /// <summary>
        /// Evaluates all species with the provided data, fits the data with the fitness function for all orgnisms, and sums the fitnesses for all species.
        /// </summary>
        /// <param name="DataToEvaluate"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task<OrganismStruct[]> EvaluateGeneration(double[] DataToEvaluate, CancellationToken token)
        {
            int numberOfOrganisms = Generation.Length;

            double[][] evaluationResults = new double[numberOfOrganisms][];

            SemaphoreSlim ThreadLimiter = new(Environment.ProcessorCount, Environment.ProcessorCount);

            SemaphoreSlim ResultLock = new(1, 1);

            void Worker(int index, INeatNetwork network, CancellationToken token)
            {
                ThreadLimiter.Wait(token);
                try
                {
                    double[] results = network.Evaluate(DataToEvaluate);

                    ResultLock.Wait(token);
                    try
                    {
                        evaluationResults[index] = results;
                    }
                    finally
                    {
                        ResultLock.Release();
                    }
                }
                finally
                {
                    // matching lock is in loop starting tasks
                    ThreadLimiter.Release();
                }
            }

            // start up workers that will evaluate every network
            Task[] tasks = new Task[numberOfOrganisms];

            for (int i = 0; i < numberOfOrganisms; i++)
            {
                token.ThrowIfCancellationRequested();

                int index = i;

                //Worker(index, Generation[i], token);

                tasks[i] = Task.Run(() => Worker(index, Generation[index], token), token);
            }

            // wait for all the workers to finish their work
            await Task.WhenAll(tasks);

            // now that we have the results all the networks calculated we should run the fitness function on them
            return FitEvaluationData(ref evaluationResults);
        }

        public async Task<OrganismStruct[]> AdjustFitnesses(OrganismStruct[] EvaluatedOrganisms, CancellationToken token)
        {
            // find the species of each organism and adjust it's fitness compared to the total population of that species

            await Task.Run(() => AdjustOrganisms(ref EvaluatedOrganisms), token);

            return EvaluatedOrganisms;
        }

        public async Task ReproduceAndMutate(OrganismStruct[] Organisms, CancellationToken token)
        {
            await Task.Run(() => ReproduceGeneration(ref Organisms), token);

            await Task.Run(() => MutateGeneration(), token);
        }

        /// <summary>
        /// Adjusts organisms based on their species size using <see cref="FitnessFunction"/>
        /// </summary>
        /// <param name="Organisms"></param>
        private void AdjustOrganisms(ref OrganismStruct[] Organisms)
        {
            // first get the population size of all species
            Span<int> SpeciesSizes = GetSpeciesSizes();

            // get the species of the organism
            Span<OrganismStruct> OrganismSpan = Organisms;

            int species;
            int size;

            for (int i = 0; i < OrganismSpan.Length; i++)
            {
                ref OrganismStruct organism = ref OrganismSpan[i];

                if (SpeciesDictionary.ContainsKey(organism.Id))
                {
                    // get the species
                    species = SpeciesDictionary[organism.Id];

                    size = SpeciesSizes[species];

                    organism.Fitness = FitnessFunction.AdjustOrganismFitness(ref organism.Fitness, ref size);
                }
                else
                {
                    throw new Exception($"Missing Organism {organism.Id} from SpeciesDictionary");
                }
            }
        }

        /// <summary>
        /// Gets the length of each species and puts it into a <see cref="Span{int}"/>
        /// </summary>
        /// <returns></returns>
        private Span<int> GetSpeciesSizes()
        {
            Span<int> SpeciesSizeSpan = new int[Species.Length]; ;

            for (int i = 0; i < SpeciesSizeSpan.Length; i++)
            {
                SpeciesSizeSpan[i] = _Species[i].Length;
            }

            return SpeciesSizeSpan;
        }

        /// <summary>
        /// Creates the dictionary that associates each organism's Id with it's species
        /// </summary>
        /// <param name="SpeciesSource"></param>
        /// <returns></returns>
        private IDictionary<int, int> CreateSpeciesDictionary(ref int[][] SpeciesSource)
        {
            IDictionary<int, int> SpeciesDict = new Dictionary<int, int>();

            Span<int[]> Species = SpeciesSource;

            for (int i = 0; i < Species.Length; i++)
            {
                Span<int> Organisms = Species[i];

                for (int x = 0; x < Organisms.Length; x++)
                {
                    SpeciesDict.TryAdd(Organisms[x], i);
                }
            }

            return SpeciesDict;
        }

        private void ReproduceGeneration(ref OrganismStruct[] Organisms)
        {
            // sort the organisms and truncate, then reproduce to fill in remaining organisms
            Span<OrganismStruct> organismSpan = Organisms;

            // sort by DESC
            organismSpan.Sort((left, right) => right.Fitness.CompareTo(left.Fitness));

            // truncate the poor performing ones
            var truncatedOrganisms = ReproductionHandler.TruncateOrganisms(ref organismSpan, out Span<OrganismStruct> RemainingOrganisms);

            // generate breeding pairs
            var pairs = ReproductionHandler.GenerateBreedingPairs(ref RemainingOrganisms);

            int index;
            int pairsLength = pairs.Length;

            // actually breed the organisms to replace the ones we truncated
            for (int i = 0; i < truncatedOrganisms.Length; i++)
            {
                // becuase there is a chance that we might run out of new pairs by the time we replaced the truncated,
                // we should just wrap
                ref var pair = ref pairs[i % pairsLength];

                // create the new organism
                INeatNetwork newOrganism = CrossNetworks(ref pair.Left, ref pair.Right);

                index = truncatedOrganisms[i].Id;

                // replace the truncated one
                lock (Generation)
                {
                    Generation[index] = newOrganism;
                }
            }
        }

        private void MutateGeneration()
        {
            Span<INeatNetwork> networks = Generation;

            for (int i = 0; i < networks.Length; i++)
            {
                networks[i].Mutate();
            }
        }

        /// <summary>
        /// Iterates the <paramref name="organismsEvaluationResults"/> and fits the data using <see cref="FitnessFunction"/>
        /// </summary>
        /// <param name="organismsEvaluationResults"></param>
        /// <returns></returns>
        internal OrganismStruct[] FitEvaluationData(ref double[][] organismsEvaluationResults)
        {
            int numOfOrganisms = organismsEvaluationResults.Length;

            OrganismStruct[] result = new OrganismStruct[numOfOrganisms];

            Span<double[]> organisms = organismsEvaluationResults;

            for (int i = 0; i < numOfOrganisms; i++)
            {
                result[i] = new OrganismStruct()
                {
                    Id = i,
                    Fitness = FitnessFunction.CalculateFitness(organisms[i])
                };
            }

            return result;
        }

        /// <summary>
        /// Compares the two organisms and sends it to the <see cref="NetworkComparer"/> to derive a genome.
        /// </summary>
        /// <param name="Left"></param>
        /// <param name="Right"></param>
        /// <returns>
        /// the new <see cref="INeatNetwork"/>
        /// </returns>
        private INeatNetwork CrossNetworks(ref OrganismStruct Left, ref OrganismStruct Right)
        {
            // get which network has better fitness
            FitnessState comparedState = Left.Fitness > Right.Fitness ? FitnessState.LeftMoreFit : Left.Fitness == Right.Fitness ? FitnessState.EqualFitness : FitnessState.RightMoreFit;

            ref INeatNetwork leftNetwork = ref Generation[Left.Id];

            ref INeatNetwork rightNetwork = ref Generation[Right.Id];

            // actually derive the new genome
            IInnovation[] crossedGenes = NetworkComparer.DeriveGenome(leftNetwork, rightNetwork, comparedState);

            // create a new network with that genome
            INeatNetwork newNetwork = InstantiateNewNetwork();

            newNetwork.ImportGenome(InputNodes, OutputNodes, crossedGenes);

            newNetwork.Name = $"{leftNetwork.Name}:{rightNetwork.Name}:{(int)comparedState}";

            return newNetwork;
        }

        /// <summary>
        /// Speciates any particular organism within the generation array into a species
        /// </summary>
        /// <param name="NetworkIndex"></param>
        private void SpeciateOrganism(ref int NetworkIndex)
        {
            void AddToSpecies(int index, int value)
            {
                if (Species.Length <= index)
                {
                    Species[index] = new int[1] { value };
                    return;
                }

                Helpers.Array.AppendValue(ref _Species[index], ref value);
            }

            void CreateSpecies(int representativeIndex)
            {
                int[] newSpecies = { representativeIndex };

                Helpers.Array.AppendValue(ref _Species, ref newSpecies);

                Helpers.Array.AppendValue(ref _SpeciesRepresentatives, ref representativeIndex);
            }

            // determine if there is already species generated
            if (SpeciesRepresentatives.Length == 0)
            {
                // set the first species as the first network, which would be just as random as randomly selecting a network from generation
                CreateSpecies(0);
            }

            ref INeatNetwork left = ref Generation[NetworkIndex];

            if (left.TopologyChanged)
            {
                left.GeneratePhenotype();
            }

            // compare each element to each species, if it is above the compatibility threashold then they should be their own species, it is within the threshold then is should be added as a member to that species
            Span<int> representatives = SpeciesRepresentatives;

            for (int species = 0; species < SpeciesRepresentatives.Length; species++)
            {
                ref INeatNetwork right = ref Generation[representatives[species]];

                // make sure the topologies are generated before we try to compare them
                if (right.TopologyChanged)
                {
                    right.GeneratePhenotype();
                }

                double compatibility = NetworkComparer.CalculateCompatibility(left, right);

                if (compatibility > CompatibilityThreashold && species == SpeciesRepresentatives.Length - 1)
                {
                    // if we are at the end of the list and we havent matched with any previous then we should be our own species
                    CreateSpecies(NetworkIndex);
                    return;
                }
                else if (compatibility <= CompatibilityThreashold)
                {
                    // since we are compatible with that species we should add ourselfs to that list
                    AddToSpecies(species, NetworkIndex);
                    return;
                }
            }
        }

        INeatNetwork InstantiateNewNetwork(params object[] Parameters)
        {
            return (INeatNetwork)Activator.CreateInstance(typeof(T), Parameters);
        }

        internal ConstructorInfo ValidateConstructor()
        {
            // we should verify if the type provided meets the requirements for compatibility for this controller
            // it should have a public ctor that matches signature .ctor(int Rows, int Columns, IInovation[] Genome)
            Type t = typeof(T);

            Type[] requiredTypes = new Type[3];

            // Rows
            requiredTypes[0] = typeof(int);
            // Columns
            requiredTypes[1] = typeof(int);
            // Derived Genome
            requiredTypes[2] = typeof(IInnovation[]);

            // intentionally don't catch errors
            ConstructorInfo ctor = t.GetConstructor(BindingFlags.Instance | BindingFlags.Public, null, requiredTypes, null);

            if (ctor != null)
            {
                return ctor;
            }

            throw Networks.Exceptions.InvalidNeatControllerType(t.ToString());
        }
    }
}
