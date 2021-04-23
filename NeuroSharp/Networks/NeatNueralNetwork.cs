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
    public class NeatNueralNetwork : INeatNetwork
    {
        public NeatNueralNetwork(ushort InputNodes, ushort OutputNodes)
        {
            Inputs = InputNodes;
            Outputs = OutputNodes;

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

        public static IInnovationHandler GlobalInnovations { get; } = new InnovationHandler();

        /// <summary>
        /// Gets the <see cref="IMutater"/> that provides functionality to how this NEAT mutates during runtime.
        /// </summary>
        public IMutater Mutater { get; init; } = new DefaultMutater();

        /// <summary>
        /// Gets the <see cref="IEvaluator"/> that should propogate values through this NEAT network and evaluate the output from this NEAT network. This is different from a <see cref="NeuroSharp.Propagation.IPropogator{T}"/>. NEAT Networks use <see cref="IEvaluator"/>s instead.
        /// </summary>
        public IEvaluator Evaluator { get; init; } = new DefaultEvaluator();

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
        public int Inputs { get; private set; } = 0;

        /// <summary>
        /// The number of ouput nodes this network was created with
        /// </summary>
        public int Outputs { get; private set; } = 0;

        // a list would be more convenient but this is ~5x faster
        internal INeatNode[] _Nodes = Array.Empty<INeatNode>();

        internal volatile int NextNodeNumber = 0;

        internal volatile bool Dirty = true;

        public bool TopologyChanged => Dirty;

        /// <summary>
        /// The hashes of all the innovations that this network currently has in its genome
        /// </summary>
        public HashSet<string> InnovationHashes { get; init; } = new();

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
        public void GeneratePhenotype()
        {
            // avoid unecessary and very performance costly genotype generation
            if (Dirty is false)
            {
                return;
            }

            // mark ourselfs as having regenerated our structure
            Dirty = false;

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

            NodeIndexSemaphore.Wait();
            NodeIndexDictionary.Clear();
            NodeIndexSemaphore.Release();

            // iterate through the nodes and create the arrays that contain the connections using the dictionaries we just built
            for (int i = 0; i < nodes.Length; i++)
            {
                var node = nodes[i];

                NodeIndexSemaphore.Wait();
                if (NodeIndexDictionary.ContainsKey(node.Id) is false)
                {
                    NodeIndexDictionary.Add(node.Id, i);
                }
                NodeIndexSemaphore.Release();

                // this is for the input nodes, they do NOT input from anywhere, their inputs are the provided inputs for an evaluation, dont waste the time searching a dict if we dont have to

                // check to see if we need to create an array for input nodes
                // we may not need to if this node is an input for example
                if (node.NodeType != NodeType.Output && Inputs.ContainsKey(node.Id))
                {
                    Span<IInnovation> inputs = new(Inputs[node.Id]);

                    // assign the indexs for the innovations so we can propogate later on

                    foreach (var item in inputs)
                    {
                        item.InputNodeIndex = (ushort)i;
                    }

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

                    // make sure we assign the indexes so nodes can find eachother later on
                    foreach (var item in outputs)
                    {
                        item.OutputNodeIndex = (ushort)i;
                    }

                    Span<IInnovation> nodeSpan = new(node.InputNodes);

                    outputs.CopyTo(nodeSpan);
                }
            }
        }

        public ushort IncrementNodeCount() => (ushort)Interlocked.Increment(ref NextNodeNumber);
        public ushort DecrementNodeCount() => (ushort)Interlocked.Decrement(ref NextNodeNumber);

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
            if (Data?.Length is null or 0 || Data.Length != Inputs)
            {
                throw Networks.Exceptions.InconsistentTrainingDataScheme(Inputs, Data.Length);
            }

            // make sure out structure has not changed
            if (Dirty)
            {

                GeneratePhenotype();
            }

            // this is by default a recursive function, may need a refactor for performance reasons (if there are any)

            return Evaluator.Evaluate(Data, this);
        }

        /// <summary>
        /// Resizes the node array and adds the given node to the end of the array.
        /// </summary>
        /// <param name="node"></param>
        public void AddNode(INeatNode node)
        {
            Array.Resize(ref _Nodes, _Nodes.Length + 1);

            Nodes[^1] = node;

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
            Array.Resize(ref _Innovations, _Innovations.Length + 1);

            Innovations[^1] = innovation;

            Dirty = true;
        }
    }
}
