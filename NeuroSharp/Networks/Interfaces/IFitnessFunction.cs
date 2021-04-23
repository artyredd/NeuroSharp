using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuroSharp
{
    public interface IFitnessFunction<T> where T : unmanaged, IComparable<T>, IEquatable<T>
    {
        T CheckFitness(IMatrix<T> Result);
        Task<T> CheckFitnessAsync(IMatrix<T> Result);
    }
}
