using System;

namespace NeuroSharp.NEAT
{
    public interface IReproductionHandler
    {
        double CustomOrganismTruncationPercentage { get; set; }
        ReferenceFunc<OrganismStruct, bool> CustomOrganismTruncater { get; set; }
        ReferenceFunc<ISpeciesFitness<double>, double, SpeciesReproductionRule> SpeciesSelectionFunction { get; set; }

        SpeciesReproductionRule[] SelectUnfitSpecies(ref ISpeciesFitness<double>[] Fitneses, ref double TotalGenerationFitness);
        Span<OrganismStruct> TruncateSpecies(ref Span<OrganismStruct> Organisms, out Span<OrganismStruct> RemainingOrganisms);
        (OrganismStruct Left, OrganismStruct Right)[] GenerateBreedingPairs(ref Span<OrganismStruct> EligibleParentOrganisms);
    }
}