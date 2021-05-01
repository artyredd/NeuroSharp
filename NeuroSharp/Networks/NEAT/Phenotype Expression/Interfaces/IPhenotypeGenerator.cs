using NeuroSharp.NEAT.Structs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NeuroSharp.NEAT
{
    public interface IPhenotypeGenerator<T> where T : unmanaged, IComparable<T>, IEquatable<T>
    {
        /// <summary>
        /// Decodes the Genome of a <see cref="INeatNetwork"/> into a <see cref="ReversableConcurrentDictionary{KeyType, ValueType}"/> (a concurrent wrapper with a reverse method for <see cref="Dictionary{TKey, TValue}"/>). Convertable to <see cref="IDictionary{TKey, TValue}"/> and <see cref="Dictionary{TKey, TValue}"/>.
        /// <code>
        /// Complexity: O(n)
        /// </code>
        /// </summary>
        /// <param name="network"></param>
        /// <returns></returns>
        DecodedGenome DecodeGenome(INeatNetwork network);
        IMatrix<T>[] GenerateMatrices(ref DecodedGenome genome);

        /// <summary>
        /// Compiles the layers found using <see cref="GenerateMatrices(INeatNetwork)"/> into a multi dimensional array where each index is an individual layer of nodes.
        /// <code>
        /// [0] is the output layer
        /// </code>
        /// <code>
        /// [1] is the input layer
        /// </code>
        /// <code>
        /// [n] is the nth layer
        /// </code>
        /// </summary>
        /// <returns></returns>
        int[][] GetLayers(ref DecodedGenome DecodedGenotype);

        /// <summary>
        /// Compiles the layers found using <see cref="GenerateMatrices(INeatNetwork)"/> into a multi dimensional array where each index is an individual layer of nodes.
        /// <code>
        /// [0] is the output layer
        /// </code>
        /// <code>
        /// [1] is the input layer
        /// </code>
        /// <code>
        /// [n] is the nth layer
        /// </code>
        /// </summary>
        /// <returns></returns>
        int[][] GetLayers(INeatNetwork network);
    }
}