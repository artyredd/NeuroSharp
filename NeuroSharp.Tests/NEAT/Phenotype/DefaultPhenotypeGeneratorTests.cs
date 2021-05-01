using NeuroSharp.NEAT;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace NeuroSharp.Tests.NEAT
{
    public class DefaultPhenotypeGeneratorTests
    {
        [Fact]
        public void Check()
        {
            NeatNueralNetwork nn = (NeatNueralNetwork)CreateExampleNetwork();

            int[][] layers = nn.PhenotypeGenerator.GetLayers(nn);

            // remember that the 0th layer is the output layer(it simplifies genotype construction)
            Assert.Equal(new int[] { 3 }, layers[0]);

            // the first layer(input layer) should have 0 1 and 2
            Assert.Equal(new int[] { 0, 1, 2 }, layers[1]);

            // the hidden layer should only have 4
            Assert.Equal(new int[] { 4 }, layers[2]);
        }

        [Fact]
        public void GetForwardedNodesWorks()
        {
            var network = CreateExampleNetwork();

            // get the genome of the network so we can call internal methods with it
            var genome = ((NeatNueralNetwork)network).PhenotypeGenerator.DecodeGenome(network);

            // get the actual structure of the genome
            int[][] layers = ((NeatNueralNetwork)network).PhenotypeGenerator.GetLayers(ref genome);

            // try to get an array of nodes that reference more than one layer
            int[][] forwardedNodes = ((DefaultPhenotypeGenerator<double>)((NeatNueralNetwork)network).PhenotypeGenerator).GetForwardedNodes(ref layers, ref genome);

            // we should get as many layers back as we gave ,minus 1
            Assert.Equal(layers.Length - 1, forwardedNodes.Length);

            // since we are using the exmaple node 2 should be a forwarded node since it references the output layer
            // make sure that the genome actually represents that fact
            Assert.Contains(3, genome.SenderDictionary[2]);

            // now that we know for a fact that 2 references the output node we should check to see if the node was marked as being forwarded
            Assert.Contains(2, forwardedNodes[0]);

            // make sure that no other nodes were marked as needing to be forwarded
            Assert.Empty(forwardedNodes[1]);
        }

        [Fact]
        public void GetForwardedNodesAdvanced()
        {
            // this is to test an edge case for the forwarded nodes
            // if a node references a layer far away so that they need to be forwarded multiple times we should make sure it
            // appears in each forwarding layer that it needs
            var network = CreateMultiForwardExample();

            // get the genome of the network so we can call internal methods with it
            var genome = ((NeatNueralNetwork)network).PhenotypeGenerator.DecodeGenome(network);

            // get the actual structure of the genome
            int[][] layers = ((NeatNueralNetwork)network).PhenotypeGenerator.GetLayers(ref genome);

            // try to get an array of nodes that reference more than one layer
            int[][] forwardedNodes = ((DefaultPhenotypeGenerator<double>)((NeatNueralNetwork)network).PhenotypeGenerator).GetForwardedNodes(ref layers, ref genome);

            // we should get as many layers back as we gave ,minus 1
            Assert.Equal(layers.Length - 1, forwardedNodes.Length);

            // for this example nodes 1 and nodes 2 should be forwarded for the first and node 2 should also be forward to the second
            // before we check for that make sure the genome was decoded to reflect that
            Assert.Contains(3, genome.SenderDictionary[2]);
            Assert.Contains(4, genome.SenderDictionary[2]);
            Assert.Contains(4, genome.SenderDictionary[1]);
            Assert.Contains(5, genome.SenderDictionary[0]);
            Assert.Contains(4, genome.SenderDictionary[5]);
            Assert.Contains(3, genome.SenderDictionary[4]);

            // now that we know for a fact that 2 references the output node we should check to see if the node was marked as being forwarded
            Assert.Contains(2, forwardedNodes[0]);

            // we should also check to make sure input node 2 is also forwarded to the second layer as well since it's not used until the output layer
            Assert.Contains(2, forwardedNodes[1]);
        }

        INeatNetwork CreateExampleNetwork()
        {
            // this is a duplicate test that tests disjoint as the other does not
            // create two neurual networks manually to ensure consistent results
            NeatNueralNetwork left = new(3, 1);
            left.Innovations = new IInnovation[] {
                new Innovation(){
                    Id = 1,
                    InputNode = 0,
                    OutputNode = 4,
                    Enabled = true,
                    Weight = 1
                },
                new Innovation(){
                    Id = 2,
                    InputNode = 1,
                    OutputNode = 4,
                    Enabled = true,
                    Weight = 1
                },
                new Innovation(){
                    Id = 4,
                    InputNode = 1,
                    OutputNode = 3,
                    Enabled = false,
                    Weight = 1
                },
                new Innovation(){
                    Id = 5,
                    InputNode = 2,
                    OutputNode = 3,
                    Enabled = true,
                    Weight = 1
                },
                new Innovation(){
                    Id = 6,
                    InputNode = 4,
                    OutputNode = 3,
                    Enabled = true,
                    Weight = 1
                },
            };
            // we use hashes to verify genes make sure to hash them
            foreach (var item in left.Innovations)
            {
                left.InnovationHashes.Add(item.Hash());
            }
            return left;
        }

        INeatNetwork CreateAlternateExample()
        {
            NeatNueralNetwork right = new(3, 1);
            right.Innovations = new IInnovation[] {
                new Innovation(){
                    Id = 1,
                    InputNode = 0,
                    OutputNode = 4,
                    Enabled = false,
                    Weight = 1
                },
                new Innovation(){
                    Id = 2,
                    InputNode = 1,
                    OutputNode = 4,
                    Enabled = true,
                    Weight = 1
                },
                new Innovation(){
                    Id = 3,
                    InputNode = 2,
                    OutputNode = 4,
                    Enabled = true,
                    Weight = 1
                },
                new Innovation(){
                    Id = 4,
                    InputNode = 1,
                    OutputNode = 3,
                    Enabled = false,
                    Weight = 1
                },
                new Innovation(){
                    Id = 6,
                    InputNode = 4,
                    OutputNode = 3,
                    Enabled = true,
                    Weight = 1
                },
                new Innovation(){
                    Id = 7,
                    InputNode = 0,
                    OutputNode = 5,
                    Enabled = true,
                    Weight = 1
                },
                new Innovation(){
                    Id = 8,
                    InputNode = 5,
                    OutputNode = 4,
                    Enabled = true,
                    Weight = 1
                },
            };

            // we use hashes to verify genes make sure to hash them
            foreach (var item in right.Innovations)
            {
                right.InnovationHashes.Add(item.Hash());
            }
            return right;
        }

        INeatNetwork CreateMultiForwardExample()
        {
            NeatNueralNetwork right = new(3, 1);
            right.Innovations = new IInnovation[] {
                new Innovation(){
                    Id = 1,
                    InputNode = 0,
                    OutputNode = 4,
                    Enabled = false,
                    Weight = 1
                },
                new Innovation(){
                    Id = 2,
                    InputNode = 1,
                    OutputNode = 4,
                    Enabled = true,
                    Weight = 1
                },
                new Innovation(){
                    Id = 3,
                    InputNode = 2,
                    OutputNode = 4,
                    Enabled = true,
                    Weight = 1
                },
                new Innovation(){
                    Id = 4,
                    InputNode = 1,
                    OutputNode = 3,
                    Enabled = false,
                    Weight = 1
                },
                new Innovation(){
                    Id = 6,
                    InputNode = 4,
                    OutputNode = 3,
                    Enabled = true,
                    Weight = 1
                },
                new Innovation(){
                    Id = 7,
                    InputNode = 0,
                    OutputNode = 5,
                    Enabled = true,
                    Weight = 1
                },
                new Innovation(){
                    Id = 8,
                    InputNode = 5,
                    OutputNode = 4,
                    Enabled = true,
                    Weight = 1
                },
                new Innovation(){
                    Id = 9,
                    InputNode = 2,
                    OutputNode = 3,
                    Enabled = true,
                    Weight = 1
                },
            };

            // we use hashes to verify genes make sure to hash them
            foreach (var item in right.Innovations)
            {
                right.InnovationHashes.Add(item.Hash());
            }
            return right;
        }
    }

}
