using NeuroSharp.NEAT;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static NeuroSharp.NEAT.DefaultFitnessFunction;

namespace NeuroSharp
{
    public interface IFitnessFunction<U, T>
    {
        /// <summary>
        /// The default fitness function just averages the outputs and returns the averaged results. Fitness function should be assigned a less arbitrary class for advanced problems. Or not assigned for simpler problems like game decisions.
        /// </summary>
        ReferenceFunc<double[], double> Function { get; set; }

        /// <summary>
        /// Calculates the raw fitness of an origisms evaluation results using <see cref="Function"/>
        /// </summary>
        /// <param name="OrginismEvaluationResults"></param>
        /// <returns></returns>
        T CalculateFitness(U OrginismEvaluationResults);

        /// <summary>
        /// Adjusts each organism's fitness so that it's proportional to the total members of the species.
        /// <code>
        /// Complexity: O(n)
        /// </code>
        /// </summary>
        /// <param name="SpeciesFitnesses"></param>
        /// <returns></returns>
        Span<T> AdjustSpeciesFitness(U SpeciesFitnesses, out T TotalFitness);

        /// <summary>
        /// Adjusts each organism's fitness so that it's proportional to the total members of the species.
        /// <code>
        /// Complexity: O(n)
        /// </code>
        /// </summary>
        /// <param name="SpeciesFitnesses"></param>
        /// <returns></returns>
        Span<T> AdjustSpeciesFitness(ref Span<T> SpeciesFitnesses, out T TotalFitness);

        T AdjustOrganismFitness(ref T Fitness, ref int SpeciesSize);
    }
}
