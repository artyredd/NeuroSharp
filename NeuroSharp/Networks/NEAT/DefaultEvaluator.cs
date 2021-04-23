using NeuroSharp.Activation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuroSharp.NEAT
{
    public class DefaultEvaluator : IEvaluator
    {
        public IActivationFunction<double> Activator { get; set; } = new Sigmoid();

        public IFitnessFunction<double> FitnessFunction { get; set; }

        public double[] Evaluate(double[] Inputs, INeatNetwork network)
        {
            // load the inputs into memory
            Span<double> inputs = new(Inputs);

            // load the nodes in memory
            Span<INeatNode> nodes = new(network.Nodes);

            // slice the pertinent nodes into two types.
            Span<INeatNode> inputNodes = nodes.Slice(0, network.Inputs);
            Span<INeatNode> outputNodes = nodes.Slice(network.Inputs, network.Outputs);

            // load the values into the input nodes
            for (int i = 0; i < inputNodes.Length; i++)
            {
                inputNodes[i].Value = inputs[i];
            }

            double[] result = new double[outputNodes.Length];

            Span<double> resultSpan = new(result);

            // evaluate the output nodes value recursively
            for (int i = 0; i < outputNodes.Length; i++)
            {
                outputNodes[i].Value = EvaluateNode(outputNodes[i], network);
                resultSpan[i] = (double)outputNodes[i].Value;
            }

            return result;
        }

        private double EvaluateNode(INeatNode node, INeatNetwork network)
        {
            // recursively get the value of the node

            // it is assumed we started from an output node, therefor we should evaulate each node under us until they all have values and summ them with the activation function

            Span<IInnovation> inns = new(node.InputNodes);

            double value = 0;

            foreach (var innovation in inns)
            {
                // try to get the value of the input node
                ref INeatNode refNode = ref network.Nodes[innovation.InputNode];

                if (refNode.Value is null)
                {
                    // since the node below us has no value we should evaluate it recursively
                    refNode.Value = EvaluateNode(refNode, network);
                }

                // multiply the value by the weight
                value += (double)(refNode.Value * innovation.Weight);

                // consider using biases here ..?
            }

            // squishy the value with the activation function, IActivationFunctions mutate the value by reference, we should not need to set the value here again even though the function returns the value back to us
            Activator.Function(ref value);

            return value;
        }
    }
}
