using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

[assembly: InternalsVisibleTo("NeuroSharp.Tests")]
namespace NeuroSharp.NEAT
{
    public class DefaultMutater : IMutater
    {
        public double MutateWeightChance { get; set; } = 0.125d;

        public double AddConnectionChance { get; set; } = 0.05d;

        public double AddNodeChance { get; set; } = 0.025d;

        public async Task<MutationResult> Mutate(INeatNetwork network)
        {
            double val = await Helpers.NextUDoubleAsync();

            if (val <= AddNodeChance && val >= AddConnectionChance)
            {
                var status = await AddNode(network);
                return status switch
                {
                    AddNodeResult.success => MutationResult.success,
                    AddNodeResult.error => MutationResult.error,
                    AddNodeResult.noEligibleConnections => MutationResult.noValidMutations,
                    AddNodeResult.alreadyExists => MutationResult.noValidMutations,
                    _ => MutationResult.error,
                };
            }
            else if (val <= AddConnectionChance && val <= AddNodeChance)
            {
                var status = await AddConnection(network);
                return status switch
                {
                    AddConnectionResult.success => MutationResult.success,
                    AddConnectionResult.error => MutationResult.error,
                    AddConnectionResult.noEligibleNodes => MutationResult.noValidMutations,
                    AddConnectionResult.alreadyExists => MutationResult.noValidMutations,
                    _ => MutationResult.error,
                };
            }


            // mutate weights
            await MutateWeights(network);

            return MutationResult.success;
        }

        public async Task<MutationResult> MutateWeights(INeatNetwork network)
        {
            if (network?.Innovations?.Length is null or 0)
            {
                return MutationResult.noValidMutations;
            }

            for (int i = 0; i < network.Innovations.Length; i++)
            {
                double val = await Helpers.NextUDoubleAsync();
                if (val <= AddNodeChance)
                {
                    network.Innovations[i].Weight += await Helpers.NextDoubleAsync();
                }
            }

            return MutationResult.success;
        }

        public async Task<AddConnectionResult> AddConnection(INeatNetwork network)
        {
            // add a connection between two nodes that did not previously exist

            INeatNode[] EligibleInputNodes;
            INeatNode[] EligibleOutputNodes;

            (EligibleInputNodes, EligibleOutputNodes) = GetEligibleNodesForNewConnection(network);

            async Task<INeatNode> GetInputNode() => EligibleInputNodes[await Helpers.NextAsync(0, EligibleInputNodes.Length)];
            async Task<INeatNode> GetOutputNode() => EligibleOutputNodes[await Helpers.NextAsync(0, EligibleOutputNodes.Length)];

            var inputNode = await GetInputNode();
            var outputNode = await GetOutputNode();

            // make sure we didn't select the same input and output node(only possible with hidden nodes)
            // i tried to test for this but i couldn't be bother to spend more than 20 minutes on it, at least if it does show it its covered, else its just a single bool check, not to big of a deal on performance
            if (inputNode.Id == outputNode.Id)
            {
                // check to see if that was the only node available
                int maxRetries = 10;
                do
                {
                    maxRetries--;

                    inputNode = await GetInputNode();
                    outputNode = await GetOutputNode();

                    if (maxRetries <= 0)
                    {
                        return AddConnectionResult.noEligibleNodes;
                    }
                } while (inputNode.Id == outputNode.Id);
            }

            // create the innovation and get it's hash to see if that innovation is already in our genome
            var newInnovation = new Innovation()
            {
                Enabled = true,
                InputNode = inputNode.Id,
                OutputNode = outputNode.Id,
                Weight = await Helpers.NextDoubleAsync()
            };

            // make sure we dont accidently create a circular connection
            //if (VerifyNoCircularConnectionExists(newInnovation, network))
            //{
            //    return AddConnectionResult.circularConnection;
            //}


            // make sure the innovation is not already in our list if its not add it
            if (network.InnovationHashes.Add(newInnovation.Hash()) is false)
            {
                // since it's already int the list instead of mutating give up 
                return AddConnectionResult.alreadyExists;
            }

            await network.AddInnovation(newInnovation);

            return AddConnectionResult.success;
        }

        public async Task<AddNodeResult> AddNode(INeatNetwork network)
        {
            // chose two random nodes to connect
            // the input can either be an input or a hidden
            // and the output can either be another hidden or an output

            // make sure we can split a connection, if there are none, return and give up
            if (network.Innovations.Length is 0)
            {
                return AddNodeResult.noEligibleConnections;
            }

            // make sure there is a connection that we can split



            IInnovation[] eligibleConnections;

            if (TryGetEligibleConnectionToSplit(out eligibleConnections, network) is false)
            {
                return AddNodeResult.noEligibleConnections;
            }

            // get a random eligible connection
            IInnovation connectionToSplit = eligibleConnections[await Helpers.NextAsync(0, eligibleConnections.Length)];

            // create the new node
            INeatNode newNode = new Node()
            {
                Id = network.IncrementNodeCount(),
                NodeType = NodeType.Hidden
            };

            // create the connection between the input and the new node
            // input --> new node --> output
            IInnovation inputConnection = new Innovation()
            {
                InputNode = connectionToSplit.InputNode,
                OutputNode = newNode.Id,
                Enabled = true,
                Weight = 1.0d
            };

            // create the connection between the new node and the output
            // input --> new node --> output
            IInnovation outputConnecion = new Innovation()
            {
                InputNode = newNode.Id,
                OutputNode = connectionToSplit.OutputNode,
                Enabled = true,
                Weight = connectionToSplit.Weight
            };

            // make sure the innovation isn't already in our innovations
            if (network.InnovationHashes.Contains(inputConnection.Hash()) || network.InnovationHashes.Contains(outputConnecion.Hash()))
            {
                // make sure we decrement the node counter that we incremented earlier when creating the node before we could check to see if this innovation was already made
                network.DecrementNodeCount();

                return AddNodeResult.alreadyExists;
            }

            // check to see if there is a connection that bypasses the new node disable is
            // ex: input -------------> output <- should be disabled since it's being replaced with input -> new node -> output
            foreach (var item in network.Innovations)
            {
                // we should avoid using full equality comparison here since this is faster and the Ids should be global innovation numbers and therefor unique
                if (item.Id == connectionToSplit.Id)
                {
                    // if the node that we are using as an input it connected the the node we are using as an output, we should disable that connection since it will bypass our new node
                    item.Enabled = false;

                    // since there should only be one break and continue
                    break;
                }
            }

            // since the 

            // try to get the innovation id for the innovation and store it
            await network.AddInnovation(inputConnection);
            await network.AddInnovation(outputConnecion);

            network.InnovationHashes.Add(inputConnection.Hash());
            network.InnovationHashes.Add(outputConnecion.Hash());

            // add the node
            network.AddNode(newNode);

            return AddNodeResult.success;
        }

        internal bool TryGetEligibleConnectionToSplit(out IInnovation[] eligibleInnovations, INeatNetwork network)
        {
            /*
                an eligible connection has the following characteristics:
                
                - enabled
                - a valid input node
                - a valid output node
            */
            if (network.Innovations.Length is 0)
            {
                eligibleInnovations = Array.Empty<Innovation>();
                return false;
            }

            HashSet<IInnovation> eligibleInns = new();

            Span<IInnovation> innovations = new(network.Innovations);

            foreach (var item in innovations)
            {
                if (item.Enabled)
                {
                    eligibleInns.Add(item);
                }
            }

            if (eligibleInns.Count is 0)
            {
                eligibleInnovations = Array.Empty<Innovation>();
                return false;
            }

            eligibleInnovations = eligibleInns.ToArray();
            return true;
        }

        internal (INeatNode[] EligibleInputNodes, INeatNode[] EligibleOutputNodes) GetEligibleNodesForNewConnection(INeatNetwork network)
        {
            // we should only connect two nodes if the following rules are met:
            /*
                the input node is not an output node:
                    output nodes are the final result and are pre-determined we shouldn't add more arbitrarily

                the output node is not an input node:
                    input nodes are what accepts incoming data, we should not create feedback loops by feeding results into them except the initial training data

                neither node is the same node:
                    we shouldnt create a connection between a node and itself
            */

            HashSet<INeatNode> EligibleInputNodes = new();
            HashSet<INeatNode> EligibleOutputNodes = new();

            Span<INeatNode> nodes = new(network.Nodes);

            foreach (var item in nodes)
            {
                switch (item.NodeType)
                {
                    case NodeType.Input:
                        EligibleInputNodes.Add(item);
                        break;
                    case NodeType.Hidden:
                        EligibleInputNodes.Add(item);
                        EligibleOutputNodes.Add(item);
                        break;
                    case NodeType.Output:
                        EligibleOutputNodes.Add(item);
                        break;
                }
            }

            return (EligibleInputNodes.ToArray(), EligibleOutputNodes.ToArray());
        }

        internal bool VerifyNoCircularConnectionExists(IInnovation innovation, INeatNetwork network)
        {
            // verify that the output node of the provided innovation does not lead back to the same innovation(a circular reference)

            // we use the stuff built by the phenotype to perform recursive checks, revbuild it if it's dirty
            if (network.TopologyChanged)
            {
                network.GeneratePhenotype();
            }

            bool ContainsBadId(IInnovation inn, ushort badId)
            {
                if (network.TryGetIndex(innovation.OutputNode, out int index))
                {
                    if (network.Nodes[index].OutputNodes?.Length is null or 0)
                    {
                        return false;
                    }
                    if (network.Nodes[index].OutputNodes.Where(x => x.Id == badId).Any())
                    {
                        return true;
                    }
                    // now that we have the index of the recursively check all the nodes it outputs to and verify none of the paths lead to this node
                    foreach (var item in network.Nodes[index].OutputNodes)
                    {
                        if (ContainsBadId(item, innovation.InputNode))
                        {
                            return true;
                        }
                    }
                    return false;
                }
                else
                {
                    return false;
                }
            }

            return ContainsBadId(innovation, innovation.OutputNode);
        }
    }
}
