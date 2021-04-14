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

        OperationSet<T, decimal> DecimalOperations { get; set; }

        OperationSet<T, double> DoubleOperations { get; set; }

        OperationSet<T, float> FloatOperations { get; set; }

        OperationSet<T, int> IntegerOperations { get; set; }

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

        void PerformMemberWise(MatrixOperations.SingleElementOperation<T> Operation);

        /// <summary>
        /// Inherited from <typeparamref name="T[]"/>
        /// </summary>
        bool Remove(T[] item);

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
        public static IMatrix<T> operator *(IMatrix<T> matrix, int Scalar)
        {
            matrix.PerformMemberWise(matrix.IntegerOperations.ReferenceMultiplier(Scalar));
            return matrix;
        }

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
        public static IMatrix<T> operator +(int Value, IMatrix<T> matrix)
        {
            matrix.PerformMemberWise(matrix.IntegerOperations.ReferenceAdder(Value));
            return matrix;
        }

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
        public static IMatrix<T> operator -(int Value, IMatrix<T> Matrix)
        {
            Matrix.PerformMemberWise(Matrix.IntegerOperations.ReferenceAdder(-Value));
            return Matrix;
        }

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
        public static IMatrix<T> operator -(IMatrix<T> Matrix, int Value) => Value - Matrix;

        /// <summary>
        /// Performs an in-place memberwise addition of the left and right Matrices. The left matrix is modified in-place. Right matrix is not mutated.
        /// <code>
        /// Complexit: 
        /// </code>
        /// </summary>
        /// <param name="Left"></param>
        /// <param name="Right"></param>
        /// <returns></returns>
        public static IMatrix<T> operator +(IMatrix<T> Left, IMatrix<T> Right)
        {
            // if both matrices are the same shape we can perform member-wise addition and subtraction
            if (Left.Columns == Right.Columns && Left.Rows == Right.Rows)
            {
                MatrixOperations.PerformTwoSidedOperationMemberwise<T>(Left, Right, Left.SameTypedOperations.TwoRefenceAdder);
            }
            else
            {
                // since the two matrices are different sizes we must perform matrix addition
                throw new NotImplementedException();
            }

            return Left;
        }

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
        public static IMatrix<T> operator *(IMatrix<T> Left, IMatrix<T> Right)
        {
            // if both matrices are the same shape we can perform member-wise addition and subtraction
            if (Left.Columns == Right.Columns && Left.Rows == Right.Rows)
            {
                MatrixOperations.PerformTwoSidedOperationMemberwise(Left, Right, Left.SameTypedOperations.TwoRefenceAdder);
            }
            else if (Left.Rows == Right.Columns || Left.Columns == Right.Rows)
            {
                //verify that the two matrices are able to be multiplied
                // two matracies can only be multiplied if the number of columns of the first matrix is the same as the number of rows in the second matrix
                Span<T> Result = MatrixOperations.MultiplyMatrices(Left, Right, Left.SameTypedOperations.TwoValueMultiplier, Left.SameTypedOperations.TwoRefenceAdder);

                // multiplied matrices are always square
                return new BaseMatrix<T>(Left.Rows, Right.Columns, Result, false);
            }
            else
            {
                throw Exceptions.IncompatibleMatrixShapeForMultiplication(Left.Rows, Right.Columns);
            }

            return Left;
        }

        /// <summary>
        /// Performs an in-place memberwise subtraction of the left and right Matrices. The left matrix is modified in-place. Right matrix is not mutated.
        /// <code>
        /// Complexit: 
        /// </code>
        /// </summary>
        /// <param name="Left"></param>
        /// <param name="Right"></param>
        /// <returns></returns>
        public static IMatrix<T> operator -(IMatrix<T> Left, IMatrix<T> Right)
        {
            // if both matrices are the same shape we can perform member-wise addition and subtraction
            if (Left.Columns == Right.Columns && Left.Rows == Right.Rows)
            {
                MatrixOperations.PerformTwoSidedOperationMemberwise(Left, Right, Left.SameTypedOperations.TwoRefenceSubtractor);
            }
            else
            {
                // since the two matrices are different sizes we must perform matrix addition
                throw new NotImplementedException();
            }

            return Left;
        }
    }
}