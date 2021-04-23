using System.Collections.Generic;
using System.Threading.Tasks;

namespace NeuroSharp.NEAT
{
    public interface INeatNetwork
    {
        static IInnovationHandler GlobalInnovations { get; }
        IInnovation[] Innovations { get; set; }
        INeatNode[] Nodes { get; set; }
        IMutater Mutater { get; }
        int Inputs { get; }
        int Outputs { get; }
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