using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuroSharp.NEAT
{
    public class DefaultReproductionHandler<T> : IReproductionHandler<T>
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

        public ReferenceFunc<T, bool> CustomOrganismTruncater { get; set; } = (ref T organism) => Helpers.Random.NextUDouble() > 0.5d;

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
        /// <param name="Fitnesses"></param>
        /// <param name="TotalGenerationFitness"></param>
        public SpeciesReproductionRule[] SelectUnfitSpecies(ref ISpeciesFitness<double>[] Fitnesses, ref double TotalGenerationFitness)
        {
            SpeciesReproductionRule[] rules = new SpeciesReproductionRule[Fitnesses.Length];

            Span<SpeciesReproductionRule> speciesRules = new(rules);

            Span<ISpeciesFitness<double>> fitnesses = new(Fitnesses);

            for (int i = 0; i < fitnesses.Length; i++)
            {
                speciesRules[i] = SpeciesSelectionFunction(ref fitnesses[i], ref TotalGenerationFitness);
            }

            return rules;
        }

        public Span<T> TruncateOrganisms(ref Span<T> Organisms, out Span<T> RemainingOrganisms)
        {
            // truncate the sorted organisms based on the truncation method
            int index = 0;

            switch (TrucationMethod)
            {
                case OrganismTruncationMethod.None:
                    RemainingOrganisms = Organisms;
                    return new(Array.Empty<T>());

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

            index = index >= Organisms.Length ? Organisms.Length : index <= 0 ? 0 : index;

            RemainingOrganisms = Organisms[0..^index];

            return Organisms[^index..];
        }

        public Span<(T Left, T Right)> GenerateBreedingPairs(ref Span<T> EligibleParentOrganisms)
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

        internal (T Left, T Right)[] BreedingPairs_Random(ref Span<T> EligibleParentOrganisms)
        {
            // calc the max pairs that can be generated
            int rollsNeeded = EligibleParentOrganisms.Length * 2;

            // create a result array
            var pairs = new (T Left, T Right)[EligibleParentOrganisms.Length];

            // get all the rolls at once to avoid costly locks in the RNG async logic
            int[] rolls = Helpers.Random.NextIntArray(rollsNeeded, 0, EligibleParentOrganisms.Length);

            // carve out contiguous blocks of memory for our working set to iterate faster
            Span<(T Left, T Right)> pairSpan = pairs;
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

        internal (T Left, T Right)[] BreedingPairs_RandomSequential(ref Span<T> EligibleParentOrganisms)
        {
            int rollsNeeded = EligibleParentOrganisms.Length;

            // create a result array
            var pairs = new (T Left, T Right)[rollsNeeded];

            int[] rolls = Helpers.Random.NextIntArray(rollsNeeded, 0, EligibleParentOrganisms.Length);

            // carve out contiguous blocks of memory for our working set to iterate faster
            Span<(T Left, T Right)> pairSpan = pairs;
            Span<int> rollSpan = rolls;

            for (int i = 0; i < EligibleParentOrganisms.Length; i++)
            {
                ref T right = ref EligibleParentOrganisms[rollSpan[i]];

                // the left parent should be sequential, and the right parent should be random
                pairSpan[i] = (EligibleParentOrganisms[i], right);
            }

            return pairs;
        }

        internal (T Left, T Right)[] BreedingPairs_RandomXOR(ref Span<T> EligibleParentOrganisms)
        {
            int maxPairs = EligibleParentOrganisms.Length;

            // create a result array
            var pairs = new (T Left, T Right)[maxPairs];

            int[] rolls = Helpers.Random.NextIntArray(maxPairs, 0, EligibleParentOrganisms.Length);

            // carve out contiguous blocks of memory for our working set to iterate faster
            Span<(T Left, T Right)> pairSpan = pairs;
            Span<int> rollSpan = rolls;

            bool random = true;
            for (int i = 0; i < maxPairs; i++)
            {
                ref T left = ref random ? ref EligibleParentOrganisms[i] : ref EligibleParentOrganisms[^i];
                ref T right = ref EligibleParentOrganisms[rollSpan[i]];

                pairSpan[i] = (left, right);

                // alternate between XORS
                random = !random;
            }

            return pairs;
        }

        internal (T Left, T Right)[] BreedingPairs_Sequential(ref Span<T> EligibleParentOrganisms)
        {
            int maxPairs = EligibleParentOrganisms.Length;

            // create a result array
            var pairs = new (T Left, T Right)[maxPairs];

            // carve out contiguous blocks of memory for our working set to iterate faster
            Span<(T Left, T Right)> pairSpan = pairs;

            for (int i = 0; i < maxPairs; i++)
            {
                int left = i;
                int right = (i + 1) % maxPairs;

                pairSpan[i] = (EligibleParentOrganisms[left], EligibleParentOrganisms[right]);
            }

            return pairs;
        }

        internal (T Left, T Right)[] BreedingPairs_XOR(ref Span<T> EligibleParentOrganisms)
        {
            int maxPairs = EligibleParentOrganisms.Length;

            // create a result array
            var pairs = new (T Left, T Right)[maxPairs];

            // carve out contiguous blocks of memory for our working set to iterate faster
            Span<(T Left, T Right)> pairSpan = pairs;

            for (int i = 0; i < maxPairs; i++)
            {
                pairSpan[i] = (EligibleParentOrganisms[i], EligibleParentOrganisms[^(i + 1)]);
            }

            return pairs;
        }

        internal Span<T> RandomTruncate(ref Span<T> Organisms, out Span<T> RemainingOrganisms)
        {
            return BoolTruncate(ref Organisms, (ref T x) => Helpers.Random.NextUDouble() >= 0.5d, out RemainingOrganisms);
        }

        internal Span<T> BoolTruncate(ref Span<T> Organisms, ReferenceFunc<T, bool> Truncator, out Span<T> RemainingOrganisms)
        {
            T[] truncated = Array.Empty<T>();
            T[] persistent = Array.Empty<T>();

            for (int i = 0; i < Organisms.Length; i++)
            {
                if (Truncator(ref Organisms[i]))
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
