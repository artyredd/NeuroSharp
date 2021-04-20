using System;

namespace NeuroSharp
{
    public interface INode<T> where T : unmanaged, IEquatable<T>, IComparable<T>
    {
        IMatrix<T> Biases { get; set; }
        IMatrix<T> Weights { get; set; }
    }
}