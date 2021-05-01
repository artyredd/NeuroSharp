using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using NeuroSharp.NEAT;

namespace NeuroSharp.Tests
{
    public class NeatNeuralNetworkTests
    {
        [Fact]
        public void ConstructorWorks()
        {
            var nn = new NeatNueralNetwork(3, 2);

            // both arrays should be instantiated when the constructor is called
            Assert.NotNull(nn.Innovations);
        }

        [Fact]
        public void AddLocalAndGlobalInnovation()
        {
            NeatNueralNetwork.GlobalInnovations.Clear().Wait();

            Assert.Empty(NeatNueralNetwork.GlobalInnovations.InnovationHashes);

            Assert.Empty(NeatNueralNetwork.GlobalInnovations.Innovations);

            // since these are static and the tests are multi-threaded we should clear previous values so this test remains accurate
            var nn = new NeatNueralNetwork(3, 2);


            Assert.Equal(3 * 2, NeatNueralNetwork.GlobalInnovations.InnovationHashes.Count);
            Assert.Equal(3 * 2, nn.Innovations.Length);
            Assert.Equal(3 * 2, NeatNueralNetwork.GlobalInnovations.Innovations.Count());
            Assert.Equal(3 * 2, NeatNueralNetwork.GlobalInnovations.Count());

            Assert.Equal(3, nn.Innovations[0].OutputNode);
        }

        [Fact]
        public void AddInnovation()
        {
            // make sure when we add innovations that conlifcting conventions are not violated and duplicate innovations always return their original ids

            var left = new NeatNueralNetwork(3, 2);
            var leftInn = new Innovation()
            {
                InputNode = 0,
                OutputNode = 3,
                Weight = 2.0f,
                Enabled = true,
            };
            var right = new NeatNueralNetwork(3, 2);
            var rightInn = new Innovation()
            {
                InputNode = 0,
                OutputNode = 3,
                Weight = 0.334f,
                Enabled = false,
            };
            left.AddInnovation(leftInn).Wait();
            right.AddInnovation(rightInn).Wait();

            // even through both innovatiuons arer different objects they should both hash the same and therefor be assigned the same id number by the global innovations list
            Assert.Equal(left.Innovations[0].Id, right.Innovations[0].Id);
        }

        [Fact]
        public void TryGetEligibleConnectionToSplit()
        {
            var nn = new NeatNueralNetwork(1, 1);

            // if there are no innovations then there should be no eligible connections to split
            var pass = ((DefaultMutater)nn.Mutater).TryGetEligibleConnectionToSplit(out _, nn);

            // at this point the innovaiton should be enabled and eligble for splitting
            Assert.True(pass);

            // if there are only disbaled connections then there are no eligible connections
            nn.Innovations[0].Enabled = false;

            pass = ((DefaultMutater)nn.Mutater).TryGetEligibleConnectionToSplit(out var eligibleCons, nn);

            Assert.False(pass);
            Assert.Empty(eligibleCons);

            // enable it so there is an eligible connection
            nn.Innovations[0].Enabled = true;

            pass = ((DefaultMutater)nn.Mutater).TryGetEligibleConnectionToSplit(out eligibleCons, nn);

            // there should be 1 eligible connection returned
            Assert.True(pass);
            Assert.Single(eligibleCons);
        }

        [Fact]
        public void AddNode()
        {
            NeatNueralNetwork.GlobalInnovations.Clear();

            var nn = new NeatNueralNetwork(1, 1);

            // we shouldnt be able to add a node if there are no innovations
            Assert.Equal(AddNodeResult.success, ((DefaultMutater)nn.Mutater).AddNode(nn).Result);

            // when we split the connection the new node should have the same weight as the original connection, and the
            // original input node should have a weight of 1 to prevent the new topology from impacting the fittness too badly of the new
            // topolgy

            Assert.False(nn.Innovations[0].Enabled);

            // make sure the innovation was properly recorded in the global innovations and the old connection was disabled

            Assert.Equal(3, nn.Innovations.Length);

            // make sure that the hashes for the new innovations were recorded
            Assert.Contains(nn.Innovations[1].Hash(), nn.InnovationHashes);
            Assert.Contains(nn.Innovations[2].Hash(), nn.InnovationHashes);

            Assert.True(NeatNueralNetwork.GlobalInnovations.InnovationHashes.ContainsKey(nn.Innovations[1].Hash()));
            Assert.True(NeatNueralNetwork.GlobalInnovations.InnovationHashes.ContainsKey(nn.Innovations[2].Hash()));

            // make sure the node that was created was added to the genome
        }

        [Fact]
        public void AddConnection()
        {
            var nn = new NeatNueralNetwork(1, 1);

            var inn = new Innovation()
            {
                Enabled = true,
                InputNode = 0,
                OutputNode = 1,
                Weight = 0.122221d
            };

            // forcibly add an innovation to verify that we can't add duplicate connections
            nn.InnovationHashes.Add(inn.Hash());

            Assert.Equal(AddConnectionResult.alreadyExists, ((DefaultMutater)nn.Mutater).AddConnection(nn).Result);

            nn.InnovationHashes.Clear();

            Assert.Equal(AddConnectionResult.success, ((DefaultMutater)nn.Mutater).AddConnection(nn).Result);
        }

        [Fact]
        public void GeneratePhenotype()
        {
            var nn = CreateExampleNetwork();

            // manually create the exmaple from(but 0 indexed instead becuase i like it better)
            // https://citeseerx.ist.psu.edu/viewdoc/download?doi=10.1.1.28.5457&rep=rep1&type=pdf page 11

            nn.GeneratePhenotype();

            nn = CreateAlternateExample();

            nn.GeneratePhenotype();

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
