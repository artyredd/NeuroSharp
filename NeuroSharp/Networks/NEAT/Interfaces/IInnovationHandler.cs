using System.Collections.Generic;
using System.Threading.Tasks;

namespace NeuroSharp.NEAT
{
    public interface IInnovationHandler
    {
        Dictionary<string, int> InnovationHashes { get; }
        IInnovation[] Innovations { get; }

        Task<int> Add(IInnovation innovation);
        Task<int> Clear();
        public int Count();
    }
}