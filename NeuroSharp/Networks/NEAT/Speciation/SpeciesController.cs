using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NeuroSharp.NEAT
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T">The class of <see cref="INeatNetwork"/> that should be used for the networks created by this controller</typeparam>
    public class SpeciesController<T> where T : IInstantiableNetwork<INeatNetwork>, new()
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

        public IReproductionHandler ReproductionHandler { get; set; } = new DefaultReproductionHandler();

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

        internal T InstantiableNetworkObject = new();

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
        /// Creates the first sets of populations with default node and connections.
        /// </summary>
        /// <returns></returns>
        public async Task<GenerationCreationResult> CreateInitialPopulation(int InputNodes, int OutputNodes)
        {
            this.InputNodes = InputNodes;
            this.OutputNodes = OutputNodes;

            Generation = new INeatNetwork[MaxPopulation];

            T tmp = new();

            for (int i = 0; i < MaxPopulation; i++)
            {
                Generation[i] = await tmp.CreateAsync(InputNodes, OutputNodes);
            }

            return GenerationCreationResult.success;
        }

        public async Task SpeciateGeneration()
        {
            // this is to remove the warning without pragma becuase i will forget otherwise
            await Task.Run(() => null);

            void AddToSpecies(int index, int value)
            {
                if (Species.Length <= index)
                {
                    Species[index] = new int[1] { value };
                    return;
                }

                int len = Species[index].Length;

                Array.Resize(ref Species[index], len + 1);

                Species[index][len] = value;
            }

            void CreateSpecies(int representativeIndex)
            {
                Array.Resize(ref _Species, Species.Length + 1);

                AddToSpecies(Species.Length - 1, representativeIndex);

                int len = SpeciesRepresentatives.Length;

                Array.Resize(ref _SpeciesRepresentatives, len + 1);

                SpeciesRepresentatives[len] = representativeIndex;
            }

            // determine if there is already species generated
            if (SpeciesRepresentatives.Length == 0)
            {
                // set the first species as the first network, which would be just as random as randomly selecting a network from generation
                CreateSpecies(0);
            }

            // iterate through the generation and speciate them
            for (int i = 1; i < Generation.Length; i++)
            {
                // compare each element to each species, if it is above the compatibility threashold then they should be their own species, it is within the threshold then is should be added as a member to that species
                for (int species = 0; species < SpeciesRepresentatives.Length; species++)
                {
                    double compatibility = NetworkComparer.CalculateCompatibility(Generation[i], Generation[SpeciesRepresentatives[i]]);

                    if (compatibility > CompatibilityThreashold && species == SpeciesRepresentatives.Length - 1)
                    {
                        // if we are at the end of the list and we havent matched with any previous then we should be our own species
                        CreateSpecies(i);
                    }
                    else
                    {
                        // since we are compatible with that species we should add ourselfs to that list
                        AddToSpecies(species, i);
                    }
                }
            }
        }

        /// <summary>
        /// Evaluates all species with the provided data, fits the data with the fitness function for all orgnisms, and sums the fitnesses for all species.
        /// </summary>
        /// <param name="DataToEvaluate"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task<(ISpeciesFitness<double>[] Fitnesses, double GenerationFitness)> EvaluateGeneration(double[] DataToEvaluate, CancellationToken token)
        {
            int numberOfSpecies = Species.Length;

            ISpeciesFitness<double>[] results = new ISpeciesFitness<double>[numberOfSpecies];

            double TotalFitness = 0;

            ISpeciesFitness<double> CreateResult(int speciesId, double[][] RawResults)
            {
                var (AdjustedFitnesses, TotalAdjustedFitness) = GetSpeciesFitness(RawResults);

                TotalFitness += TotalAdjustedFitness;

                return new SpeciesEvaluationResult<double>
                {
                    Species = speciesId,
                    Fitness = TotalAdjustedFitness,
                    Fitnesses = AdjustedFitnesses
                };
            }

            for (int i = 0; i < numberOfSpecies; i++)
            {
                token.ThrowIfCancellationRequested();
                results[i] = CreateResult(i, await EvaluateSpecies(i, DataToEvaluate, token));
            }

            return (results, TotalFitness);
        }

        public async Task<double[][]> EvaluateSpecies(int SpeciesIndex, double[] DataToEvaluate, CancellationToken token)
        {
            // species array scheme is:
            // index = species index
            // value = array
            //         index = nth organism in species
            //         value = the index of that orgnism in the Generation Array
            // 0 = { 1st Orgism, 2nd Organism, 3rd Organism .. }
            // 1 = { 1st Orgism, 2nd Organism, 3rd Organism .. }
            int n = Species[SpeciesIndex].Length;

            double[][] result = new double[n][];

            SemaphoreSlim Lock = new(1, 1);

            // Create a generic worker that runs an organism and stores it in the result
            void Worker(ref int organism, ref int resultIndex)
            {
                Lock.Wait(token);
                try
                {
                    result[resultIndex] = Generation[organism].Evaluate(DataToEvaluate);
                }
                finally
                {
                    Lock.Release();
                }
            }

            Task[] tasks = new Task[n];

            // start evalulating all of the species asynchronously 
            for (int i = 0; i < n; i++)
            {
                // create variables that will be available to the state machine when we start these tasks
                int organism = Species[SpeciesIndex][i];
                int resultIndex = i;

                tasks[i] = Task.Run(() => Worker(ref organism, ref resultIndex), token);
            }

            // wait for the tasks to finish
            await Task.WhenAll(tasks);

            return result;
        }

        /// <summary>
        /// Begins reproduction of species using <see cref="IReproductionHandler"/> to remove and modify the gene pool. 
        /// </summary>
        /// <param name="Fitnesses"></param>
        /// <param name="GenerationFitness"></param>
        /// <returns></returns>
        public async Task Reproduce(ISpeciesFitness<double>[] Fitnesses, double GenerationFitness)
        {
            // we should first get which species should and should not reproduce
            SpeciesReproductionRule[] rules = ReproductionHandler.SelectUnfitSpecies(ref Fitnesses, ref GenerationFitness);

            await Task.Run(() => ReproduceGeneration(ref Fitnesses, ref rules));
            await Task.Run(() => MutateGeneration(ref Fitnesses, ref rules));
        }

        private void ReproduceGeneration(ref ISpeciesFitness<double>[] Fitnesses, ref SpeciesReproductionRule[] rules)
        {
            // go through the species list and if they are supposed to reproduce kill lower performing children and back-fill with crossed children
            Span<ISpeciesFitness<double>> fitnessSpan = new(Fitnesses);
            Span<SpeciesReproductionRule> ruleSpan = new(rules);
            for (int i = 0; i < fitnessSpan.Length; i++)
            {
                ReproduceSpecies(ref fitnessSpan[i], ref ruleSpan[i]);
            }
        }

        private void MutateGeneration(ref ISpeciesFitness<double>[] Fitnesses, ref SpeciesReproductionRule[] rules)
        {
            // mutate the species
            Span<ISpeciesFitness<double>> fitnessSpan = new(Fitnesses);
            Span<SpeciesReproductionRule> ruleSpan = new(rules);
            for (int i = 0; i < fitnessSpan.Length; i++)
            {
                MutateSpecies(fitnessSpan[i].Species, ref ruleSpan[i]);
            }
        }

        private void ReproduceSpecies(ref ISpeciesFitness<double> Fitness, ref SpeciesReproductionRule rule)
        {
            // check if we should reproduce at all, or remove the species..
            switch (rule)
            {
                case SpeciesReproductionRule.Prohibit:
                    return;
                case SpeciesReproductionRule.ProhibitWithMutation:
                    return;
                case SpeciesReproductionRule.Remove:
                    RemoveSpecies(Fitness.Species);
                    return;
                default:
                    // allow
                    break;
            }

            // get the origanisms in the species
            Span<int> organismSpan = new(Species[Fitness.Species]);
            // get the fitnesses for the origanisms, they should be in order already assuming no body sorted them somehow

            double[] fitnesses = Fitness.Fitnesses;

            // sort the organisms by fitness
            organismSpan.Sort((x, y) => fitnesses[x].CompareTo(fitnesses[y]));

            // make room for the new organisms that will be produced when we breed the top performers
            // get a list of organisms that should be replaced
            Span<int> truncatedOrganisms = ReproductionHandler.TruncateSpecies(ref organismSpan, out Span<int> remainingOrganisms);

            // generate pairs to breed from
            (int Left, int Right)[] BreedingPairs = ReproductionHandler.GenerateBreedingPairs(ref remainingOrganisms);

            // replace the generation indices with the new organisms

            // speciate the generation
            // it remains to be seen whether or not it's more efficient to:
            // replace the indicies and re-build the whole species array after each generation
            // OR
            // replace the indicies within the same species, or if the offspring are different species, remove the indicies and re-arrange the array and resize the array, and replace an index in another species..

            throw new NotImplementedException();
        }

        private void MutateSpecies(int SpeciesIndex, ref SpeciesReproductionRule rule)
        {
            throw new NotImplementedException();
        }

        private void RemoveSpecies(int SpeciesIndex)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Calculates the overall fitness for the species and the individual orginsms fitnesses all at once
        /// <code>
        /// Complexity: O(n)
        /// </code>
        /// </summary>
        /// <param name="SpeciesEvalResults"></param>
        /// <returns></returns>
        public (double[] AdjustedFitnesses, double TotalAdjustedFitness) GetSpeciesFitness(double[][] SpeciesEvalResults)
        {
            double[] resultArray = new double[SpeciesEvalResults.Length];

            Span<double> results = new(resultArray);

            for (int i = 0; i < results.Length; i++)
            {
                results[i] = FitnessFunction.CalculateFitness(SpeciesEvalResults[i]);
            }

            results = FitnessFunction.AdjustSpeciesFitness(ref results, out double TotalFitness);

            return (resultArray, TotalFitness);
        }

        private INeatNetwork CrossNetworks((int NetworkIndex, int Fitness) Left, (int NetworkIndex, int Fitness) Right)
        {
            // get which network has better fitness
            FitnessState comparedState = Left.Fitness > Right.Fitness ? FitnessState.LeftMoreFit : Left.Fitness == Right.Fitness ? FitnessState.EqualFitness : FitnessState.RightMoreFit;

            ref INeatNetwork leftNetwork = ref Generation[Left.NetworkIndex];

            ref INeatNetwork rightNetwork = ref Generation[Right.NetworkIndex];

            // actually derive the new genome
            IInnovation[] crossedGenes = NetworkComparer.DeriveGenome(leftNetwork, rightNetwork, comparedState);

            // create a new network with that genome
            INeatNetwork newNetwork = InstantiableNetworkObject.Create(leftNetwork.InputNodes, leftNetwork.OutputNodes, crossedGenes);

            newNetwork.Name = $"{leftNetwork.Name}:{rightNetwork.Name}:{(int)comparedState}";

            return newNetwork;
        }
    }
}
