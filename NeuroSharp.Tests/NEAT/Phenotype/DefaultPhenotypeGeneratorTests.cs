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
        public async Task Check()
        {
            NeatNueralNetwork nn = (NeatNueralNetwork)CreateExampleNetwork();

            int[][] layers = await nn.PhenotypeGenerator.GetLayers(nn);

            // remember that the 0th layer is the output layer(it simplifies genotype construction)
            Assert.Equal(new int[] { 3 }, layers[0]);

            // the first layer(input layer) should have 0 1 and 2
            Assert.Equal(new int[] { 0, 1, 2 }, layers[1]);

            // the hidden layer should only have 4
            Assert.Equal(new int[] { 4 }, layers[2]);
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
    }

}
