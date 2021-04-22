using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using NeuroSharp.NEAT;

namespace NeuroSharp.Tests
{
    public class Main
    {
        [Fact]
        public void ConstructorWorks()
        {
            var nn = new NeatNueralNetwork(3, 2);

            // both arrays should be instantiated when the constructor is called
            Assert.NotNull(nn.Nodes);
            Assert.NotNull(nn.Innovations);
        }
        [Fact]
        public void AddNodeToGenomeWorks()
        {
            var nn = new NeatNueralNetwork(3, 2);
            var node = new NeuroSharp.NEAT.Node()
            {
                Id = 12
            };

            Assert.True(nn.Nodes.Length == 5);

            nn.AddNodeToGenome(node);

            Assert.True(nn.Nodes.Length == 6);

            Assert.Equal(node.Id, nn.Nodes[5].Id);
        }

        [Fact]
        public void AddLocalAndGlobalInnovation()
        {
            // since these are static and the tests are multi-threaded we should clear previous values so this test remains accurate
            NeatNueralNetwork.GlobalInnovationHashes.Clear();
            NeatNueralNetwork.GlobalInnovations = Array.Empty<IInnovation>();
            NeatNueralNetwork.CurrentGlobalInnovationIndex = 0;

            var nn = new NeatNueralNetwork(3, 2);

            Assert.Empty(nn.Innovations);

            Assert.Empty(NeatNueralNetwork.GlobalInnovationHashes);

            Assert.Empty(NeatNueralNetwork.GlobalInnovations);

            var innovation = new Innovation()
            {
                InputNode = 0,
                OutputNode = 3,
                Weight = 2.0f,
                Enabled = true,
            };

            nn.AddLocalAndGlobalInnovation(innovation).Wait();

            Assert.Single(NeatNueralNetwork.GlobalInnovationHashes);
            Assert.Single(nn.Innovations);
            Assert.Single(NeatNueralNetwork.GlobalInnovations);
            Assert.Equal(1, NeatNueralNetwork.CurrentGlobalInnovationIndex);

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
            left.AddLocalAndGlobalInnovation(leftInn).Wait();
            right.AddLocalAndGlobalInnovation(rightInn).Wait();

            // even through both innovatiuons arer different objects they should both hash the same and therefor be assigned the same id number by the global innovations list
            Assert.Equal(left.Innovations[0].Id, right.Innovations[0].Id);
        }

        [Fact]
        public void TryGetEligibleConnectionToSplit()
        {
            var nn = new NeatNueralNetwork(3, 2);

            Assert.Empty(nn.Innovations);

            var inn = new Innovation()
            {
                Enabled = false,
                Id = 12,
                InputNode = 1,
                OutputNode = 4
            };

            // if there are no innovations then there should be no eligible connections to split
            var pass = nn.TryGetEligibleConnectionToSplit(out var eligibleCons);

            Assert.False(pass);
            Assert.Empty(eligibleCons);

            // add the innovation to global
            nn.AddLocalAndGlobalInnovation(inn).Wait();

            // if there are only disbaled connections then there are no eligible connections
            nn.Innovations[0].Enabled = false;

            pass = nn.TryGetEligibleConnectionToSplit(out eligibleCons);

            Assert.False(pass);
            Assert.Empty(eligibleCons);

            // enable it so there is an eligible connection
            nn.Innovations[0].Enabled = true;

            pass = nn.TryGetEligibleConnectionToSplit(out eligibleCons);

            // there should be 1 eligible connection returned
            Assert.True(pass);
            Assert.Single(eligibleCons);
        }

        [Fact]
        public void GetEligibleNodesForNewConnection()
        {
            var nn = new NeatNueralNetwork(3, 2);

            var eligibleNodes = nn.GetEligibleNodesForNewConnection();

            Assert.Equal(3, eligibleNodes.EligibleInputNodes.Length);

            Assert.Equal(2, eligibleNodes.EligibleOutputNodes.Length);

            // hidden nodes are eligible for both inputs and output and should count for both

            var hidden = new NeuroSharp.NEAT.Node()
            {
                Id = 6,
                NodeType = NodeType.Hidden
            };

            nn.AddNodeToGenome(hidden);

            eligibleNodes = nn.GetEligibleNodesForNewConnection();

            Assert.Equal(4, eligibleNodes.EligibleInputNodes.Length);

            Assert.Equal(3, eligibleNodes.EligibleOutputNodes.Length);
        }

        [Fact]
        public void AddNode()
        {
            var nn = new NeatNueralNetwork(3, 2);
            var inn = new Innovation()
            {
                InputNode = 2,
                OutputNode = 3,
                Enabled = false,
                Weight = 1.333d,
                Id = 0
            };

            // we shouldnt be able to add a node if there are no innovations
            Assert.Equal(AddNodeResult.noEligibleConnections, nn.AddNode().Result);

            // we shouldnt be able to split a disabled connection
            nn.AddLocalAndGlobalInnovation(inn).Wait();

            Assert.Equal(AddNodeResult.noEligibleConnections, nn.AddNode().Result);

            NeatNueralNetwork.CurrentGlobalInnovationIndex = 0;
            NeatNueralNetwork.GlobalInnovationHashes.Clear();
            NeatNueralNetwork.GlobalInnovations = Array.Empty<IInnovation>();

            // when we split the connection the new node should have the same weight as the original connection, and the
            // original input node should have a weight of 1 to prevent the new topology from impacting the fittness too badly of the new
            // topolgy

            nn.Innovations[0].Enabled = true;
            Assert.Equal(AddNodeResult.success, nn.AddNode().Result);

            // make sure the innovation was properly recorded in the global innovations and the old connection was disabled
            Assert.False(nn.Innovations[0].Enabled);

            Assert.Equal(3, nn.Innovations.Length);

            // make sure that the hashes for the new innovations were recorded
            Assert.Contains(nn.Innovations[1].Hash(), nn.InnovationHashes);
            Assert.Contains(nn.Innovations[2].Hash(), nn.InnovationHashes);

            Assert.True(NeatNueralNetwork.GlobalInnovationHashes.ContainsKey(nn.Innovations[1].Hash()));
            Assert.True(NeatNueralNetwork.GlobalInnovationHashes.ContainsKey(nn.Innovations[2].Hash()));

            // make sure the node that was created was added to the genome
            Assert.Equal(6, nn.Nodes.Length);
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

            Assert.Equal(AddConnectionResult.alreadyExists, nn.AddConnection().Result);

            nn.InnovationHashes.Clear();

            Assert.Equal(AddConnectionResult.success, nn.AddConnection().Result);
        }

        [Fact]
        public void GeneratePhenotype()
        {
            var nn = new NeatNueralNetwork(1, 1);

            // create a connection between 0 and 1
            Assert.Equal(AddConnectionResult.success, nn.AddConnection().Result);

            // make sure the node ids were set correctly
            Assert.Equal(0, nn.Nodes[0].Id);
            Assert.Equal(1, nn.Nodes[1].Id);

            // make sure the connection was made correctly
            Assert.Equal(0, nn.Innovations[0].InputNode);
            Assert.Equal(1, nn.Innovations[0].OutputNode);

            // set weight to unlikely number
            nn.Innovations[0].Weight = 0.666652d;

            nn.GeneratePhenotype();

            // this innovation is a connection:
            // 0 --> 1
            // 0's output nodes should be this innovation and 1's input nodes should be this innovation
            Assert.Contains(nn.Innovations[0], nn.Nodes[0].OutputNodes);

            // manully add another connection to test that dict lookups and array reszing works
            nn.AddLocalAndGlobalInnovation(new Innovation
            {
                Enabled = true,
                Id = 1,
                InputNode = 0,
                OutputNode = 1
            }).Wait();

            nn.GeneratePhenotype();
        }
    }
}
