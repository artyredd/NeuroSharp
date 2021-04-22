using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using NeuroSharp.NEAT;

namespace NeuroSharp.Tests.NEAT
{
    public class InnovationTests
    {

        public void HashWorks()
        {
            Innovation left = new()
            {
                Enabled = true,
                InputNode = 2,
                OutputNode = 4,
                Weight = 0.887199928829d
            };
            Innovation right = new()
            {
                Enabled = true,
                InputNode = 2,
                OutputNode = 4,
                Weight = 0.887199928829d
            };
            Assert.Equal(left.GetHashCode(), right.GetHashCode());
            Assert.Equal(left.Hash(), right.Hash());
        }
    }
}
