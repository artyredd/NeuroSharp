using System;
using System.Collections;
using System.Collections.Generic;

namespace NeuroSharp
{
    public interface IMatrix<T> where T : unmanaged, IComparable<T>, IEquatable<T>
    {
        ref T[] this[int _RowIndex] { get; }

        ref T this[int RowIndex, int ColumnIndex] { get; }

        T[][] _Matrix { get; init; }

        /// <summary>
        /// Gets the total number of items that can be contained in this matrix. Rows * Columns
        /// </summary>
        int Capacity { get; }

        /// <summary>
        /// Gets the number of columns in this matrix
        /// </summary>
        int Columns { get; }

        /// <summary>
        /// Gets the number of rows in this matrix
        /// </summary>
        int Rows { get; }

        /// <summary>
        /// Gets the total elements in the matrix.
        /// </summary>
        int Count { get; }

        OperationSet<T, int> IntegerOperations { get; set; }
        OperationSet<T, double> DoubleOperations { get; set; }
        OperationSet<T, float> FloatOperations { get; set; }
        OperationSet<T, decimal> DecimalOperations { get; set; }
        OperationSet<T, long> LongOperations { get; set; }
        OperationSet<T, short> ShortOperations { get; set; }
        OperationSet<T, byte> ByteOperations { get; set; }
        OperationSet<T, sbyte> SByteOperations { get; set; }
        OperationSet<T, T> SameTypedOperations { get; set; }

        /// <summary>
        /// Inherited from <typeparamref name="T[]"/>
        /// </summary>
        bool IsFixedSize { get; }

        /// <summary>
        /// Inherited from <typeparamref name="T[]"/>
        /// </summary>
        bool IsReadOnly { get; }

        /// <summary>
        /// Inherited from <typeparamref name="T[]"/>
        /// </summary>
        bool IsSynchronized { get; }

        /// <summary>
        /// Inherited from <typeparamref name="T[]"/>
        /// </summary>
        object SyncRoot { get; }

        /// <summary>
        /// Gets transposed version of this matrix, the transposed version is accessed through index mapping and is not truly transposed. Use <see cref="Extensions.Transpose{T}(IMatrix{T})"/> to convert this matrix to a truly transposed matrix.
        /// </summary>
        TransposedMatrix<T> Transposed { get; init; }

        /// <summary>
        /// Inherited from <typeparamref name="T[]"/>
        /// </summary>
        void Add(T[] item);

        /// <summary>
        /// Inherited from <typeparamref name="T[]"/>
        /// </summary>
        void Clear();

        /// <summary>
        /// Returns a boxed clone of this object.
        /// </summary>
        /// <returns></returns>
        object Clone();

        /// <summary>
        /// Inherited from <typeparamref name="T[]"/>
        /// </summary>
        int CompareTo(object other, IComparer comparer);

        /// <summary>
        /// Inherited from <typeparamref name="T[]"/>
        /// </summary>
        bool Contains(T[] item);

        /// <summary>
        /// Inherited from <typeparamref name="T[]"/>
        /// </summary>
        void CopyTo(T[][] array, int arrayIndex);

        /// <summary>
        /// Returns a non-boxed duplication of this object.
        /// </summary>
        /// <returns></returns>
        IMatrix<T> Duplicate();

        /// <summary>
        /// Inherited from <typeparamref name="T[]"/>
        /// </summary>
        bool Equals(object other, IEqualityComparer comparer);

        /// <summary>
        /// Inherited from <typeparamref name="T[]"/>
        /// </summary>
        IEnumerator<T[]> GetEnumerator();

        /// <summary>
        /// Inherited from <typeparamref name="T[]"/>
        /// </summary>
        int GetHashCode(IEqualityComparer comparer);

        /// <summary>
        /// Invokes the provided operation member-wise on this matrix (on each element)
        /// <code>
        /// Complexity: O(n)
        /// </code>
        /// <para>
        /// Elements are mutated in-place using <c>ref</c>
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
        void PerformMemberWise(MatrixOperations<T>.SingleElementOperation<T> Operation);

        /// <summary>
        /// Inherited from <typeparamref name="T[]"/>
        /// </summary>
        bool Remove(T[] item);

        #region sbyte operators
        /// <summary>
        /// Performs a memberwise-scalar operation on the entire matrix
        /// <code>
        /// Complexity: O(n)
        /// </code>
        /// </summary>
        /// <param name="Scalar"></param>
        /// <param name="Matrix"></param>
        /// <returns>
        /// Reference to the original modified matrix <see cref="Matrix"/>
        /// </returns>
        public static IMatrix<T> operator *(sbyte Scalar, IMatrix<T> Matrix) => MatrixOperations<T>.Operators.Multiply(Scalar, Matrix);

        /// <summary>
        /// Performs a memberwise-scalar operation on the entire matrix
        /// <code>
        /// Complexity: O(n)
        /// </code>
        /// </summary>
        /// <param name="Scalar"></param>
        /// <param name="Matrix"></param>
        /// <returns>
        /// Reference to the original modified matrix <see cref="Matrix"/>
        /// </returns>
        public static IMatrix<T> operator *(IMatrix<T> Matrix, sbyte Scalar) => Scalar * Matrix;

        /// <summary>
        /// Performs a memberwise addition operation on the entire matrix member-wise
        /// <code>
        /// Complexity: O(n)
        /// </code>
        /// </summary>
        /// <param name="Value"></param>
        /// <param name="Matrix"></param>
        /// <returns>
        /// Reference to the original modified matrix <see cref="Matrix"/>
        /// </returns>
        public static IMatrix<T> operator +(sbyte Value, IMatrix<T> Matrix) => MatrixOperations<T>.Operators.Add(Value, Matrix);

        /// <summary>
        /// Performs a memberwise addition operation on the entire matrix member-wise
        /// <code>
        /// Complexity: O(n)
        /// </code>
        /// </summary>
        /// <param name="Value"></param>
        /// <param name="Matrix"></param>
        /// <returns>
        /// Reference to the original modified matrix <see cref="Matrix"/>
        /// </returns>
        public static IMatrix<T> operator +(IMatrix<T> Matrix, sbyte Value) => Value + Matrix;

        /// <summary>
        /// Performs a memberwise subtraction operation on the entire matrix member-wise
        /// <code>
        /// Complexity: O(n)
        /// </code>
        /// </summary>
        /// <param name="Value"></param>
        /// <param name="Matrix"></param>
        /// <returns>
        /// Reference to the original modified matrix <see cref="Matrix"/>
        /// </returns>
        public static IMatrix<T> operator -(sbyte Value, IMatrix<T> Matrix) => MatrixOperations<T>.Operators.Subtract(Value, Matrix);

        /// <summary>
        /// Performs a memberwise subtraction operation on the entire matrix member-wise
        /// <code>
        /// This does not mutate either side.
        /// </code>
        /// <code>
        /// Complexity: O(n)
        /// </code>
        /// </summary>
        /// <param name="Value"></param>
        /// <param name="Matrix"></param>
        /// <returns>
        /// <see cref="IMatrix{T}"/> New Matrix
        /// </returns>
        public static IMatrix<T> operator -(IMatrix<T> Matrix, sbyte Value) => Value - Matrix;
        #endregion

        #region byte operators
        /// <summary>
        /// Performs a memberwise-scalar operation on the entire matrix
        /// <code>
        /// Complexity: O(n)
        /// </code>
        /// </summary>
        /// <param name="Scalar"></param>
        /// <param name="Matrix"></param>
        /// <returns>
        /// Reference to the original modified matrix <see cref="Matrix"/>
        /// </returns>
        public static IMatrix<T> operator *(byte Scalar, IMatrix<T> Matrix) => MatrixOperations<T>.Operators.Multiply(Scalar, Matrix);

        /// <summary>
        /// Performs a memberwise-scalar operation on the entire matrix
        /// <code>
        /// Complexity: O(n)
        /// </code>
        /// </summary>
        /// <param name="Scalar"></param>
        /// <param name="Matrix"></param>
        /// <returns>
        /// Reference to the original modified matrix <see cref="Matrix"/>
        /// </returns>
        public static IMatrix<T> operator *(IMatrix<T> Matrix, byte Scalar) => Scalar * Matrix;

        /// <summary>
        /// Performs a memberwise addition operation on the entire matrix member-wise
        /// <code>
        /// Complexity: O(n)
        /// </code>
        /// </summary>
        /// <param name="Value"></param>
        /// <param name="Matrix"></param>
        /// <returns>
        /// Reference to the original modified matrix <see cref="Matrix"/>
        /// </returns>
        public static IMatrix<T> operator +(byte Value, IMatrix<T> Matrix) => MatrixOperations<T>.Operators.Add(Value, Matrix);

        /// <summary>
        /// Performs a memberwise addition operation on the entire matrix member-wise
        /// <code>
        /// Complexity: O(n)
        /// </code>
        /// </summary>
        /// <param name="Value"></param>
        /// <param name="Matrix"></param>
        /// <returns>
        /// Reference to the original modified matrix <see cref="Matrix"/>
        /// </returns>
        public static IMatrix<T> operator +(IMatrix<T> Matrix, byte Value) => Value + Matrix;

        /// <summary>
        /// Performs a memberwise subtraction operation on the entire matrix member-wise
        /// <code>
        /// Complexity: O(n)
        /// </code>
        /// </summary>
        /// <param name="Value"></param>
        /// <param name="Matrix"></param>
        /// <returns>
        /// Reference to the original modified matrix <see cref="Matrix"/>
        /// </returns>
        public static IMatrix<T> operator -(byte Value, IMatrix<T> Matrix) => MatrixOperations<T>.Operators.Subtract(Value, Matrix);

        /// <summary>
        /// Performs a memberwise subtraction operation on the entire matrix member-wise
        /// <code>
        /// This does not mutate either side.
        /// </code>
        /// <code>
        /// Complexity: O(n)
        /// </code>
        /// </summary>
        /// <param name="Value"></param>
        /// <param name="Matrix"></param>
        /// <returns>
        /// <see cref="IMatrix{T}"/> New Matrix
        /// </returns>
        public static IMatrix<T> operator -(IMatrix<T> Matrix, byte Value) => Value - Matrix;
        #endregion

        #region int operators
        /// <summary>
        /// Performs a memberwise-scalar operation on the entire matrix
        /// <code>
        /// Complexity: O(n)
        /// </code>
        /// </summary>
        /// <param name="Scalar"></param>
        /// <param name="Matrix"></param>
        /// <returns>
        /// Reference to the original modified matrix <see cref="Matrix"/>
        /// </returns>
        public static IMatrix<T> operator *(int Scalar, IMatrix<T> Matrix) => MatrixOperations<T>.Operators.Multiply(Scalar, Matrix);

        /// <summary>
        /// Performs a memberwise-scalar operation on the entire matrix
        /// <code>
        /// Complexity: O(n)
        /// </code>
        /// </summary>
        /// <param name="Scalar"></param>
        /// <param name="Matrix"></param>
        /// <returns>
        /// Reference to the original modified matrix <see cref="Matrix"/>
        /// </returns>
        public static IMatrix<T> operator *(IMatrix<T> Matrix, int Scalar) => Scalar * Matrix;

        /// <summary>
        /// Performs a memberwise addition operation on the entire matrix member-wise
        /// <code>
        /// Complexity: O(n)
        /// </code>
        /// </summary>
        /// <param name="Value"></param>
        /// <param name="Matrix"></param>
        /// <returns>
        /// Reference to the original modified matrix <see cref="Matrix"/>
        /// </returns>
        public static IMatrix<T> operator +(int Value, IMatrix<T> Matrix) => MatrixOperations<T>.Operators.Add(Value, Matrix);

        /// <summary>
        /// Performs a memberwise addition operation on the entire matrix member-wise
        /// <code>
        /// Complexity: O(n)
        /// </code>
        /// </summary>
        /// <param name="Value"></param>
        /// <param name="Matrix"></param>
        /// <returns>
        /// Reference to the original modified matrix <see cref="Matrix"/>
        /// </returns>
        public static IMatrix<T> operator +(IMatrix<T> Matrix, int Value) => Value + Matrix;

        /// <summary>
        /// Performs a memberwise subtraction operation on the entire matrix member-wise
        /// <code>
        /// Complexity: O(n)
        /// </code>
        /// </summary>
        /// <param name="Value"></param>
        /// <param name="Matrix"></param>
        /// <returns>
        /// Reference to the original modified matrix <see cref="Matrix"/>
        /// </returns>
        public static IMatrix<T> operator -(int Value, IMatrix<T> Matrix) => MatrixOperations<T>.Operators.Subtract(Value, Matrix);

        /// <summary>
        /// Performs a memberwise subtraction operation on the entire matrix member-wise
        /// <code>
        /// This does not mutate either side.
        /// </code>
        /// <code>
        /// Complexity: O(n)
        /// </code>
        /// </summary>
        /// <param name="Value"></param>
        /// <param name="Matrix"></param>
        /// <returns>
        /// <see cref="IMatrix{T}"/> New Matrix
        /// </returns>
        public static IMatrix<T> operator -(IMatrix<T> Matrix, int Value) => Value - Matrix;
        #endregion

        #region long operators
        /// <summary>
        /// Performs a memberwise-scalar operation on the entire matrix
        /// <code>
        /// Complexity: O(n)
        /// </code>
        /// </summary>
        /// <param name="Scalar"></param>
        /// <param name="Matrix"></param>
        /// <returns>
        /// Reference to the original modified matrix <see cref="Matrix"/>
        /// </returns>
        public static IMatrix<T> operator *(long Scalar, IMatrix<T> Matrix) => MatrixOperations<T>.Operators.Multiply(Scalar, Matrix);

        /// <summary>
        /// Performs a memberwise-scalar operation on the entire matrix
        /// <code>
        /// Complexity: O(n)
        /// </code>
        /// </summary>
        /// <param name="Scalar"></param>
        /// <param name="Matrix"></param>
        /// <returns>
        /// Reference to the original modified matrix <see cref="Matrix"/>
        /// </returns>
        public static IMatrix<T> operator *(IMatrix<T> Matrix, long Scalar) => Scalar * Matrix;

        /// <summary>
        /// Performs a memberwise addition operation on the entire matrix member-wise
        /// <code>
        /// Complexity: O(n)
        /// </code>
        /// </summary>
        /// <param name="Value"></param>
        /// <param name="Matrix"></param>
        /// <returns>
        /// Reference to the original modified matrix <see cref="Matrix"/>
        /// </returns>
        public static IMatrix<T> operator +(long Value, IMatrix<T> Matrix) => MatrixOperations<T>.Operators.Add(Value, Matrix);

        /// <summary>
        /// Performs a memberwise addition operation on the entire matrix member-wise
        /// <code>
        /// Complexity: O(n)
        /// </code>
        /// </summary>
        /// <param name="Value"></param>
        /// <param name="Matrix"></param>
        /// <returns>
        /// Reference to the original modified matrix <see cref="Matrix"/>
        /// </returns>
        public static IMatrix<T> operator +(IMatrix<T> Matrix, long Value) => Value + Matrix;

        /// <summary>
        /// Performs a memberwise subtraction operation on the entire matrix member-wise
        /// <code>
        /// Complexity: O(n)
        /// </code>
        /// </summary>
        /// <param name="Value"></param>
        /// <param name="Matrix"></param>
        /// <returns>
        /// Reference to the original modified matrix <see cref="Matrix"/>
        /// </returns>
        public static IMatrix<T> operator -(long Value, IMatrix<T> Matrix) => MatrixOperations<T>.Operators.Subtract(Value, Matrix);

        /// <summary>
        /// Performs a memberwise subtraction operation on the entire matrix member-wise
        /// <code>
        /// This does not mutate either side.
        /// </code>
        /// <code>
        /// Complexity: O(n)
        /// </code>
        /// </summary>
        /// <param name="Value"></param>
        /// <param name="Matrix"></param>
        /// <returns>
        /// <see cref="IMatrix{T}"/> New Matrix
        /// </returns>
        public static IMatrix<T> operator -(IMatrix<T> Matrix, long Value) => Value - Matrix;
        #endregion

        #region double operators
        /// <summary>
        /// Performs a memberwise-scalar operation on the entire matrix
        /// <code>
        /// Complexity: O(n)
        /// </code>
        /// </summary>
        /// <param name="Scalar"></param>
        /// <param name="Matrix"></param>
        /// <returns>
        /// Reference to the original modified matrix <see cref="Matrix"/>
        /// </returns>
        public static IMatrix<T> operator *(double Scalar, IMatrix<T> Matrix) => MatrixOperations<T>.Operators.Multiply(Scalar, Matrix);

        /// <summary>
        /// Performs a memberwise-scalar operation on the entire matrix
        /// <code>
        /// Complexity: O(n)
        /// </code>
        /// </summary>
        /// <param name="Scalar"></param>
        /// <param name="Matrix"></param>
        /// <returns>
        /// Reference to the original modified matrix <see cref="Matrix"/>
        /// </returns>
        public static IMatrix<T> operator *(IMatrix<T> Matrix, double Scalar) => Scalar * Matrix;

        /// <summary>
        /// Performs a memberwise addition operation on the entire matrix member-wise
        /// <code>
        /// Complexity: O(n)
        /// </code>
        /// </summary>
        /// <param name="Value"></param>
        /// <param name="Matrix"></param>
        /// <returns>
        /// Reference to the original modified matrix <see cref="Matrix"/>
        /// </returns>
        public static IMatrix<T> operator +(double Value, IMatrix<T> Matrix) => MatrixOperations<T>.Operators.Add(Value, Matrix);

        /// <summary>
        /// Performs a memberwise addition operation on the entire matrix member-wise
        /// <code>
        /// Complexity: O(n)
        /// </code>
        /// </summary>
        /// <param name="Value"></param>
        /// <param name="Matrix"></param>
        /// <returns>
        /// Reference to the original modified matrix <see cref="Matrix"/>
        /// </returns>
        public static IMatrix<T> operator +(IMatrix<T> Matrix, double Value) => Value + Matrix;

        /// <summary>
        /// Performs a memberwise subtraction operation on the entire matrix member-wise
        /// <code>
        /// Complexity: O(n)
        /// </code>
        /// </summary>
        /// <param name="Value"></param>
        /// <param name="Matrix"></param>
        /// <returns>
        /// Reference to the original modified matrix <see cref="Matrix"/>
        /// </returns>
        public static IMatrix<T> operator -(double Value, IMatrix<T> Matrix) => MatrixOperations<T>.Operators.Subtract(Value, Matrix);

        /// <summary>
        /// Performs a memberwise subtraction operation on the entire matrix member-wise
        /// <code>
        /// This does not mutate either side.
        /// </code>
        /// <code>
        /// Complexity: O(n)
        /// </code>
        /// </summary>
        /// <param name="Value"></param>
        /// <param name="Matrix"></param>
        /// <returns>
        /// <see cref="IMatrix{T}"/> New Matrix
        /// </returns>
        public static IMatrix<T> operator -(IMatrix<T> Matrix, double Value) => Value - Matrix;
        #endregion

        #region float operators
        /// <summary>
        /// Performs a memberwise-scalar operation on the entire matrix
        /// <code>
        /// Complexity: O(n)
        /// </code>
        /// </summary>
        /// <param name="Scalar"></param>
        /// <param name="Matrix"></param>
        /// <returns>
        /// Reference to the original modified matrix <see cref="Matrix"/>
        /// </returns>
        public static IMatrix<T> operator *(float Scalar, IMatrix<T> Matrix) => MatrixOperations<T>.Operators.Multiply(Scalar, Matrix);

        /// <summary>
        /// Performs a memberwise-scalar operation on the entire matrix
        /// <code>
        /// Complexity: O(n)
        /// </code>
        /// </summary>
        /// <param name="Scalar"></param>
        /// <param name="Matrix"></param>
        /// <returns>
        /// Reference to the original modified matrix <see cref="Matrix"/>
        /// </returns>
        public static IMatrix<T> operator *(IMatrix<T> Matrix, float Scalar) => Scalar * Matrix;

        /// <summary>
        /// Performs a memberwise addition operation on the entire matrix member-wise
        /// <code>
        /// Complexity: O(n)
        /// </code>
        /// </summary>
        /// <param name="Value"></param>
        /// <param name="Matrix"></param>
        /// <returns>
        /// Reference to the original modified matrix <see cref="Matrix"/>
        /// </returns>
        public static IMatrix<T> operator +(float Value, IMatrix<T> Matrix) => MatrixOperations<T>.Operators.Add(Value, Matrix);

        /// <summary>
        /// Performs a memberwise addition operation on the entire matrix member-wise
        /// <code>
        /// Complexity: O(n)
        /// </code>
        /// </summary>
        /// <param name="Value"></param>
        /// <param name="Matrix"></param>
        /// <returns>
        /// Reference to the original modified matrix <see cref="Matrix"/>
        /// </returns>
        public static IMatrix<T> operator +(IMatrix<T> Matrix, float Value) => Value + Matrix;

        /// <summary>
        /// Performs a memberwise subtraction operation on the entire matrix member-wise
        /// <code>
        /// Complexity: O(n)
        /// </code>
        /// </summary>
        /// <param name="Value"></param>
        /// <param name="Matrix"></param>
        /// <returns>
        /// Reference to the original modified matrix <see cref="Matrix"/>
        /// </returns>
        public static IMatrix<T> operator -(float Value, IMatrix<T> Matrix) => MatrixOperations<T>.Operators.Subtract(Value, Matrix);

        /// <summary>
        /// Performs a memberwise subtraction operation on the entire matrix member-wise
        /// <code>
        /// This does not mutate either side.
        /// </code>
        /// <code>
        /// Complexity: O(n)
        /// </code>
        /// </summary>
        /// <param name="Value"></param>
        /// <param name="Matrix"></param>
        /// <returns>
        /// <see cref="IMatrix{T}"/> New Matrix
        /// </returns>
        public static IMatrix<T> operator -(IMatrix<T> Matrix, float Value) => Value - Matrix;
        #endregion

        #region decimal operators
        /// <summary>
        /// Performs a memberwise-scalar operation on the entire matrix
        /// <code>
        /// Complexity: O(n)
        /// </code>
        /// </summary>
        /// <param name="Scalar"></param>
        /// <param name="Matrix"></param>
        /// <returns>
        /// Reference to the original modified matrix <see cref="Matrix"/>
        /// </returns>
        public static IMatrix<T> operator *(decimal Scalar, IMatrix<T> Matrix) => MatrixOperations<T>.Operators.Multiply(Scalar, Matrix);

        /// <summary>
        /// Performs a memberwise-scalar operation on the entire matrix
        /// <code>
        /// Complexity: O(n)
        /// </code>
        /// </summary>
        /// <param name="Scalar"></param>
        /// <param name="Matrix"></param>
        /// <returns>
        /// Reference to the original modified matrix <see cref="Matrix"/>
        /// </returns>
        public static IMatrix<T> operator *(IMatrix<T> Matrix, decimal Scalar) => Scalar * Matrix;

        /// <summary>
        /// Performs a memberwise addition operation on the entire matrix member-wise
        /// <code>
        /// Complexity: O(n)
        /// </code>
        /// </summary>
        /// <param name="Value"></param>
        /// <param name="Matrix"></param>
        /// <returns>
        /// Reference to the original modified matrix <see cref="Matrix"/>
        /// </returns>
        public static IMatrix<T> operator +(decimal Value, IMatrix<T> Matrix) => MatrixOperations<T>.Operators.Add(Value, Matrix);

        /// <summary>
        /// Performs a memberwise addition operation on the entire matrix member-wise
        /// <code>
        /// Complexity: O(n)
        /// </code>
        /// </summary>
        /// <param name="Value"></param>
        /// <param name="Matrix"></param>
        /// <returns>
        /// Reference to the original modified matrix <see cref="Matrix"/>
        /// </returns>
        public static IMatrix<T> operator +(IMatrix<T> Matrix, decimal Value) => Value + Matrix;

        /// <summary>
        /// Performs a memberwise subtraction operation on the entire matrix member-wise
        /// <code>
        /// Complexity: O(n)
        /// </code>
        /// </summary>
        /// <param name="Value"></param>
        /// <param name="Matrix"></param>
        /// <returns>
        /// Reference to the original modified matrix <see cref="Matrix"/>
        /// </returns>
        public static IMatrix<T> operator -(decimal Value, IMatrix<T> Matrix) => MatrixOperations<T>.Operators.Subtract(Value, Matrix);

        /// <summary>
        /// Performs a memberwise subtraction operation on the entire matrix member-wise
        /// <code>
        /// This does not mutate either side.
        /// </code>
        /// <code>
        /// Complexity: O(n)
        /// </code>
        /// </summary>
        /// <param name="Value"></param>
        /// <param name="Matrix"></param>
        /// <returns>
        /// <see cref="IMatrix{T}"/> New Matrix
        /// </returns>
        public static IMatrix<T> operator -(IMatrix<T> Matrix, decimal Value) => Value - Matrix;
        #endregion

        #region Matrix Operators
        /// <summary>
        /// Performs an in-place memberwise addition of the left and right Matrices. The left matrix is modified in-place. Right matrix is not mutated.
        /// <code>
        /// Complexity: O(n) 
        /// </code>
        /// </summary>
        /// <param name="Left"></param>
        /// <param name="Right"></param>
        /// <returns></returns>
        public static IMatrix<T> operator +(IMatrix<T> Left, IMatrix<T> Right) => MatrixOperations<T>.Operators.Add(Left, Right);

        /// <summary>
        /// Performs an in-place memberwise subtraction of the left and right Matrices. The left matrix is modified in-place. Right matrix is not mutated.
        /// <code>
        /// Complexity: O(n) 
        /// </code>
        /// </summary>
        /// <param name="Left"></param>
        /// <param name="Right"></param>
        /// <returns></returns>
        public static IMatrix<T> operator -(IMatrix<T> Left, IMatrix<T> Right) => MatrixOperations<T>.Operators.Subtract(Left, Right);

        /// <summary>
        /// Multiplies two congruent matrices member-wise. (Hadamard product)
        /// <code>
        /// Complexity: O(n)
        /// </code>
        /// </summary>
        /// <param name="Left"></param>
        /// <param name="Right"></param>
        /// <returns>
        /// <see cref="IMatrix{T}"/> Hadamard product of the matrices
        /// </returns>
        public static IMatrix<T> operator %(IMatrix<T> Left, IMatrix<T> Right) => MatrixOperations<T>.Operators.Modulo(Left, Right);

        /// <summary>
        /// Computes the matrix products of two matrices. 
        /// <code>
        /// The number of columns in the first matrix must be equal to the number of rows in the second matrix
        /// </code>
        /// <code>
        /// Complexity: ~ O(n³)ish
        /// </code>
        /// Example:
        /// <code>
        /// 0 1 2 3
        /// </code>
        /// <code>
        /// 4 5 6 7
        /// </code>
        /// <code>
        /// ×
        /// </code>
        /// <code>
        /// 0 1
        /// </code>
        /// <code>
        /// 2 3
        /// </code>
        /// <code>
        /// 4 5
        /// </code>
        /// <code>
        /// 6 7
        /// </code>
        /// Outputs:
        /// <code>
        /// 28 34
        /// </code>
        /// <code>
        /// 76 98
        /// </code>
        /// </summary>
        /// <param name="Left"></param>
        /// <param name="Right"></param>
        /// <returns></returns>
        public static IMatrix<T> operator *(IMatrix<T> Left, IMatrix<T> Right) => MatrixOperations<T>.Operators.Multiply(Left, Right);
        #endregion
    }
}