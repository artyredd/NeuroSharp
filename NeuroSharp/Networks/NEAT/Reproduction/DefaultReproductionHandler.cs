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

        public Span<int> TruncateSpecies(ref Span<int> Organisms)
        {
            // truncate the sorted organisms based on the truncation method
            switch (TrucationMethod)
            {
                case OrganismTruncationMethod.None:
                    return new(Array.Empty<int>());
                case OrganismTruncationMethod.Single:
                    return Organisms[^1..];
                case OrganismTruncationMethod.Quarter:
                    return Organisms[^(int)(Organisms.Length * 0.75f)..];
                case OrganismTruncationMethod.Third:
                    return Organisms[^(int)(Organisms.Length * 0.66f)..];
                case OrganismTruncationMethod.Half:
                    return Organisms[^(int)(Organisms.Length * 0.5f)..];
                case OrganismTruncationMethod.TwoThirds:
                    return Organisms[^(int)(Organisms.Length * 0.33f)..];
                case OrganismTruncationMethod.ThreeQuarters:
                    return Organisms[^(int)(Organisms.Length * 0.25f)..];
                case OrganismTruncationMethod.Custom:
                    return Organisms[^(int)(Organisms.Length * CustomOrganismTruncationPercentage)..];
                case OrganismTruncationMethod.Random:
                    return RandomTruncate(ref Organisms);
                case OrganismTruncationMethod.Bool:
                    return BoolTruncate(ref Organisms, CustomOrganismTruncater);
            }
            throw new NotSupportedException($"Invalid or missing NeuroSharp.OrganismTruncationMethod selected. at Span<int> {nameof(TruncateSpecies)}(ref Span<int> Organisms)");
        }

        internal Span<int> RandomTruncate(ref Span<int> Organisms)
        {
            return BoolTruncate(ref Organisms, (ref int x) => Helpers.NextUDouble() >= 0.5d);
        }

        internal Span<int> BoolTruncate(ref Span<int> Organisms, ReferenceFunc<int, bool> Truncator)
        {
            int[] result = Array.Empty<int>();
            for (int i = 0; i < Organisms.Length; i++)
            {
                if (Truncator(ref i))
                {
                    Helpers.Array.AppendValue(ref result, ref Organisms[i]);
                }
            }
            return result;
        }
    }
}
