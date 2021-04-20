namespace NeuroSharp
{
    public interface IActivationFunction<T>
    {
        T Function(ref T Value);
        T Derivative(ref T Value);
    }
}