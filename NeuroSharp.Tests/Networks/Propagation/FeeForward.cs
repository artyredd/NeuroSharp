using NeuroSharp.Activation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace NeuroSharp.Tests
{
    public class FeeForwardTests
    {
        private readonly Random rng = new();
        readonly Propagation.FeedForward<double> Propogator = new();
        readonly IActivationFunction<double> Activator = new Sigmoid();
        [Fact]
        public void DoesNotMutate()
        {
            var inputNode = CreateNode(2, 10);
            IMatrix<double> inputs = new Double.Matrix(10, 1);

            // store these later to verify if the values have been mutated
            IMatrix<double> weightsStored = inputNode.Weights.Duplicate();
            IMatrix<double> biasesStored = inputNode.Biases.Duplicate();

            IMatrix<double> propogated = Propogator.Forward(inputNode, inputs, Activator);

            // make sure the values have not been mutated
            Assert.Equal(weightsStored, inputNode.Weights);
            Assert.Equal(biasesStored, inputNode.Biases);

            // make sure the function actually did something to the input though
            Assert.Equal(2, propogated.Rows);
            Assert.Equal(1, propogated.Columns);

            Assert.NotEqual(0, propogated[0, 0]);
            Assert.NotEqual(1, propogated[0, 0]);

            Assert.NotEqual(0, propogated[1, 0]);
            Assert.NotEqual(1, propogated[1, 0]);
        }
        INode<double> CreateNode(int Rows, int Columns)
        {
            return new Node<double>()
            {
                Weights = new Double.Matrix(Rows, Columns, GetRandom()),
                Biases = new Double.Matrix(Rows, 1, GetRandom())
            };
        }
        private IEnumerable<double> GetRandom()
        {
            do
            {
                yield return rng.NextDouble() * rng.Next(-1, 1);
            } while (true);
        }
    }
}
