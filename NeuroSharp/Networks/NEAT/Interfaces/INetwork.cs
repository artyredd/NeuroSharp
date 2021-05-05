using System.Threading.Tasks;

namespace NeuroSharp.NEAT
{
    /// <summary>
    /// Defines a Neural Network with basic methods and information for general use
    /// </summary>
    /// <typeparam name="T">The <see cref="System.Type"/> that the inputs of the evaluation methods should accept, default is some value type array, ex <see cref="double []"/></typeparam>
    /// <typeparam name="U">
    /// The <see cref="System.Type"/> that should be returned by <see cref="EvaluateWithFitness(T)"/></typeparam>
    /// </typeparam>
    public interface INetwork<T, U>
    {
        string Name { get; set; }
        int InputNodes { get; }

        int OutputNodes { get; }

        int Count { get; }

        IEvaluator<U, T, U> Evaluator { get; init; }

        /// <summary>
        /// Take the provided data and runs it through the network and returns the result.
        /// </summary>
        /// <param name="Data"></param>
        /// <returns></returns>
        T Evaluate(T Data);
    }
}