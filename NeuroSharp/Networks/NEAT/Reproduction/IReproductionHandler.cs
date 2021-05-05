using System;

namespace NeuroSharp.NEAT
{
    public interface IReproductionHandler
    {
        double CustomOrganismTruncationPercentage { get; set; }
        ReferenceFunc<int, bool> CustomOrganismTruncater { get; set; }
        ReferenceFunc<ISpeciesFitness<double>, double, SpeciesReproductionRule> SpeciesSelectionFunction { get; set; }

        SpeciesReproductionRule[] SelectUnfitSpecies(ref ISpeciesFitness<double>[] Fitneses, ref double TotalGenerationFitness);
        Span<int> TruncateSpecies(ref Span<int> Organisms);
    }
}