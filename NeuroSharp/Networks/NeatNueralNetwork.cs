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
    public class NeatNueralNetwork
    {
        /// <summary>
        /// The index of the next innovation
        /// </summary>
        internal static volatile int CurrentGlobalInnovationIndex = 0;

        /// <summary>
        /// The actual storage location for innovations
        /// </summary>
        internal static IInnovation[] GlobalInnovations = Array.Empty<IInnovation>();

        /// <summary>
        /// a O(n) lookup for the index of an innovation by its hash
        /// </summary>
        internal static readonly Dictionary<string, int> GlobalInnovationHashes = new();

        internal static readonly SemaphoreSlim InnovationSemaphore = new(1, 1);

        public NeatNueralNetwork(ushort InputNodes, ushort OutputNodes)
        {
            Nodes = new INeatNode[InputNodes + OutputNodes];

            NextNodeNumber = 0;

            Span<INeatNode> nodes = new(Nodes);
            for (ushort i = 0; i < InputNodes; i++)
            {
                nodes[i] = new Node
                {
                    Id = i,
                    NodeType = NodeType.Input
                };
                Interlocked.Increment(ref NextNodeNumber);
            }
            for (int i = 0; i < OutputNodes; i++)
            {
                nodes[i + InputNodes] = new Node
                {
                    Id = (ushort)(i + InputNodes),
                    NodeType = NodeType.Output
                };
                Interlocked.Increment(ref NextNodeNumber);
            }
        }

        private volatile int NextNodeNumber = 0;

        // a list would be more convenient but this is ~5x faster
        internal INeatNode[] _Nodes = Array.Empty<INeatNode>();

        public INeatNode[] Nodes
        {
            get { return _Nodes; }
            set { _Nodes = value; }
        }

        internal IInnovation[] _Innovations = Array.Empty<Innovation>();

        public IInnovation[] Innovations
        {
            get { return _Innovations; }
            set { _Innovations = value; }
        }


        /// <summary>
        /// The hashes of all the innovations that this network currently has in its genome
        /// </summary>
        internal readonly HashSet<string> InnovationHashes = new();

        /// <summary>
        /// Adds the given <see cref="IInnovation"/> to the global innovations dictionary.
        /// </summary>
        /// <param name="innovation"></param>
        /// <returns>
        /// <see langword="true"/> when the <see cref="IInnovation"/> was successfully added to the global dict.
        /// <para>
        /// <see langword="false"/> when the innovation already exists in the dictionary.
        /// </para>
        /// </returns>
        public static async Task<int> AddInnovation(IInnovation innovation)
        {
            // get the hash of the inn so we can see if it's already in the global list
            string hash = innovation.Hash();

            await InnovationSemaphore.WaitAsync();

            try
            {
                // check to see if the hash exists in the dict
                if (GlobalInnovationHashes.ContainsKey(hash) is false)
                {
                    int innovationNumber = System.Threading.Interlocked.Increment(ref CurrentGlobalInnovationIndex);

                    Array.Resize(ref GlobalInnovations, innovationNumber);

                    // add the actual innovation to the global list
                    GlobalInnovations[innovationNumber - 1] = innovation;

                    // create a O(1) lookup for the index of that innovation
                    GlobalInnovationHashes.Add(hash, innovationNumber);

                    return innovationNumber;
                }
                else
                {
                    // since the hash is in the dict return the index of it's innovation number
                    return GlobalInnovationHashes[hash];
                }
            }
            finally
            {
                InnovationSemaphore.Release();
            }
        }

        /// <summary>
        /// Creates the actual arrays from the genome that are used to evaluate the network and perform mutations
        /// </summary>
        /// <returns></returns>
        public void GeneratePhenotype()
        {
            // this method should read the genome and fill the arrays for the nodes
            Dictionary<ushort, IInnovation[]> Inputs = new();
            Dictionary<ushort, IInnovation[]> Outputs = new();

            // iterate through the innovations and create dictionaries for all connections

            // load the innovations into a contigous block of memory on the stack
            Span<IInnovation> inns = new(Innovations);

            foreach (var item in inns)
            {
                // store wheere this inoovation consumes from and outputs to
                if (Inputs.ContainsKey(item.InputNode))
                {
                    // add the item to the array, initially i used lists but this is 50% faster
                    var arr = Inputs[item.InputNode];

                    Array.Resize(ref arr, arr.Length + 1);

                    arr[^1] = item;

                    Inputs[item.InputNode] = arr;
                }
                else
                {
                    Inputs.Add(item.InputNode, new IInnovation[] { item });
                }

                // store where this node consumes and outputs to
                if (Outputs.ContainsKey(item.OutputNode))
                {
                    // add the item to the array, initially i used lists but this is 50% faster
                    var arr = Outputs[item.OutputNode];

                    Array.Resize(ref arr, arr.Length + 1);

                    arr[^1] = item;

                    Outputs[item.InputNode] = arr;
                }
                else
                {
                    Outputs.Add(item.OutputNode, new IInnovation[] { item });
                }
            }

            // create a contigous block of memory to store the nodes for faster access
            Span<INeatNode> nodes = new(Nodes);

            // iterate through the nodes and create the arrays that contain the connections using the dictionaries we just built
            foreach (var node in nodes)
            {
                // this is for the input nodes, they do NOT input from anywhere, their inputs are the provided inputs for an evaluation, dont waste the time searching a dict if we dont have to

                // check to see if we need to create an array for input nodes
                // we may not need to if this node is an input for example
                if (node.NodeType != NodeType.Output && Inputs.ContainsKey(node.Id))
                {
                    Span<IInnovation> inputs = new(Inputs[node.Id]);

                    node.OutputNodes = new IInnovation[inputs.Length];

                    Span<IInnovation> nodeSpan = new(node.OutputNodes);

                    inputs.CopyTo(nodeSpan);
                }

                // this is for the output nodes, they do NOT output to anything, their result is the file eval of the inputs therefor we dont need to
                // search the dictionary for where these nodes ouput to

                // check to see if we need to create an array for output nodes
                // we may not need to if this node is an input for example
                if (node.NodeType != NodeType.Input && Outputs.ContainsKey(node.Id))
                {
                    Span<IInnovation> outputs = new(Outputs[node.Id]);

                    node.InputNodes = new IInnovation[outputs.Length];

                    Span<IInnovation> nodeSpan = new(node.InputNodes);

                    outputs.CopyTo(nodeSpan);
                }
            }
        }

        public async Task Mutate()
        {
            /*  when we mutate we can only do the following:
                Add Node
                Add Connection Between Nodes
                Disable Connection Between Nodes

            */

            // add a node
            // we can only add hidden nodes, since our inputs and outputs are pre-determined by the end-user
            // add a node to our genome

        }

        public async Task<AddConnectionResult> AddConnection()
        {
            // add a connection between two nodes that did not previously exist

            INeatNode[] EligibleInputNodes;
            INeatNode[] EligibleOutputNodes;

            (EligibleInputNodes, EligibleOutputNodes) = GetEligibleNodesForNewConnection();

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

            // make sure the innovation is not already in our list if its not add it
            if (InnovationHashes.Add(newInnovation.Hash()) is false)
            {
                // since it's already int the list instead of mutating give up 
                return AddConnectionResult.alreadyExists;
            }

            await AddLocalAndGlobalInnovation(newInnovation);

            return AddConnectionResult.success;
        }

        public async Task<AddNodeResult> AddNode()
        {
            // chose two random nodes to connect
            // the input can either be an input or a hidden
            // and the output can either be another hidden or an output

            // make sure we can split a connection, if there are none, return and give up
            if (Innovations.Length is 0)
            {
                return AddNodeResult.noEligibleConnections;
            }

            // make sure there is a connection that we can split



            IInnovation[] eligibleConnections;

            if (TryGetEligibleConnectionToSplit(out eligibleConnections) is false)
            {
                return AddNodeResult.noEligibleConnections;
            }

            // get a random eligible connection
            IInnovation connectionToSplit = eligibleConnections[await Helpers.NextAsync(0, eligibleConnections.Length)];

            // create the new node
            INeatNode newNode = new Node()
            {
                Id = (ushort)Interlocked.Increment(ref NextNodeNumber),
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
            if (InnovationHashes.Contains(inputConnection.Hash()) || InnovationHashes.Contains(outputConnecion.Hash()))
            {
                // make sure we decrement the node counter that we incremented earlier when creating the node before we could check to see if this innovation was already made
                Interlocked.Decrement(ref NextNodeNumber);

                return AddNodeResult.alreadyExists;
            }

            // check to see if there is a connection that bypasses the new node disable is
            // ex: input -------------> output <- should be disabled since it's being replaced with input -> new node -> output
            foreach (var item in Innovations)
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
            await AddLocalAndGlobalInnovation(inputConnection);
            await AddLocalAndGlobalInnovation(outputConnecion);

            InnovationHashes.Add(inputConnection.Hash());
            InnovationHashes.Add(outputConnecion.Hash());

            // add the node
            AddNodeToGenome(newNode);

            return AddNodeResult.success;
        }

        /// <summary>
        /// Adds the given node to this local instances genome, automatically resizes the array and inserts it to the last index;
        /// </summary>
        /// <param name="node"></param>
        internal void AddNodeToGenome(INeatNode node)
        {
            Array.Resize(ref _Nodes, _Nodes.Length + 1);

            Nodes[^1] = node;
        }

        internal async Task<bool> AddLocalAndGlobalInnovation(IInnovation innovation)
        {
            // set the innovation id of the new innovation, if the new gene is unique we will get a new number from the static method, if it already exists we will get its existing innovation number
            // also add it to the global genome
            innovation.Id = await AddInnovation(innovation);

            // resize the backing array for the innovations to accomodate the new innovation
            Array.Resize(ref _Innovations, _Innovations.Length + 1);

            Innovations[^1] = innovation;

            return true;
        }


        internal bool TryGetEligibleConnectionToSplit(out IInnovation[] eligibleInnovations)
        {
            /*
                an eligible connection has the following characteristics:
                
                - enabled
                - a valid input node
                - a valid output node
            */
            if (Innovations.Length is 0)
            {
                eligibleInnovations = Array.Empty<Innovation>();
                return false;
            }

            HashSet<IInnovation> eligibleInns = new();

            Span<IInnovation> innovations = new(Innovations);

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

        internal (INeatNode[] EligibleInputNodes, INeatNode[] EligibleOutputNodes) GetEligibleNodesForNewConnection()
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

            Span<INeatNode> nodes = new(Nodes);

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
    }
}
