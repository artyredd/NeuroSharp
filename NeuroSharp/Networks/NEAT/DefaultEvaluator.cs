using NeuroSharp.Activation;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NeuroSharp;
using NeuroSharp.Propagation;

namespace NeuroSharp.NEAT
{
    public class DefaultEvaluator : IEvaluator<double, double[], double>
    {
        public IActivationFunction<double> Activator { get; set; } = new SigmoidalTransfer();

        public IPropogator<double> Propogator { get; set; } = new FeedForward<double>();

        public double[] Evaluate(double[] Inputs, INeatNetwork network)
        {
            IMatrix<double> propogated = new Double.Matrix(Inputs.Length, 1, Inputs);

            for (int i = 0; i < network.Matrices.Length; i++)
            {
                propogated = Propogator.Forward(network.Matrices[i], propogated, Activator);
            }

            return propogated.ToOneDimension();
        }
    }
}
