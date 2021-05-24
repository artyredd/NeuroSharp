using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace NeuroSharp.Tests
{
    public class HelperTests
    {
        [Fact]
        public void NextDoubleAlwaysReturnsWithinRange()
        {
            for (int i = 0; i < 1_000; i++)
            {
                double x = Helpers.Random.NextDouble();
                Assert.True(x is < 1d and > -1d);
            }
        }
    }
}
