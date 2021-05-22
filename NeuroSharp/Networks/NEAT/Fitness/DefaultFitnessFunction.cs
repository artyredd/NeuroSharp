using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuroSharp.NEAT
{
    /// <summary>
    /// The default fitness function just averages the outputs and returns the averaged results. Fitness function should be assigned a less arbitrary class for advanced problems. Or not assigned for simpler problems like game decisions.
    /// </summary>
    public class DefaultFitnessFunction : IFitnessFunction<double[], double>
    {
        /// <summary>
        /// The <see cref="ReferenceFunc{T, TResult}"/> that accepts a <see langword="double"/>[] (the results from evaluating a single organism) and outputs a double(the overall raw Fitness of the network's results).
        /// <para>
        /// Example:
        /// <code>
        /// IFitnessFuncion fitter = new();
        /// </code>
        /// <code>
        /// fitter.Function = (ref double[] evalutationResults) => evaulationResults.Sum();
        /// </code>
        /// <code>
        /// fitt.CalculateFitness(new double[]{1,2,3,4,5});
        /// </code>
        /// <para>
        /// Output:
        /// <code>
        /// 15 (the summed value of the doubles)
        /// </code>
        /// </para>
        /// </para>
        /// </summary>
        public ReferenceFunc<double[], double> Function { get; set; } = (ref double[] arr) => { throw new Exception("Failed to Adjust organism species, did you forget to provide a method to calculate the fitness for organisms? Set IFitnessFunction<T,U>.Function"); };

        public double CalculateFitness(double[] OrginismEvaluationResults)
        {
            return Function(ref OrginismEvaluationResults);
        }

        /// <summary>
        /// Adjusts the fitness for every element(according to population size) and totals the fitness for the provided set.
        /// </summary>
        /// <param name="RawFitnesses"></param>
        /// <param name="TotalFitness"></param>
        /// <returns></returns>
        public Span<double> AdjustSpeciesFitness(ref Span<double> RawFitnesses, out double TotalFitness)
        {
            // the adjusted fitness of any organism is (or reduces to) fitness/population size
            // pg 12 https://citeseerx.ist.psu.edu/viewdoc/download?doi=10.1.1.28.5457&rep=rep1&type=pdf
            int populationSize = RawFitnesses.Length;

            TotalFitness = default;

            double AdjustFitness(ref double fitness)
            {
                return fitness / populationSize;
            }

            for (int i = 0; i < populationSize; i++)
            {
                ref double fitness = ref RawFitnesses[i];
                fitness = AdjustFitness(ref fitness);
                TotalFitness += fitness;
            }

            return RawFitnesses;
        }

        /// <summary>
        /// Adjusts the fitness for every element(according to population size) and totals the fitness for the provided set.
        /// </summary>
        /// <param name="RawFitnesses"></param>
        /// <param name="TotalFitness"></param>
        /// <returns></returns>
        public Span<double> AdjustSpeciesFitness(double[] RawFitnesses, out double TotalFitness)
        {
            // the adjusted fitness of any organism is (or reduces to) fitness/population size
            // pg 12 https://citeseerx.ist.psu.edu/viewdoc/download?doi=10.1.1.28.5457&rep=rep1&type=pdf
            Span<double> fitnesses = new(RawFitnesses);

            return AdjustSpeciesFitness(ref fitnesses, out TotalFitness);
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

        public double AdjustOrganismFitness(ref double Fitness, ref int SpeciesSize)
        {
            return Fitness / SpeciesSize;
        }
    }
}
