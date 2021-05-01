using NeuroSharp.Activation;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuroSharp.NEAT
{
    public class DefaultEvaluator : IEvaluator<double, double[], double>
    {
        public IActivationFunction<double> Activator { get; set; } = new Sigmoid();

        public IFitnessFunction<double[], double> FitnessFunction { get; set; } = new DefaultFitnessFunction();

        public double EvaluateWithFitness(double[] Inputs, INeatNetwork network)
        {
            double[] eval = Evaluate(Inputs, network);
            return FitnessFunction.CheckFitness(eval);
        }

        [DoesNotReturn]
        public double[] Evaluate(double[] Inputs, INeatNetwork network)
        {
            throw new NotImplementedException();
        }
    }
}
