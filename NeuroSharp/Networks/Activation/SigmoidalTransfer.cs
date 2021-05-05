using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuroSharp.Activation
{
    public class SigmoidalTransfer : IActivationFunction<double>
    {
        // see https://citeseerx.ist.psu.edu/viewdoc/download?doi=10.1.1.28.5457&rep=rep1&type=pdf pg 13 4.1
        private const double Coefficient = -4.9;
        public double Derivative(ref double value)
        {
            value *= 1d - (value / Coefficient);
            return value;
        }

        public double Function(ref double Value)
        {
            Value = 1d / (1d + Math.Pow(Math.E, -Coefficient * Value));
            return Value;
        }
    }
}
