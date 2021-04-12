using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuroSharp
{
    /// <summary>
    /// An abtraction layer over a basic matrix <see cref="T[][]"/>, auto implements public methods for adding, transposing, and multiplying itself with <see cref="int"/>egers, <see cref="float"/>s, other <see cref="Matrix{T}"/>s etc..
    /// </summary>
    public class Matrix<T> :
        ICollection<T[]>,
        IEnumerable<T[]>,
        IStructuralComparable,
        IStructuralEquatable,
        ICloneable
        where T : unmanaged, IComparable<T>, IEquatable<T>

    {
        public T[][] _Matrix { get; init; }

        // Allow this object to be used as a multidimensional array for convenience
        public ref T this[int RowIndex, int ColumnIndex] => ref _Matrix[RowIndex][ColumnIndex];

        public ref T[] this[int _RowIndex] => ref _Matrix[_RowIndex];

        /// <summary>
        /// Gets the total capacity of this matrix. Calculated by multiplying the number of rows by the number of columns.
        /// </summary>
        public int Capacity => _Matrix is null ? 0 : _Matrix.Length * this[0].Length;

        public int Rows => Count;

        public int Columns => _Matrix is null ? 0 : this[0].Length;

        public delegate T MemberWiseOperation(ref T Member);

        // BEGIN CONSRUCTORS

        /// <summary>
        /// Instantiates a new Matrix array with the given <paramref name="Rows"/> and <paramref name="Columns"/>
        /// </summary>
        /// <param name="Rows"></param>
        /// <param name="Columns"></param>
        public Matrix(int Rows, int Columns)
        {
            _Matrix = InstantiateMatrix(Rows, Columns);
        }

        /// <summary>
        /// Instantiates a new Matrix array with the given <paramref name="Rows"/> and <paramref name="Columns"/>. For each new item in all rows <paramref name="DelegateThatReturnsT"/> is invoked to set the value of the new <typeparamref name="T"/> object in the spot.
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
        public Matrix(int Rows, int Columns, Func<T> DelegateThatReturnsT)
        {
            _Matrix = InstantiateMatrix(Rows, Columns, DelegateThatReturnsT);
        }

        /// <summary>
        /// Instantiates a new Matrix array with the given <paramref name="Rows"/> and <paramref name="Columns"/>. For each new item in all rows <paramref name="DelegateThatReturnsTUsingRowNumber"/> is invoked to set the value of the new <typeparamref name="T"/> object in the spot, this <paramref name="DelegateThatReturnsTUsingRowNumber"/> is invoked with a <see cref="int"/> Row Index where that element is contained.
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
        public Matrix(int Rows, int Columns, Func<int, T> DelegateThatReturnsTUsingRowNumber)
        {
            _Matrix = InstantiateMatrix(Rows, Columns, DelegateThatReturnsTUsingRowNumber);
        }

        /// <summary>
        /// Instantiates a new Matrix array with the given <paramref name="Rows"/> and <paramref name="Columns"/>. For each new item in all rows <paramref name="DelegateThatReturnsTUsingRowNumberAndColumnNumber"/> is invoked to set the value of the new <typeparamref name="T"/> object in the spot, this <paramref name="DelegateThatReturnsTUsingRowNumberAndColumnNumber"/> is invoked with a <see cref="int"/> Row Index where that element is contained.
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
        public Matrix(int Rows, int Columns, Func<int, int, T> DelegateThatReturnsTUsingRowNumberAndColumnNumber)
        {
            _Matrix = InstantiateMatrix(Rows, Columns, DelegateThatReturnsTUsingRowNumberAndColumnNumber);
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
        public Matrix(int Rows, int Columns, T DefaultValue)
        {
            _Matrix = InstantiateMatrix(Rows, Columns, DefaultValue);
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
        public Matrix(int Rows, int Columns, IEnumerable<T> Enumerable)
        {
            _Matrix = InstantiateMatrix(Rows, Columns, Enumerable);
        }

        private Matrix()
        {

        }

        /// <summary>
        /// Instantiates the internal matrix with either default values or using the provided delegate that returns T
        /// </summary>
        /// <param name="Rows"></param>
        /// <param name="Columns"></param>
        /// <param name="DelegateOrDefaultValue"></param>
        private T[][] InstantiateMatrix(int Rows, int Columns, object DelegateOrDefaultValue = null)
        {
            // make sure the user is not attempting to create a matrix that will take 10 minutes to create and/or fail to create.
            if (Rows is <= 0 or > 2_147_450_000)
            {
                throw new ArgumentOutOfRangeException(nameof(Rows), $"Attempted to instantiate a new {nameof(Matrix<T>)} with number of Rows outside of range 0 .. 2.147B");
            }

            if (Columns is <= 0 or > 2_147_450_000)
            {
                throw new ArgumentOutOfRangeException(nameof(Columns), $"Attempted to instantiate a new {nameof(Matrix<T>)} with number of Columns outside of range 0 .. 2.147B");
            }

            var (ExceedsSize, EstimatedSize, AvailableMemory) = Helpers.PotentialSizeExceedsTotalMemory<T>(Rows, Columns);

            if (ExceedsSize)
            {
                throw new OutOfMemoryException($"Attempted to create a matrix with {Rows} Rows and {Columns} Columns, whos size in bits({EstimatedSize}) is larger than available memory ({AvailableMemory})!");
            }

            // instantiate the matrix since it's readonly
            var newMatrix = new T[Rows][];

            /*
                Matrix Format is as follows

                       Column[0] Column[1] Column[n]
                Row[0]: ___|_________|_________|____
                Row[1]: ___|_________|_________|____
                Row[n]: ___|_________|_________|____
            */

            // it may seem like i could factor out the for loop in each case, however there is a non-neglible performance impact the switch every loop that way
            switch (DelegateOrDefaultValue)
            {
                // if it's null break and the values will be set to default at the end of this method
                case null:
                    break;
                /*
                Usage new Matrix<int>(1, 1, 14)

                Output: 
                       Column[0] 
                Row[0]:   14

                */
                case T value:

                    for (int row = 0; row < Rows; row++)
                    {
                        newMatrix[row] = new T[Columns];

                        Array.Fill(newMatrix[row], value);
                    }

                    return newMatrix;
                // usage (int row, int column) => row + column etc..
                case Func<int, int, T> RowAndColumnDelegate:
                    for (int row = 0; row < Rows; row++)
                    {
                        newMatrix[row] = new T[Columns];

                        // create the default values
                        for (int column = 0; column < Columns; column++)
                        {
                            newMatrix[row][column] = RowAndColumnDelegate.Invoke(row, column);
                        }
                    }
                    return newMatrix;

                // usage (int row) => row + 1 etc...
                case Func<int, T> RowParamDelegate:
                    for (int row = 0; row < Rows; row++)
                    {
                        newMatrix[row] = new T[Columns];

                        // create the default values
                        for (int column = 0; column < Columns; column++)
                        {
                            newMatrix[row][column] = RowParamDelegate.Invoke(row);
                        }
                    }
                    return newMatrix;

                // usage () => 420 etc...
                case Func<T> NoParamsDelegate:
                    for (int row = 0; row < Rows; row++)
                    {
                        newMatrix[row] = new T[Columns];

                        // create the default values
                        for (int column = 0; column < Columns; column++)
                        {
                            newMatrix[row][column] = NoParamsDelegate.Invoke();
                        }
                    }
                    return newMatrix;

                // usage: new Matrix<int>(1,1, int[] {1,2,3})
                case IEnumerable<T> Enumerable:
                    using (var Enumerator = Enumerable.GetEnumerator())
                    {
                        // if the enumerator is broken, null or empty break away and fill with default values
                        if (Enumerator is null)
                        {
                            break;
                        }

                        for (int row = 0; row < Rows; row++)
                        {
                            // create a new row
                            newMatrix[row] = new T[Columns];

                            // create the default values
                            for (int column = 0; column < Columns; column++)
                            {
                                // if we get a value back from the enumerator then use it, else use a default value
                                if (Enumerator.MoveNext())
                                {
                                    newMatrix[row][column] = Enumerator.Current;
                                }
                                else
                                {
                                    newMatrix[row][column] = default;
                                }
                            }
                        }
                    }
                    return newMatrix;

                // if we got here something is wrong, i would unit test this but i cant think of anything that bypasses the default microsoft rosyln/compiler type matching to break this, let me know if you find anything i will add it to the tests, or better yet submit a pull request =)
                default:
                    throw new NotSupportedException(string.Format("Failed to parse the provided delegate as something that returns Type({0}) Expected Something like Func<{0}>, instead got: {1}", typeof(T), DelegateOrDefaultValue.GetType().Name));
            }

            // if we got here just fill the matrix with default values
            for (int i = 0; i < Rows; i++)
            {

                newMatrix[i] = new T[Columns];
                newMatrix[i].Initialize();
            }

            return newMatrix;
        }

        /// <summary>
        /// Invokes the provided operation member-wise on this matrix (on each element)
        /// <code>
        /// Complexity: O(n)
        /// </code>
        /// <para>
        /// Elements are mutated in-place using ref;
        /// </para>
        /// Example Usage:
        /// <code>
        /// Matrix&lt;int&gt; matrix = new(2,2);
        /// </code>
        /// <code>
        /// matrix[0][0] = 4; matrix[0, 1] = 1;
        /// </code>
        /// <code>
        /// matrix.ApplyMemberWiseOperation((ref int x) => x *= 2);
        /// </code>
        /// Outputs:
        /// <code>
        /// 8 2
        /// </code>
        /// <code>
        /// 0 0
        /// </code>
        /// </summary>
        /// <param name="Operation"></param>
        /// <returns></returns>
        public void ApplyMemberWiseOperation(MemberWiseOperation Operation)
        {
            for (int row = 0; row < Rows; row++)
            {
                for (int column = 0; column < Columns; column++)
                {
                    Operation(ref _Matrix[row][column]);
                }
            }
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
        /// <see cref="Matrix{T}"/>
        /// </returns>
        public Matrix<T> Transpose()
        {
            // ceate a new matrix with opposite shape
            Matrix<T> newMatrix = new(Columns, Rows);


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

            for (int SourceRow = 0; SourceRow < Rows; SourceRow++)
            {
                for (int SourceColumn = 0; SourceColumn < Columns; SourceColumn++)
                {
                    newMatrix[SourceColumn, SourceRow] = this[SourceRow, SourceColumn];
                }
            }

            return newMatrix;
        }

        /// <summary>
        /// Combines the entire matrix into a one-dimensional array.
        /// <code>
        /// Complexity: O(n)
        /// </code>
        /// </summary>
        /// <returns></returns>
        public T[] ToArray()
        {
            T[] megaArray = new T[Capacity];
            for (int i = 0; i < Rows; i++)
            {
                Array.Copy(_Matrix[i], 0, megaArray, i * Columns, Columns);
            }
            return megaArray;
        }

        // BEGIN ARRAY EXTENSION PROPERTIES AND METHODS

        // extend the same interfaces as a normal array so users will implicitly understand how to use this object
        public int Count => _Matrix is null ? 0 : _Matrix.Length;

        public bool IsSynchronized => _Matrix.IsSynchronized;

        public object SyncRoot => _Matrix.SyncRoot;

        public bool IsFixedSize => _Matrix.IsFixedSize;

        public bool IsReadOnly => _Matrix.IsReadOnly;

        public void Add(T[] item) => ((ICollection<T[]>)_Matrix).Add(item);

        public void Clear() => ((ICollection<T[]>)_Matrix).Clear();

        public bool Contains(T[] item) => ((ICollection<T[]>)_Matrix).Contains(item);

        public void CopyTo(T[][] array, int arrayIndex) => ((ICollection<T[]>)_Matrix).CopyTo(array, arrayIndex);

        public bool Remove(T[] item) => ((ICollection<T[]>)_Matrix).Remove(item);

        public IEnumerator<T[]> GetEnumerator() => ((IEnumerable<T[]>)_Matrix).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => _Matrix.GetEnumerator();

        public int CompareTo(object other, IComparer comparer) => ((IStructuralComparable)_Matrix).CompareTo(other, comparer);

        public bool Equals(object other, IEqualityComparer comparer) => ((IStructuralEquatable)_Matrix).Equals(other, comparer);

        public int GetHashCode(IEqualityComparer comparer) => ((IStructuralEquatable)_Matrix).GetHashCode(comparer);

        /// <summary>
        /// Creates a clone of this object. Avoids boxing the object on the heap.
        /// </summary>
        /// <returns>
        /// <see cref="Matrix{T}"/>
        /// </returns>
        public Matrix<T> Duplicate()
        {


            T[][] ClonedMatrix = new T[Rows][];

            for (int row = 0; row < Rows; row++)
            {
                ClonedMatrix[row] = new T[Columns];
                Array.Copy(_Matrix[row], Columns * 0, ClonedMatrix[row], 0, Columns);
            }

            var newMatrix = new Matrix<T>()
            {
                _Matrix = ClonedMatrix
            };

            return newMatrix;
        }

        /// <summary>
        /// Creates a shallow clone of the object
        /// <code>
        /// Boxed on Heap
        /// </code>
        /// </summary>
        /// <returns>
        /// <see cref="object"/>
        /// </returns>
        public object Clone()
        {
            return Duplicate();
        }
    }
}