using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuroSharp.NEAT
{
    public interface IInstantiableNetwork<T>
    {
        T Create(int InputNodes, int OutputNodes);
        T Create(int InputNodes, int OutputNodes, IInnovation[] Genome);
        Task<T> CreateAsync(int InputNodes, int OutputNodes);
        Task<T> CreateAsync(int InputNodes, int OutputNodes, IInnovation[] Genome);
    }
}
