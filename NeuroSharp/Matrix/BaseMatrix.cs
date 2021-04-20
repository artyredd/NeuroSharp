using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuroSharp
{
    /// <summary>
    /// An abtraction layer over a basic matrix <see cref="T[][]"/>, auto implements public methods for adding, transposing, and multiplying itself with <see cref="int"/>egers, <see cref="float"/>s, other <see cref="BaseMatrix{T}"/>s etc..
    /// </summary>
    public class BaseMatrix<T> :
        ICollection<T[]>,
        IEnumerable<T[]>,
        IStructuralComparable,
        IStructuralEquatable,
        ICloneable,
        IMatrix<T>
        where T : unmanaged, IComparable<T>, IEquatable<T>

    {
        public T[][] _Matrix { get; init; }

        // Allow this object to be used as a multidimensional array for convenience
        public virtual ref T this[int RowIndex, int ColumnIndex] => ref _Matrix[RowIndex][ColumnIndex];

        public virtual ref T[] this[int _RowIndex] => ref _Matrix[_RowIndex];

        public virtual TransposedMatrix<T> Transposed { get; init; }

        /// <summary>
        /// Gets the total capacity of this matrix. Calculated by multiplying the number of rows by the number of columns.
        /// </summary>
        public int Capacity => _Matrix is null ? 0 : _Matrix.Length * this[0].Length;

        public virtual int Rows => Count;

        public virtual int Columns => _Matrix is null ? 0 : this[0].Length;

        public virtual OperationSet<T, int> IntegerOperations { get; set; }
        public virtual OperationSet<T, double> DoubleOperations { get; set; }
        public virtual OperationSet<T, float> FloatOperations { get; set; }
        public virtual OperationSet<T, decimal> DecimalOperations { get; set; }
        public virtual OperationSet<T, long> LongOperations { get; set; }
        public virtual OperationSet<T, short> ShortOperations { get; set; }
        public virtual OperationSet<T, byte> ByteOperations { get; set; }
        public virtual OperationSet<T, sbyte> SByteOperations { get; set; }
        public virtual OperationSet<T, T> SameTypedOperations { get; set; }

        // BEGIN CONSRUCTORS

        public BaseMatrix(int Rows, int Columns)
        {
            var tmp = ArrayFactory.Create<T>(Rows, Columns);
            _Matrix = tmp;
            Transposed = new(ref tmp);
        }


        public BaseMatrix(int Rows, int Columns, Func<T> T_Delegate)
        {
            var tmp = ArrayFactory.Create(Rows, Columns, T_Delegate);
            _Matrix = tmp;
            Transposed = new(ref tmp);
        }


        public BaseMatrix(int Rows, int Columns, Func<int, T> T_DelegateUsingRowIndex)
        {
            var tmp = ArrayFactory.Create(Rows, Columns, T_DelegateUsingRowIndex);
            _Matrix = tmp;
            Transposed = new(ref tmp);
        }


        public BaseMatrix(int Rows, int Columns, Func<int, int, T> T_DelegateUsingIndices)
        {
            var tmp = ArrayFactory.Create(Rows, Columns, T_DelegateUsingIndices);
            _Matrix = tmp;
            Transposed = new(ref tmp);
        }


        public BaseMatrix(int Rows, int Columns, T DefaultValue)
        {
            var tmp = ArrayFactory.Create(Rows, Columns, DefaultValue);
            _Matrix = tmp;
            Transposed = new(ref tmp);
        }


        public BaseMatrix(int Rows, int Columns, IEnumerable<T> Enumerable, int? MaxEnumeration = null)
        {
            var tmp = ArrayFactory.Create(Rows, Columns, Enumerable, MaxEnumeration);
            _Matrix = tmp;
            Transposed = new(ref tmp);
        }

        public BaseMatrix(int Rows, int Columns, Span<T> Values, bool AsEnumerable)
        {
            var tmp = AsEnumerable ? ArrayFactory.Create(Rows, Columns, Values) : ArrayFactory.Create(Rows, Columns, Values);
            _Matrix = tmp;
            Transposed = new(ref tmp);
        }

        protected BaseMatrix() { }

        public void PerformMemberWise(MatrixOperations<T>.SingleElementOperation<T> Operation) => MatrixOperations<T>.PerformMutableOperation(this, Operation);

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
        /// Creates a clone of this object.
        /// <code>
        /// Avoids boxing the object
        /// </code>
        /// <code>Complexity: O(n)</code>
        /// </summary>
        /// <returns>
        /// <see cref="BaseMatrix{T}"/>
        /// </returns>
        public virtual IMatrix<T> Duplicate()
        {
            T[][] ClonedMatrix = new T[Rows][];

            for (int row = 0; row < Rows; row++)
            {
                ClonedMatrix[row] = new T[Columns];
                Array.Copy(_Matrix[row], Columns * 0, ClonedMatrix[row], 0, Columns);
            }

            var newMatrix = new BaseMatrix<T>()
            {
                _Matrix = ClonedMatrix,
                SByteOperations = this.SByteOperations,
                ByteOperations = this.ByteOperations,
                ShortOperations = this.ShortOperations,
                IntegerOperations = this.IntegerOperations,
                LongOperations = this.LongOperations,
                FloatOperations = this.FloatOperations,
                DoubleOperations = this.DoubleOperations,
                DecimalOperations = this.DecimalOperations,
                SameTypedOperations = this.SameTypedOperations,
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