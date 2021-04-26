namespace NeuroSharp.NEAT
{
    /// <summary>
    /// Defines the object responsible for evaluating inputs using a given network and determining the organisms fitness
    /// </summary>
    /// <typeparam name="T">The value type that the inputs are in, and the weights are in.</typeparam>
    /// <typeparam name="U">The type of result the evaluator returns after using the inputs</typeparam>
    /// <typeparam name="V">The type of result the evaluator returns after using the inputs and running them through the fitness function</typeparam>
    public interface IEvaluator<T, U, V>
    {
        IActivationFunction<T> Activator { get; set; }
        IFitnessFunction<U, T> FitnessFunction { get; set; }

        U Evaluate(T[] Inputs, INeatNetwork network);

        V EvaluateWithFitness(T[] Inputs, INeatNetwork network);
    }
}