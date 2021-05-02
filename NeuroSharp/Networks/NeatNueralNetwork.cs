using NeuroSharp.NEAT.Structs;
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
        public string Name { get; set; } = "";

        public NeatNueralNetwork(int InputNodes, int OutputNodes)
        {
            this.InputNodes = InputNodes;
            this.OutputNodes = OutputNodes;

            this.Mutater.InitializeConnections(this);
        }

        public NeatNueralNetwork()
        {

        }

        /// <summary>
        /// Responsible for handling the global innovations for all networks. Most notably the semantics involved with maintaining an asynchronous list of innovations across all networks.
        /// </summary>
        public static IInnovationHandler GlobalInnovations { get; } = new InnovationHandler();

        /// <summary>
        /// Gets the <see cref="IMutater"/> that provides functionality to how this NEAT mutates during runtime.
        /// </summary>
        public IMutater Mutater { get; init; } = new DefaultMutater();

        /// <summary>
        /// Gets the <see cref="IEvaluator"/> that should propogate values through this NEAT network and evaluate the output from this NEAT network. This is different from a <see cref="NeuroSharp.Propagation.IPropogator{T}"/>. NEAT Networks use <see cref="IEvaluator"/>s instead.
        /// </summary>
        public IEvaluator<double, double[], double> Evaluator { get; init; } = new DefaultEvaluator();

        /// <summary>
        /// Responsible for generating the explicit representation of the network using <see cref="Innovations"/>
        /// </summary>
        public IPhenotypeGenerator<double> PhenotypeGenerator { get; init; } = new DefaultPhenotypeGenerator<double>();

        /// <summary>
        /// The layers of this network. Each <see cref="int"/>[] at <c>n</c> index represents the nth layer of the network.
        /// <code>
        /// [0] = Output Layer
        /// </code>
        /// <code>
        /// [1] = Input Layer
        /// </code>
        /// <code>
        /// [n] = nth Layer
        /// </code>
        /// <code>
        /// Generated with <see cref="GeneratePhenotype"/>
        /// </code>
        /// </summary>
        public int[][] NodeLayers { get; private set; } = Array.Empty<int[]>();

        /// <summary>
        /// The nodes of this network. Key: Node Id, Value: The layer the node is contained within.
        /// <code>
        /// Generated with <see cref="GeneratePhenotype"/>
        /// </code>
        /// </summary>
        public IDictionary<int, ushort> NodeDictionary { get; private set; }

        /// <summary>
        /// The innovations that this network has evolved
        /// </summary>
        public IInnovation[] Innovations
        {
            get { return _Innovations; }
            set { _Innovations = value; }
        }
        internal IInnovation[] _Innovations = Array.Empty<Innovation>();

        /// <summary>
        /// The number of input nodes this network was created with
        /// </summary>
        public int InputNodes { get; private set; } = 0;

        /// <summary>
        /// The number of ouput nodes this network was created with
        /// </summary>
        public int OutputNodes { get; private set; } = 0;

        /// <summary>
        /// Gets the total number of nodes in this network.
        /// </summary>
        public int Count => NodeDictionary?.Count ?? 0;

        internal volatile int NextNodeNumber = 0;

        internal volatile bool Dirty = true;

        /// <summary>
        /// <see langword="true"/> when any innovation has been changed in any way, such that, the topology of the network has been altered. This happens when adding nodes, adding connections, or any modification to connections or nodes.
        /// <para>
        /// Example:
        /// <code>
        /// var network = new NeatNueralNetwork(3,2);
        /// </code>
        /// <code>
        /// network.AddInnovation( ... );
        /// </code>
        /// Outputs:
        /// <code>
        /// TopologyChanged: true
        /// </code>
        /// </para>
        /// </summary>
        public bool TopologyChanged => Dirty;

        public IMatrix<double>[] Matrices { get; private set; }

        /// <summary>
        /// The hashes of all the innovations that this network currently has in its genome
        /// </summary>
        public HashSet<string> InnovationHashes { get; init; } = new();

        public INeatNetwork Create(int InputNodes, int OutputNodes) => CreateAsync(InputNodes, OutputNodes).Result;

        /// <summary>
        /// Creates a new INeatNetwork object with the given <paramref name="InputNodes"/> and <paramref name="OutputNodes"/> and initialized using the <see cref="IMutater.InitializeConnections(INeatNetwork)"/>
        /// </summary>
        /// <param name="InputNodes"></param>
        /// <param name="OutputNodes"></param>
        /// <returns></returns>
        public async Task<INeatNetwork> CreateAsync(int InputNodes, int OutputNodes)
        {
            // create the default connections
            // per the specs all input nodes must connection to all output nodes
            await Mutater.InitializeConnections(this);

            return new NeatNueralNetwork((ushort)InputNodes, (ushort)OutputNodes);
        }

        /// <summary>
        /// Creates a new INeatNetwork object with the given <paramref name="InputNodes"/> and <paramref name="OutputNodes"/> and initialized using the <see cref="IMutater.InitializeConnections(INeatNetwork)"/>
        /// </summary>
        /// <param name="InputNodes"></param>
        /// <param name="OutputNodes"></param>
        /// <returns></returns>
        public async Task<INeatNetwork> CreateAsync(int InputNodes, int OutputNodes, IInnovation[] Genome)
        {
            INeatNetwork newNetwork = new NeatNueralNetwork()
            {
                InputNodes = InputNodes,
                OutputNodes = OutputNodes,
                Dirty = true
            };

            // import the innovations
            for (int i = 0; i < Genome.Length; i++)
            {
                string hash = Genome[i].Hash();
                if (newNetwork.InnovationHashes.Add(hash))
                {
                    await newNetwork.AddInnovation(Genome[i]);
                }
            }

            newNetwork.GeneratePhenotype();

            await Helpers.Sleep();

            return newNetwork;
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
            if (Dirty is false)
            {
                return;
            }

            Dirty = false;

            // decode the genome into several dictionaries that will help with constructing the matrices that are used for evaluation
            DecodedGenome genome = PhenotypeGenerator.DecodeGenome(this);

            // store the dictionary for fast lookup of node layers
            this.NodeDictionary = genome.NodeDictionary;

            // derive the layers from the dictionary
            this.NodeLayers = PhenotypeGenerator.GetLayers(ref genome);

            // get the largest node id contained within innovations so we can reset the nextNodeNumber
            Interlocked.Add(ref NextNodeNumber, GetLargestNodeId() - NextNodeNumber);

            this.Matrices = PhenotypeGenerator.GenerateMatrices(ref genome);
        }

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
            return Evaluator.Evaluate(Data, this);
        }

        internal async Task VerifyNetworkBeforeEvaluation(double[] Data)
        {
            if (Data?.Length is null or 0 || Data.Length != InputNodes)
            {
                throw Networks.Exceptions.InconsistentTrainingDataScheme(InputNodes, Data.Length);
            }

            // make sure out structure has not changed
            if (Dirty)
            {
                GeneratePhenotype();
            }

            // placeholder for real async stuff
            await Helpers.Sleep();
        }

        /// <summary>
        /// Gets an id from the <see cref="GlobalInnovations"/>, sets the provided's <see cref="IInnovation.Id"/>, resizes the <see cref="Innovations"/> array, and sets the last element to the <paramref name="innovation"/>
        /// </summary>
        /// <param name="innovation"></param>
        /// <returns></returns>
        public async Task AddInnovation(IInnovation innovation)
        {
            // set the innovation id of the new innovation, if the new gene is unique we will get a new number from the static method, if it already exists we will get its existing innovation number
            // also add it to the global genome
            innovation.Id = await GlobalInnovations.Add(innovation);

            // resize the backing array for the innovations to accomodate the new innovation
            Helpers.Array.AppendValue(ref _Innovations, ref innovation);

            Dirty = true;
        }

        public ushort IncrementNextNodeId()
        {
            // explicit casting w/ boxing was ~3x faster than using an actual Ushort with semaphore and 2x faster than using traditional object locking
            return (ushort)Interlocked.Increment(ref NextNodeNumber);
        }

        public ushort DecrementNextNodeId()
        {
            // explicit casting w/ boxing was ~3x faster than using an actual Ushort with semaphore and 2x faster than using traditional object locking
            return (ushort)Interlocked.Decrement(ref NextNodeNumber);
        }

        private int GetLargestNodeId()
        {
            Span<IInnovation> innovations = new(_Innovations);
            ushort largestId = 0;
            for (int i = 0; i < innovations.Length; i++)
            {
                if (innovations[i].InputNode > largestId)
                {
                    largestId = innovations[i].InputNode;
                }
                if (innovations[i].OutputNode > largestId)
                {
                    largestId = innovations[i].OutputNode;
                }
            }
            return largestId;
        }

        public void Reset()
        {
            InnovationHashes?.Clear();

            NodeDictionary?.Clear();

            NodeLayers = Array.Empty<int[]>();

            _Innovations = Array.Empty<IInnovation>();

            Dirty = true;

            NextNodeNumber = 0;

            Interlocked.Add(ref NextNodeNumber, InputNodes);

            Interlocked.Add(ref NextNodeNumber, OutputNodes);
        }
    }
}
