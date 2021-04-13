using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuroSharp
{
    public static class MatrixOperations
    {
        public delegate T CombinedMemberwiseOperation<T>(ref T left, ref T right);

        public static BaseMatrix<T> PerformMemberWiseCombinedOperation<T>(BaseMatrix<T> Left, BaseMatrix<T> Right, CombinedMemberwiseOperation<T> Operation) where T : unmanaged, IComparable<T>, IEquatable<T>
        {
            throw new NotImplementedException();
        }
    }
}
