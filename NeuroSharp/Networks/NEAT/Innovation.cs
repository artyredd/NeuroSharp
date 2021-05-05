using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        public static INeatHasher Hasher { get; set; } = new DefaultHasher();

        public bool Equals(Innovation x, Innovation y)
        {
            return x?.InputNode == y?.InputNode && x?.OutputNode == y?.OutputNode && x?.Enabled == y?.Enabled && x?.Weight == y?.Weight;
        }

        public int GetHashCode([DisallowNull] Innovation obj)
        {
            return obj.GetHashCode();
        }

        public override string ToString()
        {
            return string.Format("{3} {0} → {1} —— {2} ", InputNode, OutputNode, Id, Enabled ? "✔" : "❌");
        }

        public string Hash() => Hasher.Hash(this);
    }
}
