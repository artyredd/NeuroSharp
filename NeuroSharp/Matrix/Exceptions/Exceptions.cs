using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuroSharp
{
    public static class Exceptions
    {
        /// <summary>
        /// Creates and returns an <see cref="InvalidOperationException"/> with the following message:
        /// <code>
        /// Attempted to perform a lossy operation on a matrix by attempting to [ADD] with a matrix that contains [INTEGER]s with a [DOUBLE].
        /// </code>
        /// </summary>
        /// <param name="Operation"></param>
        /// <param name="LeftType"></param>
        /// <param name="rightType"></param>
        /// <returns></returns>
        public static Exception InvalidMatrix_Operation(string Operation, string LeftType, string rightType)
        {
            return new InvalidOperationException($"Attempted to perform a lossy operation on a matrix by attempting to {Operation} with a matrix that contains {LeftType}s with a {rightType}.");
        }

        /// <summary>
        /// Creates and returns an <see cref="InvalidOperationException"/> with the following message:
        /// <code>
        /// Matrices incompatible for multiplication. The number of columns in the first matrix([4]) must be equal to the number of rows in the second matrix([6])
        /// </code>
        /// </summary>
        /// <param name="LeftRows"></param>
        /// <param name="RightColumns"></param>
        /// <returns></returns>
        public static Exception IncompatibleMatrixShapeForMultiplication(int LeftRows, int RightColumns)
        {
            return new InvalidOperationException($"Matrices incompatible for multiplication.The number of columns in the first matrix({ LeftRows}) must be equal to the number of rows in the second matrix({ RightColumns })");
        }

        /// <summary>
        /// Creates and returns an <see cref="ArgumentException"/> with the following message:
        /// <code>
        /// Failed to create a jagged array matrix from Span&lt;T&gt;. The span's length([10]) must be equal to the number of rows([5]) multiplied by the number of columns([6]). Use ArrayFactory.Create(int, int, IEnumerable<T>) if you're unable to predict if the content can properly fill the resultant matrix.
        /// </code>
        /// </summary>
        /// <param name="LeftRows"></param>
        /// <param name="RightColumns"></param>
        /// <returns></returns>
        public static Exception InconsistentSpanLengthForNewMatrix(int spanLength, int providedRows, int providedColumns)
        {
            return new ArgumentException($"Failed to create a jagged array matrix from Span<T>. The span's length({spanLength}) must be equal to the number of rows({providedRows}) multiplied by the number of columns({providedColumns}). Use {nameof(ArrayFactory.Create)}<T>(int, int, IEnumerable<T>) if you're unable to predict if the content can properly fill the resultant matrix.");
        }
    }
}
