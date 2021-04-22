using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using NeuroSharp.NEAT;
namespace NeuroSharp.Tests.NEAT
{
    public class NeatNodeTests
    {
        [Fact]
        public void DuplicateNotMutable()
        {
            INeatNode node = new NeuroSharp.NEAT.Node()
            {
                Id = 1,
                NodeType = NodeType.Hidden
            };

            var dupe = node.Duplicate();

            Assert.Equal(node.Id, dupe.Id);
            Assert.Equal(node.NodeType, dupe.NodeType);
        }
    }
}
