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
        public double WeightReassignmentChance { get; set; } = 0.1d;

        public double MutateWeightChance { get; set; } = 0.9d;

        public double AddConnectionChance { get; set; } = 0.05d;

        public double AddNodeChance { get; set; } = 0.025d;

        public double WeightMutationModifier { get; set; } = 1.0d;

        public virtual async Task<InitializeNetworkResult> InitializeConnections(INeatNetwork network)
        {
            // first get an array for the new weights for the new connections
            // since this is the default mutater we will use random weights
            // get a array of new weights for the network
            // by default all inputs should be connected to all outputs
            int numberOfConnections = network.InputNodes * network.OutputNodes;

            double[] rolls = await Helpers.NextDoubleArray(numberOfConnections);

            // reset the network since we are starting a new one
            network.Reset();

            // create placeholders to avoid GCC
            string innHash;
            IInnovation newInn;
            int rollIndex = 0;
            for (int i = 0; i < network.InputNodes; i++)
            {
                for (int x = network.InputNodes; x < network.InputNodes + network.OutputNodes; x++)
                {
                    newInn = new Innovation()
                    {
                        Enabled = true,
                        InputNode = (ushort)i,
                        OutputNode = (ushort)x,
                        Weight = rolls[rollIndex++]
                    };

                    innHash = newInn.Hash();

                    if (network.InnovationHashes.Add(innHash))
                    {
                        await network.AddInnovation(newInn);
                    }
                }
            }

            network.GeneratePhenotype();

            return InitializeNetworkResult.success;
        }

        internal delegate void WeightModification(ref IInnovation innovation, ref double roll);

        public virtual async Task<MutationResult> Mutate(INeatNetwork network)
        {
            double val = await Helpers.NextUDoubleAsync();

            MutationResult result = MutationResult.success;

            bool Between(double value, double lower, double upper)
            {
                return value <= upper && value >= lower;
            }

            bool addNode;
            bool addConnection;

            if (AddConnectionChance > AddNodeChance)
            {
                // 0                                                                            1
                // |- Add Node -|---- Add Connection ----|-------------- Roll Fail -------------|
                addNode = Between(val, 0, AddNodeChance);
                addConnection = Between(val, AddNodeChance, AddNodeChance + AddConnectionChance);
            }
            else
            {
                // 0                                                                            1
                // |- Add Connection -|---- Add Node ----|-------------- Roll Fail -------------|
                addNode = Between(val, 0, AddConnectionChance);
                addConnection = Between(val, AddConnectionChance, AddNodeChance + AddConnectionChance);
            }

            if (addNode)
            {
                var status = await AddNode(network);
                // cast the add node result to a mutation result
                result = status switch
                {
                    AddNodeResult.success => MutationResult.success,
                    AddNodeResult.error => MutationResult.error,
                    AddNodeResult.noEligibleConnections => MutationResult.noValidMutations,
                    AddNodeResult.alreadyExists => MutationResult.noValidMutations,
                    _ => MutationResult.error,
                };
            }
            else if (addConnection)
            {
                var status = await AddConnection(network);
                // cast the add connection result to a mutation result
                result = status switch
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

            return result;
        }

        public async Task<MutationResult> MutateWeights(INeatNetwork network)
        {
            // make sure we don't accidentally try to modify an array that is invalid
            if (network?.Innovations?.Length is null or 0)
            {
                return MutationResult.noValidMutations;
            }

            // get the rolls all at once instead of in a loop since accessing the asynchronous RNG is expensive
            double[] rolls = await Helpers.NextUDoubleArray(network.Innovations.Length);
            double[] weights = await Helpers.NextDoubleArray(network.Innovations.Length);

            void _Mutate(double[] rolls, double[] weights)
            {
                bool reassignmentLessThanMutate = WeightReassignmentChance <= MutateWeightChance;

                double LowVal = reassignmentLessThanMutate ? WeightReassignmentChance : MutateWeightChance;

                double HighVal = reassignmentLessThanMutate ? MutateWeightChance : WeightReassignmentChance;

                WeightModification lowRoll;
                WeightModification highRoll;

                void ReassignValue(ref IInnovation val, ref double newVal)
                {
                    val.Weight = newVal;
                }

                void ModifyValue(ref IInnovation val, ref double roll)
                {
                    val.Weight += roll * WeightMutationModifier;
                }

                if (reassignmentLessThanMutate)
                {
                    lowRoll = ReassignValue;
                    highRoll = ModifyValue;
                }
                else
                {
                    lowRoll = ModifyValue;
                    highRoll = ReassignValue;
                }

                // carve out contigous memory for the innovations and rolls for faster access(there may be hundreds of entries we may need to modify)
                Span<IInnovation> innSpan = new(network.Innovations);

                Span<double> rollSpan = new(rolls);
                Span<double> weightSpan = new(weights);

                for (int i = 0; i < network.Innovations.Length; i++)
                {
                    // roll to see if we modify the value at all
                    ref double val = ref rollSpan[i];

                    // 0                                                           1
                    // |---- Low Roll ----|------- High Roll -------|--  No Roll --|
                    if (val <= LowVal)
                    {
                        lowRoll(ref innSpan[i], ref weightSpan[i]);
                    }
                    else if (val > LowVal && val <= HighVal)
                    {
                        highRoll(ref innSpan[i], ref weightSpan[i]);
                    }
                }
            }

            // do the actual mutation on the stack since this is async and is contained within a state machine and can't process contiguous blocks of memory unless we pin the memory manually(and i dont feel like doing that)
            _Mutate(rolls, weights);

            return MutationResult.success;
        }

        public async Task<AddConnectionResult> AddConnection(INeatNetwork network)
        {
            // make sure we have nodes we can connection connections to
            if (network?.Innovations?.Length is null or 0)
            {
                return AddConnectionResult.noEligibleNodes;
            }

            // check to see that we have generated the layers yet for the network
            if (network.TopologyChanged)
            {
                network.GeneratePhenotype();
            }

            // add a connection between two nodes that did not previously exist

            // get a random layer from the network
            // remember that the output layer is the 0th layer and we can't add a connection originating from an output node, that would create a circular reference and that's bad mmkay

            int startLayer = await Helpers.NextAsync(1, network.NodeLayers.Length);

            // default the target layer as the output layer becuase we know that prevents circular references
            int targetLayer = 0;

            // out int n n n 
            // 0    1  2 3 4

            // determine a random target layer
            if (startLayer < network.NodeLayers.Length - 1)
            {
                // if the start layer isn't the last hidden layer that means there is a possibility that we can add a connection between that layer and either another hidden layer or the output layer
                // it should be noted that this random roll is not inclusive of the upper bounds of the provided value
                // therefor, although it may appear that the modulo would implement wrapping to the 1st element of the layers it actually wraps to the 0th layer(the output layer) since it's not inclusive, hence the + 1 instead of just using .Length(what i would normally use to wrap the the 0th element)
                targetLayer = await Helpers.NextAsync(startLayer + 1, network.NodeLayers.Length + 1) % network.NodeLayers.Length;
            }

            // now that we have the start and end layers we should choose random nodes from each of those layers
            int startNodeIndex = await Helpers.NextAsync(0, network.NodeLayers[startLayer].Length);
            int endNodeIndex = await Helpers.NextAsync(0, network.NodeLayers[targetLayer].Length);

            ushort startNode = (ushort)network.NodeLayers[startLayer][startNodeIndex];
            ushort endNode = (ushort)network.NodeLayers[targetLayer][endNodeIndex];

            // create the innovation and get it's hash to see if that innovation is already in our genome
            IInnovation newInnovation = new Innovation()
            {
                Enabled = true,
                InputNode = startNode,
                OutputNode = endNode,
                Weight = await Helpers.NextDoubleAsync()
            };

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


            IInnovation[] eligibleConnections;

            if (TryGetEligibleConnectionToSplit(out eligibleConnections, network) is false)
            {
                return AddNodeResult.noEligibleConnections;
            }

            // get a random eligible connection
            IInnovation connectionToSplit = eligibleConnections[await Helpers.NextAsync(0, eligibleConnections.Length)];

            // get the nodes id
            ushort newNodeId = network.IncrementNextNodeId();

            // create the connection between the input and the new node
            // input --> new node --> output
            IInnovation inputConnection = new Innovation()
            {
                InputNode = connectionToSplit.InputNode,
                OutputNode = newNodeId,
                Enabled = true,
                // weight is required to be 1.0 per spec
                Weight = 1.0d
            };

            // create the connection between the new node and the output
            // input --> new node --> output
            IInnovation outputConnecion = new Innovation()
            {
                InputNode = newNodeId,
                OutputNode = connectionToSplit.OutputNode,
                Enabled = true,
                Weight = connectionToSplit.Weight
            };

            // hash both the new connections so we can verify that they don't exist already
            string inputHash = inputConnection.Hash();
            string outputHash = outputConnecion.Hash();

            // make sure the innovation isn't already in our innovations
            if (network.InnovationHashes.Contains(inputHash) || network.InnovationHashes.Contains(outputHash))
            {
                // make sure we decrement the node counter that we incremented earlier when creating the node before we could check to see if this innovation was already made
                network.DecrementNextNodeId();

                return AddNodeResult.alreadyExists;
            }

            // check to see if there is a connection that bypasses the new node disable is
            // ex: input -------------> output <- should be disabled since it's being replaced with input -> new node -> output
            DisableConnections(connectionToSplit.Id, network.Innovations);

            // try to get the innovation id for the innovation and store it
            await network.AddInnovation(inputConnection);
            await network.AddInnovation(outputConnecion);

            // make sure we store the hashes so we cant create the same connections again
            network.InnovationHashes.Add(inputHash);
            network.InnovationHashes.Add(outputHash);

            return AddNodeResult.success;
        }

        internal static void DisableConnections(int id, IInnovation[] innovations)
        {
            Span<IInnovation> inns = new(innovations);
            for (int i = 0; i < inns.Length; i++)
            {
                ref IInnovation tmp = ref inns[i];
                if (tmp.Id == id)
                {
                    tmp.Enabled = false;
                    break;
                }
            }
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
    }
}
