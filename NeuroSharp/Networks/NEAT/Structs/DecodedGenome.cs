using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuroSharp.NEAT.Structs
{
    /// <summary>
    /// Represents the explicit representation of an <see cref="IInnovation"/>[] Genome.
    /// <code>
    /// Do not pass around as a non-ref struct. This can potentially be gigantic, avoid copying this object needlessly
    /// </code>
    /// <code>
    /// ! DO NOT STORE AS FIELD, THIS CAN BE POTENTIALLY GIGANTIC
    /// </code>
    /// </summary>
    public ref struct DecodedGenome
    {
        /// <summary>
        /// The hasher used to compare <see cref="IInnovation"/>s for weight lookups
        /// </summary>
        public static INeatHasher Hasher { get; set; } = new DefaultHasher();

        /// <summary>
        /// Key: Node Id Value: Layer The Node is in
        /// <code>
        /// [0] is output layer
        /// </code>
        /// <code>
        /// [1] is input layer
        /// </code>
        /// <code>
        /// [n] is the n th layer
        /// </code>
        /// </summary>
        public IDictionary<int, ushort> NodeDictionary { get; init; }

        /// <summary>
        /// Key: Layer Id Id Value: Nodes within layer
        /// <code>
        /// [0] is output layer
        /// </code>
        /// <code>
        /// [1] is input layer
        /// </code>
        /// <code>
        /// [n] is the n th layer
        /// </code>
        /// </summary>
        public IDictionary<ushort, int[]> LayerDictionary { get; init; }

        /// <summary>
        /// Key: Node Id Id Value: Nodes that the node sends to
        /// <code>
        /// [0] is output layer
        /// </code>
        /// <code>
        /// [1] is input layer
        /// </code>
        /// <code>
        /// [n] is the n th layer
        /// </code>
        /// </summary>
        public IDictionary<int, int[]> SenderDictionary { get; init; }

        /// <summary>
        /// Key: Node Id Id Value: Nodes that this node inputs from
        /// <code>
        /// [0] is output layer
        /// </code>
        /// <code>
        /// [1] is input layer
        /// </code>
        /// <code>
        /// [n] is the n th layer
        /// </code>
        /// </summary>
        public IDictionary<int, int[]> ReceieverDictionary { get; init; }

        /// <summary>
        /// Key: Hashed IInovation, see: <see cref="IInnovation.Hash()"/>, Value: the <see cref="IInnovation"/>
        /// </summary>
        public IDictionary<string, IInnovation> InnovationDictionary { get; init; }

        public bool TryLookupInnovation(int fromNode, int toNode, out IInnovation innovation)
        {
            string hash = Hasher.Hash(fromNode, toNode);
            if (InnovationDictionary.ContainsKey(hash))
            {
                innovation = InnovationDictionary[hash];
                return true;
            }
            innovation = default;
            return false;
        }
    }
}
