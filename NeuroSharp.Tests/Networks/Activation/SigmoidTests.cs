using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace NeuroSharp.Tests.Networks.Activation
{
    public class SigmoidTests
    {
        private readonly NeuroSharp.Activation.Sigmoid Sigmoid = new();
        private readonly Random rng = new();
        [Fact]
        public void AlwaysWithinRange()
        {
            List<double> vals = new();
            for (int i = 0; i < 1000; i++)
            {
                double tmp = rng.NextDouble() * int.MaxValue;
                // make sure the sigmoid function is actually working
                double changed = Sigmoid.Function(ref tmp);

                Assert.NotEqual(tmp, changed);

                vals.Add(changed);
            }
            foreach (var item in vals)
            {
                Assert.True(item < 1d);
                Assert.True(item > -1d);
            }
        }
    }
}
