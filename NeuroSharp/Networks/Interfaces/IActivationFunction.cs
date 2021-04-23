namespace NeuroSharp
{
    public interface IActivationFunction<T>
    {
        /// <summary>
        /// Runs an activation function on the value by reference
        /// <code>
        /// This should mutate <paramref name="Value"/>
        /// </code>
        /// </summary>
        /// <param name="Value"></param>
        /// <returns></returns>
        T Function(ref T Value);

        /// <summary>
        /// Runs the derivative of the activation function on the value by reference
        /// <code>
        /// This should mutate <paramref name="Value"/>
        /// </code>
        /// </summary>
        /// <param name="Value"></param>
        /// <returns></returns>
        T Derivative(ref T Value);
    }
}