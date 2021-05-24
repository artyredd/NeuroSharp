using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using NeuroSharp;
using NeuroSharp.NEAT;

namespace NeuroSharp.Tests.Networks.Reproduction
{
    public class UnfitSpeciesSelection
    {
        private readonly IReproductionHandler<OrganismStruct> ReproductionHandler = new DefaultReproductionHandler<OrganismStruct>();
        [Fact]
        public void SelectionFunctionWorks()
        {
            ISpeciesFitness<double>[] Fitneses = new ISpeciesFitness<double>[] {
                new SpeciesEvaluationResult<double>(){ Fitness = 1.5, Fitnesses = new double[]{ 0, 2, 3},Species =0 },
                new SpeciesEvaluationResult<double>(){ Fitness = 1, Fitnesses = new double[]{ 1, 1, 1},Species =1 },
                new SpeciesEvaluationResult<double>(){ Fitness = 2, Fitnesses = new double[]{ 2, 3, 3},Species =2 },
                new SpeciesEvaluationResult<double>(){ Fitness = 3, Fitnesses = new double[]{ 3, 5, 5},Species =3 },
                new SpeciesEvaluationResult<double>(){ Fitness = 4, Fitnesses = new double[]{ 6, 6, 6},Species =4 },
                new SpeciesEvaluationResult<double>(){ Fitness = 4, Fitnesses = new double[]{ 6, 6, 6},Species =5 },
            };
            double totalGenerationFitness = 5;

            SpeciesReproductionRule Selector(ref ISpeciesFitness<double> species, ref double totalFitness)
            {
                return (SpeciesReproductionRule)species.Species;
            }

            SpeciesReproductionRule[] result = Array.Empty<SpeciesReproductionRule>();

            lock (ReproductionHandler)
            {
                ReproductionHandler.SpeciesSelectionFunction = Selector;
                result = ReproductionHandler.SelectUnfitSpecies(ref Fitneses, ref totalGenerationFitness);
            }

            Assert.Equal(Fitneses.Length, result.Length);

            Assert.Equal(SpeciesReproductionRule.Allow, result[0]);
            Assert.Equal(SpeciesReproductionRule.PreventMutation, result[1]);
            Assert.Equal(SpeciesReproductionRule.Prohibit, result[2]);
            Assert.Equal(SpeciesReproductionRule.ProhibitWithMutation, result[3]);
            Assert.Equal(SpeciesReproductionRule.Random, result[4]);
            Assert.Equal(SpeciesReproductionRule.Remove, result[5]);
        }
    }
}
