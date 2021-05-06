using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuroSharp.NEAT
{
    public class DefaultReproductionHandler : IReproductionHandler
    {
        /// <summary>
        /// The function used to determine whether a species is allowed, prohibited, or removed from a gene pool in the next generation
        /// </summary>
        public ReferenceFunc<ISpeciesFitness<double>, double, SpeciesReproductionRule> SpeciesSelectionFunction { get; set; } =
        (ref ISpeciesFitness<double> fitness, ref double total) => SpeciesReproductionRule.Allow;

        /// <summary>
        /// Defines how many organisms get truncated to make room for new organisms when a species reproduces
        /// </summary>
        public OrganismTruncationMethod TrucationMethod { get; set; } = OrganismTruncationMethod.Quarter;

        /// <summary>
        /// Defines how this handler selects parents for crossing.
        /// </summary>
        public OrganismReproductionMethod ReproductionMethod { get; set; } = OrganismReproductionMethod.Sequential;

        /// <summary>
        /// The percentage of organisms that should be truncated to make room for more offspring
        /// <code>
        /// Range: 0d - 1d
        /// </code>
        /// </summary>
        public double CustomOrganismTruncationPercentage { get; set; } = 0.2d;

        public ReferenceFunc<int, bool> CustomOrganismTruncater { get; set; } = (ref int NetworkIndex) => Helpers.NextUDouble() > 0.5d;

        /// <summary>
        /// Selects which species should reproduce into the next generation
        /// <para>
        /// Example:
        /// <code>
        /// ReproductionHandler.SpeciesSelectionFunction = 
        /// <code>
        /// //// (ref ISpeciesFitness&lt;double&gt; speciesFitness, ref double GenerationFitness) =&gt;
        /// <code>
        /// //// {
        /// </code>
        /// <code>
        /// //////// if(speciesFitness.Fitness >= 13.0d) return SpeciesReproductionRule.Allow;
        /// </code>
        /// //////// return SpeciesReproductionRule.Prohibit;
        /// <code>
        /// //// }
        /// </code>
        /// Alternatively: (ref ISpeciesFitness&lt;double&gt; speciesFitness, ref double GenerationFitness) =&gt; speciesFitness.Fitness >= 14.0f ? SpeciesReproductionRule.Allow : SpeciesReproductionRule.Prohibit;
        /// </code>
        /// <code>
        /// Alternatively:
        /// </code>
        /// SpeciesReproductionRule MySpeciesSelector()
        /// <code>
        /// {
        /// </code>
        /// //// return SpeciesReproductionRule.ProhibitWithMutation;
        /// <code>
        ///     }
        /// </code>
        /// ReproductionHandler.SpeciesSelectionFunction = MySpeciesSelector;
        /// </code>
        /// </para>
        /// </summary>
        /// <param name="Fitneses"></param>
        /// <param name="TotalGenerationFitness"></param>
        public SpeciesReproductionRule[] SelectUnfitSpecies(ref ISpeciesFitness<double>[] Fitneses, ref double TotalGenerationFitness)
        {
            SpeciesReproductionRule[] rules = new SpeciesReproductionRule[Fitneses.Length];

            Span<SpeciesReproductionRule> speciesRules = new(rules);

            Span<ISpeciesFitness<double>> fitnesses = new(Fitneses);

            for (int i = 0; i < fitnesses.Length; i++)
            {
                speciesRules[i] = SpeciesSelectionFunction(ref fitnesses[i], ref TotalGenerationFitness);
            }

            return rules;
        }

        public Span<int> TruncateSpecies(ref Span<int> Organisms, out Span<int> RemainingOrganisms)
        {
            // truncate the sorted organisms based on the truncation method
            int index = 0;

            switch (TrucationMethod)
            {
                case OrganismTruncationMethod.None:
                    RemainingOrganisms = Organisms;
                    return new(Array.Empty<int>());

                case OrganismTruncationMethod.Single:
                    RemainingOrganisms = Organisms[0..^1];
                    return Organisms[^1..];

                case OrganismTruncationMethod.Quarter:
                    index = (int)(Organisms.Length * 0.75f);
                    break;

                case OrganismTruncationMethod.Third:
                    index = (int)(Organisms.Length * 0.66f);
                    break;

                case OrganismTruncationMethod.Half:
                    index = (int)(Organisms.Length * 0.5f);
                    break;

                case OrganismTruncationMethod.TwoThirds:
                    index = (int)(Organisms.Length * 0.33f);
                    break;

                case OrganismTruncationMethod.ThreeQuarters:
                    index = (int)(Organisms.Length * 0.25f);
                    break;

                case OrganismTruncationMethod.Custom:
                    index = (int)(Organisms.Length * CustomOrganismTruncationPercentage);
                    break;
                case OrganismTruncationMethod.Random:

                    return RandomTruncate(ref Organisms, out RemainingOrganisms);

                case OrganismTruncationMethod.Bool:
                    return BoolTruncate(ref Organisms, CustomOrganismTruncater, out RemainingOrganisms);
            }

            RemainingOrganisms = Organisms[0..^index];

            return Organisms[^index..];
        }

        public (int Left, int Right)[] GenerateBreedingPairs(ref Span<int> EligibleParentOrganisms)
        {
            return ReproductionMethod switch
            {
                OrganismReproductionMethod.Random =>
                    BreedingPairs_Random(ref EligibleParentOrganisms),

                OrganismReproductionMethod.RandomSequential =>
                    BreedingPairs_RandomSequential(ref EligibleParentOrganisms),

                OrganismReproductionMethod.RandomXOR =>
                    BreedingPairs_RandomXOR(ref EligibleParentOrganisms),

                OrganismReproductionMethod.Sequential =>
                    BreedingPairs_Sequential(ref EligibleParentOrganisms),

                OrganismReproductionMethod.XOR =>
                    BreedingPairs_XOR(ref EligibleParentOrganisms),

                _ =>
                    BreedingPairs_Sequential(ref EligibleParentOrganisms),
            };
        }

        internal (int Left, int Right)[] BreedingPairs_Random(ref Span<int> EligibleParentOrganisms)
        {
            // calc the max pairs that can be generated
            int rollsNeeded = EligibleParentOrganisms.Length * 2;

            // create a result array
            var pairs = new (int Left, int Right)[EligibleParentOrganisms.Length];

            // get all the rolls at once to avoid costly locks in the RNG async logic
            int[] rolls = Helpers.NextIntArray(rollsNeeded, 0, EligibleParentOrganisms.Length);

            // carve out contiguous blocks of memory for our working set to iterate faster
            Span<(int Left, int Right)> pairSpan = pairs;
            Span<int> rollSpan = rolls;

            int rollIndex = 0;

            for (int i = 0; i < EligibleParentOrganisms.Length; i++)
            {
                int left = rollSpan[rollIndex];
                int right = rollSpan[rollIndex + 1];

                rollIndex += 2;

                pairSpan[i] = (EligibleParentOrganisms[left], EligibleParentOrganisms[right]);
            }

            return pairs;
        }

        internal (int Left, int Right)[] BreedingPairs_RandomSequential(ref Span<int> EligibleParentOrganisms)
        {
            int rollsNeeded = EligibleParentOrganisms.Length;

            // create a result array
            var pairs = new (int Left, int Right)[rollsNeeded];

            int[] rolls = Helpers.NextIntArray(rollsNeeded, 0, EligibleParentOrganisms.Length);

            // carve out contiguous blocks of memory for our working set to iterate faster
            Span<(int Left, int Right)> pairSpan = pairs;
            Span<int> rollSpan = rolls;

            for (int i = 0; i < EligibleParentOrganisms.Length; i++)
            {
                int right = rollSpan[i];

                // the left parent should be sequential, and the right parent should be random
                pairSpan[i] = (EligibleParentOrganisms[i], EligibleParentOrganisms[right]);
            }

            return pairs;
        }

        internal (int Left, int Right)[] BreedingPairs_RandomXOR(ref Span<int> EligibleParentOrganisms)
        {
            int maxPairs = EligibleParentOrganisms.Length;

            // create a result array
            var pairs = new (int Left, int Right)[maxPairs];

            int[] rolls = Helpers.NextIntArray(maxPairs, 0, EligibleParentOrganisms.Length);

            // carve out contiguous blocks of memory for our working set to iterate faster
            Span<(int Left, int Right)> pairSpan = pairs;
            Span<int> rollSpan = rolls;

            bool random = false;
            for (int i = 0; i < maxPairs; i++)
            {
                int left = random ? EligibleParentOrganisms[i] : EligibleParentOrganisms[^(i + 1)];
                int right = EligibleParentOrganisms[rollSpan[i]];

                pairSpan[i] = (left, right);

                // alternate between XORS
                random = !random;
            }

            return pairs;
        }

        internal (int Left, int Right)[] BreedingPairs_Sequential(ref Span<int> EligibleParentOrganisms)
        {
            int maxPairs = EligibleParentOrganisms.Length;

            // create a result array
            var pairs = new (int Left, int Right)[maxPairs];

            // carve out contiguous blocks of memory for our working set to iterate faster
            Span<(int Left, int Right)> pairSpan = pairs;

            for (int i = 0; i < maxPairs; i++)
            {
                int left = i;
                int right = (i + 1) % maxPairs;

                pairSpan[i] = (EligibleParentOrganisms[left], EligibleParentOrganisms[right]);
            }

            return pairs;
        }

        internal (int Left, int Right)[] BreedingPairs_XOR(ref Span<int> EligibleParentOrganisms)
        {
            int maxPairs = EligibleParentOrganisms.Length;

            // create a result array
            var pairs = new (int Left, int Right)[maxPairs];

            // carve out contiguous blocks of memory for our working set to iterate faster
            Span<(int Left, int Right)> pairSpan = pairs;

            for (int i = 0; i < maxPairs; i++)
            {
                pairSpan[i] = (EligibleParentOrganisms[i], EligibleParentOrganisms[^(i + 1)]);
            }

            return pairs;
        }

        internal Span<int> RandomTruncate(ref Span<int> Organisms, out Span<int> RemainingOrganisms)
        {
            return BoolTruncate(ref Organisms, (ref int x) => Helpers.NextUDouble() >= 0.5d, out RemainingOrganisms);
        }

        internal Span<int> BoolTruncate(ref Span<int> Organisms, ReferenceFunc<int, bool> Truncator, out Span<int> RemainingOrganisms)
        {
            int[] truncated = Array.Empty<int>();
            int[] persistent = Array.Empty<int>();

            for (int i = 0; i < Organisms.Length; i++)
            {
                if (Truncator(ref i))
                {
                    Helpers.Array.AppendValue(ref truncated, ref Organisms[i]);
                }
                else
                {
                    Helpers.Array.AppendValue(ref persistent, ref Organisms[i]);
                }
            }

            RemainingOrganisms = persistent;

            return truncated;
        }
    }
}
