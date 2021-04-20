using System;

namespace NeuroSharp.Propagation
{
    public interface IPropogator<T> where T : unmanaged, IComparable<T>, IEquatable<T>
    {
        IMatrix<T> Forward(INode<T> Node, IMatrix<T> Matrix, IActivationFunction<T> Activator);
        (IMatrix<T> WeightDelta, IMatrix<T> BiasDelta) Backward(IMatrix<T> Output, IMatrix<T> Error, IMatrix<T> PreviousLayerOutput, double TrainingRate, IActivationFunction<T> Activator);
    }
}