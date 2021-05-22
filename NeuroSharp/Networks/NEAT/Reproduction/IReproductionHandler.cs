using System;

namespace NeuroSharp.NEAT
{
    public interface IReproductionHandler<T>
    {
        double CustomOrganismTruncationPercentage { get; set; }

        OrganismTruncationMethod TrucationMethod { get; set; }

        OrganismReproductionMethod ReproductionMethod { get; set; }

        ReferenceFunc<T, bool> CustomOrganismTruncater { get; set; }

        ReferenceFunc<ISpeciesFitness<double>, double, SpeciesReproductionRule> SpeciesSelectionFunction { get; set; }

        SpeciesReproductionRule[] SelectUnfitSpecies(ref ISpeciesFitness<double>[] Fitneses, ref double TotalGenerationFitness);

        Span<T> TruncateOrganisms(ref Span<T> Organisms, out Span<T> RemainingOrganisms);

        Span<(T Left, T Right)> GenerateBreedingPairs(ref Span<T> EligibleParentOrganisms);
    }
}