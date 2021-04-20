using System;

namespace NeuroSharp.Propagation
{
    public interface IPropogator<T> where T : unmanaged, IComparable<T>, IEquatable<T>
    {
        IMatrix<T> PropogateForward(INode<T> Node, IMatrix<T> Matrix, IActivationFunction<T> Activator);
    }
}