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
        public double? Value { get; set; } = null;

        // we can avoid constantly hashing the object if we store the hash once
        internal string _Hash = null;

        public static INeatHasher Hasher { get; set; } = new DefaultHasher();

        public string Hash()
        {
            if (_Hash is not null)
            {
                return _Hash;
            }

            _Hash = Hasher.Hash(this);

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
