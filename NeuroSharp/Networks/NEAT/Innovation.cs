using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace NeuroSharp.NEAT
{
    public class Innovation : IInnovation, IEqualityComparer<Innovation>
    {
        /// <summary>
        /// The innovation ID of this object
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// The node that accepts the input for this innovation. InputNode -> Weight -> OutputNode
        /// </summary>
        public ushort InputNode { get; init; }

        /// <summary>
        /// The node that should recieve the value caluculated by this node.
        /// </summary>
        public ushort OutputNode { get; init; }

        /// <summary>
        /// Whether or not this innovation is active
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        /// The weight for this node
        /// </summary>
        public double Weight { get; set; }

        public bool Equals(Innovation x, Innovation y)
        {
            return x?.InputNode == y?.InputNode && x?.OutputNode == y?.OutputNode && x?.Enabled == y?.Enabled && x?.Weight == y?.Weight;
        }

        public int GetHashCode([DisallowNull] Innovation obj)
        {
            return obj.GetHashCode();
        }

        public string Hash()
        {
            // this explicitly generates a hash that encodes this innvotation excluding the weight
            byte[] EncodedBytes = new byte[8];

            Span<byte> EncodedSpan = new(EncodedBytes);

            Span<byte> inputNode = BitConverter.GetBytes(InputNode);

            Span<byte> outputNode = BitConverter.GetBytes(OutputNode);

            inputNode.CopyTo(EncodedSpan.Slice(0, 4));

            outputNode.CopyTo(EncodedSpan.Slice(2, 4));

            return Convert.ToHexString(EncodedSpan);
        }
    }
}
