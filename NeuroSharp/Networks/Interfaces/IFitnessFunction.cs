using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuroSharp
{
    public interface IFitnessFunction<in U, T>
    {
        T CheckFitness(U Result);
        Task<T> CheckFitnessAsync(U Result);
    }
}
