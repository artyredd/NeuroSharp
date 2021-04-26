using System;

namespace NeuroSharp.NEAT
{
    public interface IPhenotypeGenerator<T> where T : unmanaged, IComparable<T>, IEquatable<T>
    {
        IMatrix<T>[] Generate(INeatNetwork network);
    }
}