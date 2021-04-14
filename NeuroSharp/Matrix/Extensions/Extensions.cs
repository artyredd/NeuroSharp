using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuroSharp
{
    /// <summary>
    /// Contains helper extensions to assist in handling matricies
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Combines the entire matrix into a one-dimensional array.
        /// <code>
        /// Complexity: O(n)
        /// </code>
        /// <para>
        /// Example:
        /// <c>var matrix = new Matrix&lt;int&gt;( 2, 2, new int[]{ 1 2 3 4 });</c>
        /// </para>
        /// Outputs:
        /// <code>
        /// 1 2
        /// </code>
        /// <code>
        /// 3 4
        /// </code>
        /// Then: <c>matrix.ToArray()</c>
        /// <para>
        /// Ouputs:
        /// </para>
        /// <c>int[]{ 1 2 3 4 }</c>
        /// </summary>
        /// <returns></returns>
        public static T[] ToOneDimension<T>(this BaseMatrix<T> matrix) where T : unmanaged, IComparable<T>, IEquatable<T>
        {
            T[] megaArray = new T[matrix.Capacity];

            for (int i = 0; i < matrix.Rows; i++)
            {
                Array.Copy(matrix[i], 0, megaArray, i * matrix.Columns, matrix.Columns);
            }

            return megaArray;
        }

        /// <summary>
        /// Returns a new Transposed(rotated matrix);
        /// <code>
        /// Complexity: O(n)
        /// </code>
        /// <para>
        /// Example:
        /// <code>
        /// 0 1 2
        /// </code>
        /// <code>
        /// 3 4 5
        /// </code>
        /// Outputs:
        /// <code>
        /// 0 3
        /// </code>
        /// <code>
        /// 1 4
        /// </code>
        /// <code>
        /// 2 5
        /// </code>
        /// </para>
        /// </summary>
        /// <returns>
        /// <see cref="BaseMatrix{T}"/>
        /// </returns>
        public static BaseMatrix<T> Transpose<T>(this BaseMatrix<T> sourceMatrix) where T : unmanaged, IComparable<T>, IEquatable<T>
        {
            // ceate a new matrix with opposite shape
            BaseMatrix<T> targetMatrix = new(sourceMatrix.Columns, sourceMatrix.Rows);

            /*
                Expected Behaviour
                | 1 2 3 |
                | 4 5 6 |

                | 1 4 |
                | 2 5 |
                | 3 6 |
                
                | [0, 0] [0, 1] [0, 2] |
                | [1, 0] [1, 1] [1, 2] |
                
                | [0, 0] [1, 0] |
                | [0, 1] [1, 1] |
                | [0, 2] [1, 2] |
            */

            for (int SourceRow = 0; SourceRow < sourceMatrix.Rows; SourceRow++)
            {
                for (int SourceColumn = 0; SourceColumn < sourceMatrix.Columns; SourceColumn++)
                {
                    targetMatrix[SourceColumn, SourceRow] = sourceMatrix[SourceRow, SourceColumn];
                }
            }

            return targetMatrix;
        }

        public static BaseMatrix<T> TransposeWithMutableSpan<T>(this BaseMatrix<T> sourceMatrix) where T : unmanaged, IComparable<T>, IEquatable<T>
        {
            // ceate a new matrix with opposite shape
            BaseMatrix<T> targetMatrix = new(sourceMatrix.Columns, sourceMatrix.Rows);

            /*
                Expected Behaviour
                | 1 2 3 |
                | 4 5 6 |

                | 1 4 |
                | 2 5 |
                | 3 6 |
                
                | [0, 0] [0, 1] [0, 2] |
                | [1, 0] [1, 1] [1, 2] |
                
                | [0, 0] [1, 0] |
                | [0, 1] [1, 1] |
                | [0, 2] [1, 2] |
            */

            for (int SourceRow = 0; SourceRow < sourceMatrix.Rows; SourceRow++)
            {
                Span<T> Row = new(sourceMatrix[SourceRow]);

                for (int SourceColumn = 0; SourceColumn < sourceMatrix.Columns; SourceColumn++)
                {
                    targetMatrix[SourceColumn, SourceRow] = Row[SourceColumn];
                }
            }

            return targetMatrix;
        }
    }
}
