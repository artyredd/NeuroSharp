﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
        public static T[] ToOneDimension<T>(this IMatrix<T> matrix) where T : unmanaged, IComparable<T>, IEquatable<T>
        {
            T[] megaArray = new T[matrix.Capacity];

            for (int i = 0; i < matrix.Rows; i++)
            {
                System.Array.Copy(matrix[i], 0, megaArray, i * matrix.Columns, matrix.Columns);
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
        public static IMatrix<T> Transpose<T>(this IMatrix<T> sourceMatrix) where T : unmanaged, IComparable<T>, IEquatable<T>
        {
            // ceate a new matrix with opposite shape
            BaseMatrix<T> targetMatrix = new(sourceMatrix.Columns, sourceMatrix.Rows)
            {
                SameTypedOperations = sourceMatrix.SameTypedOperations,
                SByteOperations = sourceMatrix.SByteOperations,
                ByteOperations = sourceMatrix.ByteOperations,
                ShortOperations = sourceMatrix.ShortOperations,
                IntegerOperations = sourceMatrix.IntegerOperations,
                LongOperations = sourceMatrix.LongOperations,
                FloatOperations = sourceMatrix.FloatOperations,
                DoubleOperations = sourceMatrix.DoubleOperations,
                DecimalOperations = sourceMatrix.DecimalOperations,
            };

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

        public static IMatrix<T> TransposeWithMutableSpan<T>(this IMatrix<T> sourceMatrix) where T : unmanaged, IComparable<T>, IEquatable<T>
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
        public static bool Equals<T>(this IMatrix<T> Left, IMatrix<T> right) where T : unmanaged, IComparable<T>, IEquatable<T>
        {
            if (Left.Rows != right.Rows || Left.Columns != right.Columns)
            {
                return false;
            }


            for (int rows = 0; rows < Left.Rows; rows++)
            {
                for (int cols = 0; cols < Left.Columns; cols++)
                {
                    if (Left[rows, cols].Equals(right[rows, cols]) is false)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// Searches the dictionary for the <paramref name="key"/>, if it does not exists it creates a new <see cref="{T}[]"/> with the <paramref name="value"/> as the first entry. If the <paramref name="key"/> is found, the array is resized and the <paramref name="value"/> is added as the last element in the array.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dict"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="Limiter"></param>
        /// <returns></returns>
        public static Task AddValueToArray<T>(this IDictionary<T, T[]> dict, T key, T value) => AddValueToArray(dict, key, value, null);

        /// <summary>
        /// Searches the dictionary for the <paramref name="key"/>, if it does not exists it creates a new <see cref="{T}[]"/> with the <paramref name="value"/> as the first entry. If the <paramref name="key"/> is found, the array is resized and the <paramref name="value"/> is added as the last element in the array.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dict"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="Limiter"></param>
        /// <returns></returns>
        public static async Task AddValueToArray<T>(this IDictionary<T, T[]> dict, T key, T value, SemaphoreSlim Limiter)
        {
            if (Limiter is not null)
            {
                await Limiter.WaitAsync();
            }
            try
            {
                if (dict.ContainsKey(key))
                {
                    var arr = dict[key];

                    // i could probably remove this check since it doesnt make sense for a node to have more than one connection to the same node
                    if (arr.Contains(value) is false)
                    {
                        dict[key] = Array.ResizeAndAdd(arr, value);
                    }
                }
                else
                {
                    dict.Add(key, new T[] { value });
                }
            }
            finally
            {
                Limiter?.Release();
            }
        }

        public static class Array
        {
            /// <summary>
            /// Resizes the <paramref name="array"/> then sets the last element to the <paramref name="value"/>
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="array"></param>
            /// <param name="value"></param>
            /// <returns></returns>
            public static T[] ResizeAndAdd<T>(T[] array, T value) => ResizeAndAdd(ref array, value);

            /// <summary>
            /// Resizes the <paramref name="array"/> then sets the last element to the <paramref name="value"/>
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="array"></param>
            /// <param name="value"></param>
            /// <returns></returns>
            public static T[] ResizeAndAdd<T>(ref T[] array, T value)
            {
                int newIndex = array.Length;

                System.Array.Resize(ref array, newIndex + 1);

                array[newIndex] = value;

                return array;
            }
        }
    }
}
