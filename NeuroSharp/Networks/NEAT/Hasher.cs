using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuroSharp.NEAT
{
    public class DefaultHasher : INeatHasher
    {
        /// <summary>
        /// Hashes the provided innovation so that it can be compared with others in the genome
        /// </summary>
        /// <param name="innovation"></param>
        /// <returns></returns>
        public string Hash(IInnovation innovation) => Hash(innovation.InputNode, innovation.OutputNode);

        public string Hash(int InputNode, int OutputNode)
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
