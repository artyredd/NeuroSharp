using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using NeuroSharp.NEAT;

namespace NeuroSharp.Tests.NEAT.Evaluations
{
    public class DefaultEvaluator
    {
        [Fact]
        public async Task CalculatesCorrectly()
        {
            var nn = new NeatNueralNetwork(1, 1);

            // force mutation
            await ((DefaultMutater)nn.Mutater).AddConnection(nn);

            nn.GeneratePhenotype();

            double[] inputs = new double[] { 1 };

            return;
            // evaluate the network
            double[] result = nn.Evaluator.Evaluate(inputs, nn);

            // make sure we only got the amount of outputnodes we originally started with
            Assert.Single(result);

            // make sure the number was squishified
            Assert.True(result[0] >= 0);
            Assert.True(result[0] <= 1);

            // further complicate the network to verify that recursion works correctly
            await ((DefaultMutater)nn.Mutater).AddNode(nn);

            nn.GeneratePhenotype();

            // now there should be a node inbetween the input and output, however becuase NEAT mutation dictates normal mutation of a node
            // as the original connection has it's weight set to 1 we should expect a similar or the same number from the second recursive eval

            double storedVal = result[0];

            result = nn.Evaluator.Evaluate(inputs, nn);

            // make sure we only got the amount of outputnodes we originally started with
            Assert.Single(result);

            // make sure the number was squishified
            Assert.True(result[0] > 0);
            Assert.True(result[0] <= 1);

            Assert.Equal(storedVal, result[0]);

            // further complicate the network to ensure the 3rd level of recursion works as well
            await ((DefaultMutater)nn.Mutater).AddNode(nn);

            nn.GeneratePhenotype();

            storedVal = result[0];

            result = nn.Evaluator.Evaluate(inputs, nn);

            // make sure we only got the amount of outputnodes we originally started with
            Assert.Single(result);

            // make sure the number was squishified
            Assert.True(result[0] > 0);
            Assert.True(result[0] <= 1);

            Assert.Equal(storedVal, result[0]);
        }
    }
}
