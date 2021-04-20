using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuroSharp
{
    public class Node<T> : INode<T> where T : unmanaged, IComparable<T>, IEquatable<T>
    {
        public IMatrix<T> Weights { get; set; }
        public IMatrix<T> Biases { get; set; }

        public Node()
        {

        }

        public Node(IMatrix<T> weights, IMatrix<T> biases)
        {
            Weights = weights;
            Biases = biases;
        }
    }
}
