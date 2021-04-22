using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuroSharp.NEAT
{
    public class Node : INeatNode
    {
        public ushort Id { get; init; }

        public NodeType NodeType { get; init; }

        public IInnovation[] OutputNodes { get; set; }

        public IInnovation[] InputNodes { get; set; }

        // we can avoid constantly hashing the object if we store the hash once
        protected string _Hash = null;

        public string Hash()
        {
            if (_Hash is not null)
            {
                return _Hash;
            }

            // this explicitly generates a hash that encodes this innvotation excluding the weight
            byte[] EncodedBytes = new byte[4];

            Span<byte> EncodedSpan = new(EncodedBytes);

            Span<byte> inputNode = BitConverter.GetBytes(Id);

            Span<byte> outputNode = BitConverter.GetBytes((ushort)NodeType);

            inputNode.CopyTo(EncodedSpan.Slice(0, 2));

            outputNode.CopyTo(EncodedSpan.Slice(2, 2));

            _Hash = Convert.ToHexString(EncodedSpan);

            return _Hash;
        }

        public INeatNode Duplicate()
        {
            return new Node()
            {
                Id = this.Id,
                NodeType = this.NodeType,
                _Hash = this._Hash
            };
        }
    }
}
