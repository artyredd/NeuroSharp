using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuroSharp.Activation
{
    public class Sigmoid : IActivationFunction<double>
    {
        public double Derivative(ref double value)
        {
            value *= (1d - value);
            return value;
        }

        public double Function(ref double Value)
        {
            Value = 1d / (1d + Math.Pow(Math.E, -Value));
            return Value;
        }
    }
}
