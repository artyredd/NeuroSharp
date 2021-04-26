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
        public double CheckFitness(double[] Result)
        {
            return AverageDoubleArray(Result);
        }

#pragma warning disable CS1998
        public async Task<double> CheckFitnessAsync(double[] Result)
        {
            return AverageDoubleArray(Result);
        }

        internal double AverageDoubleArray(double[] Inputs)
        {
            double average = 0d;

            Span<double> nums = new(Inputs);
            for (int i = 0; i < nums.Length; i++)
            {
                average += nums[i];
            }

            return average / nums.Length;
        }
    }
#pragma warning restore CS1998
}
