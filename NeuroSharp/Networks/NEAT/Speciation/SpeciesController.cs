﻿using System;
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

            await Helpers.Random.Sleep();

            return GenerationCreationResult.success;
        }

        public async Task SpeciateGeneration()
        {
            // this is to remove the warning without pragma becuase i will forget otherwise
            await Helpers.Random.Sleep();

            // clear the old values
            _SpeciesRepresentatives = Array.Empty<int>();
            _Species = Array.Empty<int[]>();

            // iterate through the generation and speciate them
            for (int i = 1; i < Generation.Length; i++)
            {
                SpeciateOrganism(ref i);
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

            // now that we have reproduced and mutated we should speciate the new organisms
            await SpeciateGeneration();
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

            // carve out memory becuase we will be using these values immediately to construct other spans
            Span<int> organismSpan = Species[Fitness.Species];
            Span<double> fitnessSpan = Fitness.Fitnesses;

            // convert to structs
            Span<OrganismStruct> sortedOrganisms = new OrganismStruct[organismSpan.Length];

            for (int i = 0; i < sortedOrganisms.Length; i++)
            {
                sortedOrganisms[i] = new OrganismStruct(organismSpan[i], fitnessSpan[i]);
            }

            // sort the organisms by fitness
            sortedOrganisms.Sort((x, y) => y.Fitness.CompareTo(x.Fitness));

            // make room for the new organisms that will be produced when we breed the top performers
            // get a list of organisms that should be replaced
            Span<OrganismStruct> truncatedOrganisms = ReproductionHandler.TruncateSpecies(ref sortedOrganisms, out Span<OrganismStruct> remainingOrganisms);

            // convert the remaining organisms to an array so we can replace the species array to avoid having to do weird array resizing and sorting later on
            int[] newSpeciesArray = new int[remainingOrganisms.Length];

            Span<int> newSpeciesSpan = newSpeciesArray;
            for (int i = 0; i < newSpeciesSpan.Length; i++)
            {
                // OrganismStruct is implicitly convertible to int(it's index)
                newSpeciesSpan[i] = remainingOrganisms[i];
            }

            // replace the species array to remove the truncated organisms
            SpeciesLock.Wait();
            try
            {
                Species[Fitness.Species] = newSpeciesArray;
            }
            finally
            {
                SpeciesLock.Release();
            }

            // generate pairs to breed from(this is a span)
            var breedingPairSpan = ReproductionHandler.GenerateBreedingPairs(ref remainingOrganisms);

            // actually create the new organisms
            // replace the generation indices with the new organisms

            // get a reference to the network that is our species representative
            INeatNetwork representative = Generation[SpeciesRepresentatives[Fitness.Species]];

            // keep track of which indicies need to be removed later on
            int[] differentSpeciesIndicies = Array.Empty<int>();

            // only replace the organisms that were truncated
            for (int i = 0; i < truncatedOrganisms.Length; i++)
            {
                // create new organism
                int index = i % breedingPairSpan.Length;

                var newNetwork = CrossNetworks(ref breedingPairSpan[index].Left, ref breedingPairSpan[index].Right);

                int id = truncatedOrganisms[i].Id;

                // replace the networks in the main array
                GenerationLock.Wait();
                try
                {
                    Generation[id] = newNetwork;
                }
                finally
                {
                    GenerationLock.Release();
                }
            }
        }

        private void MutateSpecies(int SpeciesIndex, ref SpeciesReproductionRule rule)
        {
            switch (rule)
            {
                case SpeciesReproductionRule.Allow:
                    break;
                case SpeciesReproductionRule.PreventMutation:
                    return;
                case SpeciesReproductionRule.Prohibit:
                    return;
                case SpeciesReproductionRule.ProhibitWithMutation:
                    break;
                case SpeciesReproductionRule.Random:
                    break;
                case SpeciesReproductionRule.Remove:
                    return;
            }

            Span<int> organisms = _Species[SpeciesIndex];

            for (int i = 0; i < organisms.Length; i++)
            {
                Generation[organisms[i]].Mutate();
            }
        }

        private void RemoveSpecies(int SpeciesIndex)
        {
            // mark the indices in the generation array as being replaceable
            // remove the species from the representative list
            lock (_Species)
            {
                // mark all the species as being available
                Span<int> species = _Species[SpeciesIndex];
                for (int i = 0; i < species.Length; i++)
                {
                    AvailableGenerationIndices.Enqueue(species[i]);
                }

                // remove the species from the species array
                Helpers.Array.RemoveIndex(ref _Species, SpeciesIndex);
            }
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
