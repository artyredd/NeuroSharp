using System;
using System.Threading.Tasks;

namespace NeuroSharp.NEAT
{
    public interface IPhenotypeGenerator<T> where T : unmanaged, IComparable<T>, IEquatable<T>
    {
        Task<ReversableConcurrentDictionary<int, int>> DecodeGenome(INeatNetwork network);
        IMatrix<T>[] GenerateMatrices(INeatNetwork network);
        Task<int[][]> GetLayers(INeatNetwork network);
        Task<int[][]> GetLayers(ReversableConcurrentDictionary<int, int> DecodedGenotype);
    }
}