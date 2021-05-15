using System.Collections.Generic;
using System.Threading.Tasks;

namespace NeuroSharp.NEAT
{
    public interface INeatNetwork : IMutableNetwork, INetwork<double[], double>
    {
        IInnovation[] Innovations { get; set; }

        bool TopologyChanged { get; }

        int[][] NodeLayers { get; }

        IMatrix<double>[] Matrices { get; }

        IDictionary<int, ushort> NodeDictionary { get; }

        HashSet<string> InnovationHashes { get; }

        void GeneratePhenotype();

        Task AddInnovation(IInnovation innovation);

        Task<INeatNetwork> ImportGenome(int InputNodes, int OutputNodes, IInnovation[] Genome);

        public ushort IncrementNextNodeId();

        public ushort DecrementNextNodeId();

        Task<INeatNetwork> CreateAsync(int InputNodes, int OutputNodes, IInnovation[] Genome);

        void Reset();
    }
}