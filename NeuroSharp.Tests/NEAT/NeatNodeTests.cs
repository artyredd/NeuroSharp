using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using NeuroSharp.NEAT;
namespace NeuroSharp.Tests.NEAT
{
    //public class NeatNodeTests
    //{
    //    [Fact]
    //    public void DuplicateNotMutable()
    //    {
    //        INeatNode node = new NeuroSharp.NEAT.Node()
    //        {
    //            Id = 1,
    //            NodeType = NodeType.Hidden
    //        };

    //        var dupe = node.Duplicate();

    //        Assert.Equal(node.Id, dupe.Id);
    //        Assert.Equal(node.NodeType, dupe.NodeType);
    //    }

    //    [Fact]
    //    public void HashWorks()
    //    {
    //        // the hash that the node uses should generate a hex code from 4 byte string that distinctly identifies the nodes id and type
    //        var node = new Node()
    //        {
    //            Id = 1,
    //            NodeType = NodeType.Hidden
    //        };

    //        string hash = node.Hash();

    //        Assert.Equal(8, hash.Length);

    //        // create an identical node and verify that they have the same hash
    //        var other = new Node()
    //        {
    //            Id = 1,
    //            NodeType = NodeType.Hidden
    //        };

    //        string otherHash = other.Hash();

    //        Assert.Equal(node.Hash(), otherHash);

    //        // make sure different nodes have different hashes
    //        var other1 = new Node()
    //        {
    //            Id = 2,
    //            NodeType = NodeType.Hidden
    //        };
    //        var other2 = new Node()
    //        {
    //            Id = 1,
    //            NodeType = NodeType.Output
    //        };

    //        Assert.NotEqual(node.Hash(), other1.Hash());
    //        Assert.NotEqual(node.Hash(), other2.Hash());
    //        Assert.NotEqual(other1.Hash(), other2.Hash());
    //    }
    //}
}
