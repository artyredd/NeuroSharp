using System.Collections.Generic;
using System.Threading.Tasks;

namespace NeuroSharp.NEAT
{
    public interface INeatNetwork : IMutableNetwork, INetwork<double[], double>
    {
        IInnovation[] Innovations { get; set; }

        INeatNode[] Nodes { get; set; }

        bool TopologyChanged { get; }

        HashSet<string> InnovationHashes { get; }

        void GeneratePhenotype();

        ushort IncrementNodeCount();

        ushort DecrementNodeCount();

        void AddNode(INeatNode node);

        Task AddInnovation(IInnovation innovation);

        bool TryGetIndex(ushort NodeId, out int index);
    }
}