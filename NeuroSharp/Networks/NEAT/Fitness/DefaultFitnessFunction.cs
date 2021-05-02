using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuroSharp.NEAT
{
    /// <summary>
    /// Represents an <see cref="Action{T1}"/> that accepts and mutates a by-reference <typeparamref name="T"/>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="value"></param>
    /// <returns></returns>
    public delegate U ReferenceAction<T, U>(ref T value);

    /// <summary>
    /// The default fitness function just averages the outputs and returns the averaged results. Fitness function should be assigned a less arbitrary class for advanced problems. Or not assigned for simpler problems like game decisions.
    /// </summary>
    public class DefaultFitnessFunction : IFitnessFunction<double[], double>
    {
        public Span<double> CalculateFitnesses(double[] OrginismEvaluationResults, ReferenceAction<double, double[]> ActionThatCalculatesFitness)
        {
            Span<double> results = new(OrginismEvaluationResults);
            for (int i = 0; i < results.Length; i++)
            {
                ActionThatCalculatesFitness(ref results[i]);
            }
            return results;
        }

        public double CalculateFitnesses(double[] OrginismEvaluationResults, ReferenceAction<double[], double> ActionThatCalculatesFitness)
        {
            return ActionThatCalculatesFitness(ref OrginismEvaluationResults);
        }

        public Span<double> AdjustSpeciesFitnesses(ref Span<double> fitnesses)
        {
            // the adjusted fitness of any organism is (or reduces to) fitness/population size
            // pg 12 https://citeseerx.ist.psu.edu/viewdoc/download?doi=10.1.1.28.5457&rep=rep1&type=pdf
            int populationSize = fitnesses.Length;

            double AdjustFitness(ref double fitness)
            {
                return fitness / populationSize;
            }

            for (int i = 0; i < populationSize; i++)
            {
                ref double fitness = ref fitnesses[i];
                fitness = AdjustFitness(ref fitness);
            }

            return fitnesses;
        }

        public Span<double> AdjustSpeciesFitnesses(double[] SpeciesFitnesses)
        {
            // the adjusted fitness of any organism is (or reduces to) fitness/population size
            // pg 12 https://citeseerx.ist.psu.edu/viewdoc/download?doi=10.1.1.28.5457&rep=rep1&type=pdf
            Span<double> fitnesses = new(SpeciesFitnesses);
            return AdjustSpeciesFitnesses(ref fitnesses);
        }

        public double[] GetProportionalFitnesses(double[] SummedSpeciesFitnesses)
        {
            Span<double> fitnesses = new(SummedSpeciesFitnesses);

            // get the total fitnesses for all the species
            double total = SumDoubleSpan(ref fitnesses);

            // adjust each value so that it's proportional to it's overall worth compared to the total
            for (int i = 0; i < fitnesses.Length; i++)
            {
                ref double fitness = ref fitnesses[i];
                fitness /= total;
            }

            return fitnesses.ToArray();
        }

        internal double SumDoubleSpan(ref Span<double> numbers)
        {
            double sum = 0d;
            for (int i = 0; i < numbers.Length; i++)
            {
                sum += numbers[i];
            }
            return sum;
        }
    }
}
