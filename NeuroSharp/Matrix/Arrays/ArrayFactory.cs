using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuroSharp
{
    /// <summary>
    /// Creates and fills various Jagged Arrays
    /// </summary>
    public static class ArrayFactory
    {
        /// <summary>
        /// Creates a new Row-Based Jagged Array with <c>n</c> <paramref name="Rows"/> and <c>m</c> <paramref name="Columns"/>. All values default to <see langword="default"/> (<typeparamref name="T"/>)
        /// <code>
        /// Complexity: O(n)
        /// </code>
        /// <para>
        /// Example:
        /// <c>var matrix = Create&lt;int&gt;(2,2);</c>
        /// </para>
        /// Outputs:
        /// <code>
        ///  0 0
        /// </code>
        /// <code>
        ///  0 0
        /// </code>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Rows">Height of the matrix</param>
        /// <param name="Columns">Width of the matrix</param>
        /// <returns>
        /// <typeparamref name="T"/>[][] Matrix
        /// </returns>
        public static T[][] Create<T>(int Rows, int Columns)
        {
            var matrix = new T[Rows][];

            for (int i = 0; i < Rows; i++)
            {
                matrix[i] = new T[Columns];
                matrix[i].Initialize();
            }

            return matrix;
        }

        /// <summary>
        /// Instantiates a new Matrix array with the given <paramref name="Rows"/> and <paramref name="Columns"/>. Each new item in the array is set using the value returned from an iteration from <paramref name="Enumerable"/>. If the enumerable object fails to produce enough items to fill the entire matrix, all remaining values will be set to their default valies.
        /// <para>
        /// Example Usage:
        /// <code>
        /// var matrix = new Matrix&lt;int&gt;( Rows: 2, Columns: 3, Enumerable: new int[]{ 0, 1, 2, 3 });
        /// </code>
        /// Output:
        /// <code>
        /// Matrix: Row[0] = { 0, 1, 2 }, Row[1] = { 3, 0, 0 }
        /// </code>
        /// </para>
        /// </summary>
        /// <param name="Rows"></param>
        /// <param name="Columns"></param>
        public static T[][] Create<T>(int Rows, int Columns, IEnumerable<T> Enumerable)
        {
            if (Enumerable?.GetEnumerator() is null)
            {
                // if for some reason the enumerable object give has no reference or no enumeration create a default matrix and return it.
                return Create<T>(Rows, Columns);
            }

            var matrix = new T[Rows][];

            using (var Enumerator = Enumerable.GetEnumerator())
            {
                for (int row = 0; row < Rows; row++)
                {
                    // create a new row
                    matrix[row] = new T[Columns];

                    // fill with default values since we have no garuntee the enumerable object that is provided can provide enough values to fill the matrix
                    matrix[row].Initialize();

                    // try to enumerate and set the column values
                    for (int column = 0; column < Columns; column++)
                    {
                        // if we get a value back from the enumerator then set it.
                        if (Enumerator.MoveNext())
                        {
                            matrix[row][column] = Enumerator.Current;
                        }
                    }
                }
            }

            return matrix;
        }

        /// <summary>
        /// Instantiates a new Matrix array with the given <paramref name="Rows"/> and <paramref name="Columns"/>. For each new item in all rows <paramref name="T_Provider"/> is invoked to set the value of the new <typeparamref name="T"/> object in the spot.
        /// <para>
        /// Example Usage:
        /// <code>
        /// var matrix = new Matrix&lt;int&gt;( Rows: 2, Columns: 3, DelegateThatReturnsT: ()=&gt; 1);
        /// </code>
        /// Output:
        /// <code>
        /// Matrix: Row[0] = { 1, 1, 1 }, Row[1] = { 1, 1, 1 }
        /// </code>
        /// </para>
        /// </summary>
        /// <param name="Rows"></param>
        /// <param name="Columns"></param>
        public static T[][] Create<T>(int Rows, int Columns, Func<T> T_Provider)
        {
            var matrix = new T[Rows][];
            for (int row = 0; row < Rows; row++)
            {
                matrix[row] = new T[Columns];

                // create the default values
                for (int column = 0; column < Columns; column++)
                {
                    matrix[row][column] = T_Provider.Invoke();
                }
            }
            return matrix;
        }

        /// <summary>
        /// Instantiates a new Matrix array with the given <paramref name="Rows"/> and <paramref name="Columns"/>. For each new item in all rows <paramref name="T_ProviderUsingRow"/> is invoked to set the value of the new <typeparamref name="T"/> object in the spot, this <paramref name="T_ProviderUsingRow"/> is invoked with a <see cref="int"/> Row Index where that element is contained.
        /// <para>
        /// Example Usage(where i = Row Index where the element is located):
        /// <code>
        /// var matrix = new Matrix&lt;int&gt;( Rows: 2, Columns: 3, DelegateThatReturnsTUsingRowNumber: (i)=&gt; i);
        /// </code>
        /// Output:
        /// <code>
        /// Matrix: Row[0] = { 0, 0, 0 }, Row[1] = { 1, 1, 1 }
        /// </code>
        /// </para>
        /// </summary>
        /// <param name="Rows"></param>
        /// <param name="Columns"></param>
        public static T[][] Create<T>(int Rows, int Columns, Func<int, T> T_ProviderUsingRow)
        {
            var matrix = new T[Rows][];
            for (int row = 0; row < Rows; row++)
            {
                matrix[row] = new T[Columns];

                // create the default values
                for (int column = 0; column < Columns; column++)
                {
                    matrix[row][column] = T_ProviderUsingRow.Invoke(row);
                }
            }
            return matrix;
        }

        /// <summary>
        /// Instantiates a new Matrix array with the given <paramref name="Rows"/> and <paramref name="Columns"/>. For each new item in all rows <paramref name="T_ProviderUsingIndexes"/> is invoked to set the value of the new <typeparamref name="T"/> object in the spot, this <paramref name="T_ProviderUsingIndexes"/> is invoked with a <see cref="int"/> Row Index where that element is contained.
        /// <para>
        /// Example Usage(where x = Row Index where the element is located, and y = Column Index where the element is located):
        /// <code>
        /// var matrix = new Matrix&lt;int&gt;( Rows: 2, Columns: 3, DelegateThatReturnsTUsingRowNumberAndColumnNumber: (x,y)=&gt; x + y);
        /// </code>
        /// Output:
        /// <code>
        /// Matrix: Row[0] = { 0, 1, 2 }, Row[1] = { 1, 2, 3 }
        /// </code>
        /// </para>
        /// </summary>
        /// <param name="Rows"></param>
        /// <param name="Columns"></param>
        public static T[][] Create<T>(int Rows, int Columns, Func<int, int, T> T_ProviderUsingIndexes)
        {
            var matrix = new T[Rows][];
            for (int row = 0; row < Rows; row++)
            {
                matrix[row] = new T[Columns];

                // create the default values
                for (int column = 0; column < Columns; column++)
                {
                    matrix[row][column] = T_ProviderUsingIndexes.Invoke(row, column);
                }
            }
            return matrix;
        }

        /// <summary>
        /// Instantiates a new Matrix array with the given <paramref name="Rows"/> and <paramref name="Columns"/>. Each new item in the array is set to the provided <paramref name="DefaultValue"/>
        /// <para>
        /// Example Usage:
        /// <code>
        /// var matrix = new Matrix&lt;int&gt;( Rows: 2, Columns: 3, DefaultValue: 12);
        /// </code>
        /// Output:
        /// <code>
        /// Matrix: Row[0] = { 12, 12, 12 }, Row[1] = { 12, 12, 12 }
        /// </code>
        /// </para>
        /// </summary>
        /// <param name="Rows"></param>
        /// <param name="Columns"></param>
        public static T[][] Create<T>(int Rows, int Columns, T DefaultValue)
        {
            var matrix = new T[Rows][];

            for (int row = 0; row < Rows; row++)
            {
                matrix[row] = new T[Columns];

                Array.Fill(matrix[row], DefaultValue);
            }

            return matrix;
        }
    }
}
