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
        public double CompatibilityThreashold { get; set; } = 1.0f;

        public INeatNetwork[] Generation { get; private set; } = Array.Empty<INeatNetwork>();

        public INeatNetworkComparer NetworkComparer { get; set; } = new NeatNetworkComparer();

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

        public int[] SpeciesFitnesses { get; private set; } = Array.Empty<int>();

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

        public async Task<double[][]> EvaluateAsync(double[] Data, CancellationToken token)
        {
            double[][] result = new double[Generation.Length][];

            for (int i = 0; i < Generation.Length; i++)
            {
                await Task.Run(() => Generation[i].Evaluate(Data), token);
            }

            return result;
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
    }
}
