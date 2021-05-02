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
        /// Invokes the provided delegate on the whole results of the network.
        /// <para>
        /// <code>
        /// The delegate MUST MUTATE THE RESULT BY REFERENCE
        /// </code>
        /// Example:
        /// </para>
        /// <code>
        /// double[] orginismResults = {1, 2, 3, 4};
        /// </code>
        /// <code>
        /// var fitnesses = CalculateFitnesses(orginismResults,(ref double[] arr) => arr.Select(x=>x+1));
        /// </code>
        /// <para>
        /// Results:
        /// <code>
        /// double[] {2, 3, 4, 5}
        /// </code>
        /// </para>
        /// </summary>
        /// <param name="OrginismEvaluationResults"></param>
        /// <param name="ActionThatCalculatesFitness"></param>
        /// <returns></returns>
        T CalculateFitnesses(U OrginismEvaluationResults, ReferenceAction<U, T> ActionThatCalculatesFitness);
        /// <summary>
        /// Adjusts each organism's fitness so that it's proportional to the total members of the species.
        /// <code>
        /// Complexity: O(n)
        /// </code>
        /// </summary>
        /// <param name="SpeciesFitnesses"></param>
        /// <returns></returns>
        Span<T> AdjustSpeciesFitnesses(U SpeciesFitnesses);
        /// <summary>
        /// Adjusts each organism's fitness so that it's proportional to the total members of the species.
        /// <code>
        /// Complexity: O(n)
        /// </code>
        /// </summary>
        /// <param name="SpeciesFitnesses"></param>
        /// <returns></returns>
        Span<T> AdjustSpeciesFitnesses(ref Span<T> SpeciesFitnesses);
        /// <summary>
        /// Calculates which species performed better and assigns them a proportional value for how well that species did
        /// <code>
        /// Complexity: O(n)
        /// </code>
        /// </summary>
        /// <param name="SummedSpeciesFitnesses"></param>
        /// <returns></returns>
        U GetProportionalFitnesses(U SummedSpeciesFitnesses);
    }
}
