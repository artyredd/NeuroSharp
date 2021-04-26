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
    public class NeatNueralNetwork : INeatNetwork, IInstantiableNetwork<INeatNetwork>
    {
        public NeatNueralNetwork(ushort InputNodes, ushort OutputNodes)
        {
            InputNodeCount = InputNodes;
            OutputNodeCount = OutputNodes;

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

        public NeatNueralNetwork()
        {

        }

        public static IInnovationHandler GlobalInnovations { get; } = new InnovationHandler();

        /// <summary>
        /// Gets the <see cref="IMutater"/> that provides functionality to how this NEAT mutates during runtime.
        /// </summary>
        public IMutater Mutater { get; init; } = new DefaultMutater();

        /// <summary>
        /// Gets the <see cref="IEvaluator"/> that should propogate values through this NEAT network and evaluate the output from this NEAT network. This is different from a <see cref="NeuroSharp.Propagation.IPropogator{T}"/>. NEAT Networks use <see cref="IEvaluator"/>s instead.
        /// </summary>
        public IEvaluator<double, double[], double> Evaluator { get; init; } = new DefaultEvaluator();

        public IPhenotypeGenerator<double> PhenotypeGenerator { get; init; } = new DefaultPhenotypeGenerator<double>();

        /// <summary>
        /// The nodes that this network has evolved or started with.
        /// <code>
        /// Order of elements:
        /// </code>
        /// Nodes:
        /// [ <c>input nodes</c>  ... | <c>output nodes</c> ... | <c>hidden nodes</c> ... ]
        /// </summary>
        public INeatNode[] Nodes
        {
            get { return _Nodes; }
            set { _Nodes = value; }
        }

        internal Dictionary<ushort, int> NodeIndexDictionary = new();

        internal SemaphoreSlim NodeIndexSemaphore = new(1, 1);

        internal IInnovation[] _Innovations = Array.Empty<Innovation>();

        /// <summary>
        /// The innovations that this network has evolved
        /// </summary>
        public IInnovation[] Innovations
        {
            get { return _Innovations; }
            set { _Innovations = value; }
        }

        /// <summary>
        /// The number of input nodes this network was created with
        /// </summary>
        public int InputNodeCount { get; private set; } = 0;

        /// <summary>
        /// The number of ouput nodes this network was created with
        /// </summary>
        public int OutputNodeCount { get; private set; } = 0;

        // a list would be more convenient but this is ~5x faster
        internal INeatNode[] _Nodes = Array.Empty<INeatNode>();

        internal volatile int NextNodeNumber = 0;

        internal volatile bool Dirty = true;

        public bool TopologyChanged => Dirty;

        /// <summary>
        /// The hashes of all the innovations that this network currently has in its genome
        /// </summary>
        public HashSet<string> InnovationHashes { get; init; } = new();

        public INeatNetwork Create(int InputNodes, int OutputNodes) => CreateAsync(InputNodes, OutputNodes).Result;

        public async Task<INeatNetwork> CreateAsync(int InputNodes, int OutputNodes)
        {
            return new NeatNueralNetwork((ushort)InputNodes, (ushort)OutputNodes);
        }

        public bool TryGetIndex(ushort NodeId, out int index)
        {
            NodeIndexSemaphore.Wait();
            try
            {
                if (NodeIndexDictionary.ContainsKey(NodeId))
                {
                    index = NodeIndexDictionary[NodeId];
                    return true;
                }
                index = default;
                return false;
            }
            finally
            {
                NodeIndexSemaphore.Release();
            }
        }

        /// <summary>
        /// Creates the actual arrays from the genome that are used to evaluate the network and perform mutations
        /// <code>
        /// Complexity: Innovations = O(2n)  Nodes = O(n)
        /// </code>
        /// </summary>
        /// <returns></returns>
        //public void GeneratePhenotype()
        //{
        //    // avoid unecessary and very performance costly genotype generation
        //    if (Dirty is false)
        //    {
        //        return;
        //    }

        //    // mark ourselfs as having regenerated our structure
        //    Dirty = false;

        //    // we should generate the phenotype from innovations alone

        //    /*
        //        Innovation scheme:

        //          innovation id(#)
        //            from -> to
        //              Weight
        //             ENABLED

        //    */
        //    // Key = the id of the node that sends to int[] nodes
        //    Dictionary<int, int[]> Senders = new();

        //    // Key = the id of the node that receives from int[] nodes
        //    Dictionary<int, int[]> Recipients = new();

        //    // Key = the id of the node, Value = the int layer the node is in
        //    // 0 = output layer
        //    // 1 = input layer
        //    // > 1 Hidden node layers
        //    Dictionary<int, int> LayerDict = new();

        //    // the value the id of a node must be less than to be either an input or output node
        //    int mustBeInputOrOutputThreashold = InputNodeCount + OutputNodeCount;

        //    void AddRecipientToSender(int node, int recipient) => Senders.AddValueToArray(node, recipient);

        //    void AddSenderToRecipient(int node, int sender) => Recipients.AddValueToArray(node, sender);

        //    void SetLayer(int node, int layer)
        //    {
        //        if (LayerDict.ContainsKey(node) is false)
        //        {
        //            LayerDict.Add(node, layer);
        //            return;
        //        }
        //        LayerDict[node] = layer;
        //    }

        //    // keep track of which nodes need further computation to figure out their exact layer, so we can avoid unessesary complexity
        //    int[] hiddenNodes = Array.Empty<int>();
        //    NodeType GetAndStoreNodeType(int nodeId)
        //    {
        //        if (nodeId < mustBeInputOrOutputThreashold)
        //        {
        //            // check to see if it's an output node
        //            // node array scheme is always input, output, hidden ...
        //            // layer scheme is 0= outputm 1 = input 1+ hidden
        //            if (nodeId >= InputNodeCount)
        //            {
        //                // must be output node
        //                SetLayer(nodeId, 0);
        //                return NodeType.Output;
        //            }
        //            else
        //            {
        //                // must be input node
        //                SetLayer(nodeId, 1);
        //                return NodeType.Input;
        //            }
        //        }

        //        // add to the hiddenNodes array so we can figure out it's actual layer later
        //        Extensions.Array.ResizeAndAdd(ref hiddenNodes, nodeId);

        //        return NodeType.Hidden;
        //    }

        //    // iterate through the innovations list and record where every node sends to and every node receives from
        //    Span<IInnovation> innovations = new(_Innovations);

        //    // ~ O(n)
        //    for (int i = 0; i < innovations.Length; i++)
        //    {
        //        ref IInnovation inn = ref innovations[i];

        //        // only enabled genes show up in the phenotype
        //        if (inn.Enabled is false)
        //        {
        //            continue;
        //        }

        //        // record where the 'from' node sends to
        //        AddRecipientToSender(inn.InputNode, inn.OutputNode);

        //        // records where the 'to' receives from
        //        AddSenderToRecipient(inn.OutputNode, inn.InputNode);

        //        // determine if the node is an input, output, or hidden node, store that value
        //        GetAndStoreNodeType(inn.InputNode);
        //        GetAndStoreNodeType(inn.OutputNode);
        //    }

        //    int GetAndSetHiddenLayer(int hiddenNodeId)
        //    {
        //        // first check to see if we have already calculated the layer
        //        if (LayerDict.ContainsKey(hiddenNodeId))
        //        {
        //            return LayerDict[hiddenNodeId];
        //        }
        //        // rules for finding the exact layer for the node
        //        /*
        //            if all of the nodes it receives from are input nodes, it's layer is 2 (1 above the input layer)
        //            if ANY od the nodes it receives from are hidden nodes, it's layer is above 2
        //                -> if child has an entry the parent hidden node's layer is the child nodes layer + 1
        //                -> if child doesn't have an entry, recurse until a all nodes received from are input nodes
        //        */

        //        // i could do a check to make sure the key exists, but it SHOULD exist by now, and if it doesn't it should throw an error because something is wrong
        //        Span<int> nodesReceivedFrom = new(Recipients[hiddenNodeId]);

        //        // this represents what layer all of the children are in, when this is layer 2(1 above input layer) the the highest layer is 1 and our layer is that + 1
        //        // when any of our children are hidden we should get their height in the network recursively as well and record their height here. We will always be +1 to our highest child in the network
        //        int HighestLayerOfChildren = 1;

        //        // iterate through all fo the nodes that provide to this node and check if they are all input nodes
        //        for (int i = 0; i < nodesReceivedFrom.Length; i++)
        //        {
        //            ref int id = ref nodesReceivedFrom[i];

        //            // check to see if we have already found the layer of the child node
        //            if (LayerDict.ContainsKey(id))
        //            {
        //                // if the layer dict contains the key it means the id is either a input or an output node, or a hidden node that has already recursively found it's layer, make sure its not 0, since that would be an output node and that would be a circular reference and break everything
        //                if (LayerDict[id] >= 1)
        //                {
        //                    // since this is a hidden node we should make sure we keep track of it's height since our height will be the highest child + 1
        //                    if (LayerDict[id] > HighestLayerOfChildren)
        //                    {
        //                        // since it's larger set our largest child to that + 1
        //                        HighestLayerOfChildren = LayerDict[id];
        //                    }
        //                }
        //                else
        //                {
        //                    int id_ByValue = id;
        //                    // a node should not be able to 'receieve input' from an output node (in this context at least) - for cyclical DAGs and networks
        //                    // they should implement a cycle that is not explicitly represented in the innovation genome. idk tho i never went to college
        //                    throw Networks.Exceptions.CircularInnovationReference(Innovations.Where(x => x.Id == id_ByValue).FirstOrDefault());
        //                }
        //            }
        //            else
        //            {
        //                // if we werent able to find the node in the dict we should recursively check to see if all their children are inputs, and if they are
        //                // our layer would be 1 above theirs(or the highest child in our tree)
        //                int val = GetAndSetHiddenLayer(id);

        //                // check to see if they are taller than all the rest of our children
        //                if (val > HighestLayerOfChildren)
        //                {
        //                    HighestLayerOfChildren = val;
        //                }
        //            }
        //        }

        //        // at this point either all of the nodes we receive from were input nodes (HighestLayer of 1) or we found the tallest child
        //        // our height is always + 1 to the tallest node we recieve from

        //        // set our layer in the dictionary

        //        HighestLayerOfChildren += 1;

        //        SetLayer(hiddenNodeId, HighestLayerOfChildren);

        //        // return so we recurse properly
        //        return HighestLayerOfChildren;
        //    }

        //    // O(i * n)
        //    // go through the hidden and find their exact layer
        //    Span<int> hiddenSpan = new(hiddenNodes);
        //    for (int i = 0; i < hiddenSpan.Length; i++)
        //    {
        //        GetAndSetHiddenLayer(hiddenSpan[i]);
        //    }

        //    // now that we have what our input nodes, output nodes, and hidden nodes are and their exact location we can construct the matrix to represent the data
        //}

        public void GeneratePhenotype() => PhenotypeGenerator.Generate(this);

        public ushort IncrementNodeCount() => (ushort)Interlocked.Increment(ref NextNodeNumber);
        public ushort DecrementNodeCount() => (ushort)Interlocked.Decrement(ref NextNodeNumber);

        public async Task<MutationResult> Mutate() => await Mutater.Mutate(this);

        /// <summary>
        /// Evaluates the provided data using the <see cref="Evaluator"/> and returns the results
        /// <code>
        /// Complexit: O(??) probably O(n²)
        /// </code>
        /// <code>
        /// ! Recursive !
        /// </code>
        /// </summary>
        /// <param name="Data"></param>
        /// <returns></returns>
        public double[] Evaluate(double[] Data)
        {
            VerifyNetworkBeforeEvaluation(ref Data);

            // this is by default a recursive function, may need a refactor for performance reasons (if there are any)

            return Evaluator.Evaluate(Data, this);
        }

        /// <summary>
        /// Evaluates the data, runs the result through the <see cref="IFitnessFunction{U, T}"/> in the <see cref="Evaluator"/> and returns the result.
        /// </summary>
        /// <param name="Data"></param>
        /// <returns></returns>
        public double EvaluateWithFitness(double[] Data)
        {
            VerifyNetworkBeforeEvaluation(ref Data);

            return Evaluator.EvaluateWithFitness(Data, this);
        }

        internal void VerifyNetworkBeforeEvaluation(ref double[] Data)
        {
            if (Data?.Length is null or 0 || Data.Length != InputNodeCount)
            {
                throw Networks.Exceptions.InconsistentTrainingDataScheme(InputNodeCount, Data.Length);
            }

            // make sure out structure has not changed
            if (Dirty)
            {

                GeneratePhenotype();
            }
        }

        /// <summary>
        /// Resizes the node array and adds the given node to the end of the array.
        /// </summary>
        /// <param name="node"></param>
        public void AddNode(INeatNode node)
        {
            Extensions.Array.ResizeAndAdd(ref _Nodes, node);

            Dirty = true;
        }

        /// <summary>
        /// Gets an id from the <see cref="GlobalInnovations"/>, sets the provided's <see cref="IInnovation.Id"/>, resizes the <see cref="Nodes"/> array, and sets the last element to the <paramref name="innovation"/>
        /// </summary>
        /// <param name="innovation"></param>
        /// <returns></returns>
        public async Task AddInnovation(IInnovation innovation)
        {
            // set the innovation id of the new innovation, if the new gene is unique we will get a new number from the static method, if it already exists we will get its existing innovation number
            // also add it to the global genome
            innovation.Id = await GlobalInnovations.Add(innovation);

            // resize the backing array for the innovations to accomodate the new innovation
            Extensions.Array.ResizeAndAdd(ref _Innovations, innovation);

            Dirty = true;
        }
    }
}
