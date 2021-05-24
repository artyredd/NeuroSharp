using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NeuroSharp.Extensions.Matrix;

namespace NeuroSharp
{
    /// <summary>
    /// Represents the transposed version of the parent matrix
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TransposedMatrix<T> : BaseMatrix<T> where T : unmanaged, IComparable<T>, IEquatable<T>
    {
        public override ref T this[int RowIndex, int ColumnIndex] => ref base[ColumnIndex, RowIndex];

        public override int Rows => base.Columns;

        public override int Columns => base.Rows;

        public TransposedMatrix(ref T[][] Matrix)
        {
            base._Matrix = Matrix;
        }

        /// <summary>
        /// Duplicates the transposed matrix of the parent matrix.
        /// <code>Avoids boxing on heap</code>
        /// <code>Complexity: O(n²)</code>
        /// </summary>
        /// <returns>
        /// <see cref="BaseMatrix{T}"/>
        /// </returns>
        public override IMatrix<T> Duplicate()
        {
            var matrix = base.Duplicate().Transpose();

            return matrix;
        }
    }
}
