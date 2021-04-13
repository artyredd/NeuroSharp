using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuroSharp.Int
{
    public class Matrix : BaseMatrix<int>
    {
        /// <summary>
        /// Creates a new int32 Matrix.
        /// <code>
        /// Complexity: O(n)
        /// </code>
        /// <para>
        /// Example:
        /// <c>var matrix = new Matrix(2,2)</c>
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
        /// <see cref="Matrix"/>
        /// </returns>
        public Matrix(int Rows, int Columns) : base(Rows, Columns) { }

        /// <summary>
        /// Instantiates a new Matrix array with the given <paramref name="Rows"/> and <paramref name="Columns"/>. For each new item in all rows <paramref name="T_Provider"/> is invoked to set the value of the int32 in the spot.
        /// <para>
        /// Example Usage:
        /// <code>
        /// var matrix = new Matrix( Rows: 2, Columns: 3, DelegateThatReturnsT: ()=&gt; 1);
        /// </code>
        /// Output:
        /// <code>
        /// Matrix: Row[0] = { 1, 1, 1 }, Row[1] = { 1, 1, 1 }
        /// </code>
        /// </para>
        /// </summary>
        /// <param name="Rows"></param>
        /// <param name="Columns"></param>
        /// <returns>
        /// <see cref="Matrix"/>
        /// </returns>
        public Matrix(int Rows, int Columns, Func<int> T_Delegate) : base(Rows, Columns, T_Delegate) { }

        /// <summary>
        /// Instantiates a new Matrix array with the given <paramref name="Rows"/> and <paramref name="Columns"/>. For each new item in all rows <paramref name="T_ProviderUsingRow"/> is invoked to set the value of the in32 in the spot, this <paramref name="T_ProviderUsingRow"/> is invoked with a <see cref="int"/> Row Index where that element is contained.
        /// <para>
        /// Example Usage(where i = Row Index where the element is located):
        /// <code>
        /// var matrix = new Int.Matrix( Rows: 2, Columns: 3, DelegateThatReturnsTUsingRowNumber: (i)=&gt; i);
        /// </code>
        /// Output:
        /// <code>
        /// Matrix: Row[0] = { 0, 0, 0 }, Row[1] = { 1, 1, 1 }
        /// </code>
        /// </para>
        /// </summary>
        /// <param name="Rows"></param>
        /// <param name="Columns"></param>
        /// <returns>
        /// <see cref="Matrix"/>
        /// </returns>
        public Matrix(int Rows, int Columns, Func<int, int> T_DelegateUsingRowIndex) : base(Rows, Columns, T_DelegateUsingRowIndex) { }

        /// <summary>
        /// Instantiates a new Matrix array with the given <paramref name="Rows"/> and <paramref name="Columns"/>. For each new item in all rows <paramref name="T_ProviderUsingIndexes"/> is invoked to set the value of the int32 in the spot, this <paramref name="T_ProviderUsingIndexes"/> is invoked with a <see cref="int"/> Row Index where that element is contained.
        /// <para>
        /// Example Usage(where x = Row Index where the element is located, and y = Column Index where the element is located):
        /// <code>
        /// var matrix = new Int.Matrix( Rows: 2, Columns: 3, DelegateThatReturnsTUsingRowNumberAndColumnNumber: (x,y)=&gt; x + y);
        /// </code>
        /// Output:
        /// <code>
        /// Matrix: Row[0] = { 0, 1, 2 }, Row[1] = { 1, 2, 3 }
        /// </code>
        /// </para>
        /// </summary>
        /// <param name="Rows"></param>
        /// <param name="Columns"></param>
        /// <returns>
        /// <see cref="Matrix"/>
        /// </returns>
        public Matrix(int Rows, int Columns, Func<int, int, int> T_DelegateUsingIndices) : base(Rows, Columns, T_DelegateUsingIndices) { }

        /// <summary>
        /// Instantiates a new Matrix array with the given <paramref name="Rows"/> and <paramref name="Columns"/>. Each new item in the array is set to the provided <paramref name="DefaultValue"/>
        /// <para>
        /// Example Usage:
        /// <code>
        /// var matrix = new Int.Matrix( Rows: 2, Columns: 3, DefaultValue: 12);
        /// </code>
        /// Output:
        /// <code>
        /// Matrix: Row[0] = { 12, 12, 12 }, Row[1] = { 12, 12, 12 }
        /// </code>
        /// </para>
        /// </summary>
        /// <param name="Rows"></param>
        /// <param name="Columns"></param>
        /// <returns>
        /// <see cref="Matrix"/>
        /// </returns>
        public Matrix(int Rows, int Columns, int DefaultValue) : base(Rows, Columns, DefaultValue) { }

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
        /// <returns>
        /// <see cref="Matrix"/>
        /// </returns>
        public Matrix(int Rows, int Columns, IEnumerable<int> Enumerable) : base(Rows, Columns, Enumerable) { }

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
        public static Matrix operator *(int Scalar, Matrix Matrix)
        {
            Matrix.PerformMemberWise((ref int value) => value *= Scalar);
            return Matrix;
        }

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
        public static Matrix operator *(Matrix Matrix, int Scalar) => Scalar * Matrix;

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
        public static Matrix operator +(int Value, Matrix Matrix)
        {
            Matrix.PerformMemberWise((ref int value) => value += Value);
            return Matrix;
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
        public static Matrix operator +(Matrix Matrix, int Value) => Value + Matrix;

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
        public static Matrix operator -(int Value, Matrix Matrix)
        {
            Matrix.PerformMemberWise((ref int value) => value -= Value);
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
        public static Matrix operator -(Matrix Matrix, int Value) => Value + Matrix;

        /// <summary>
        /// Performs an in-place memberwise addition of the left and right Matrices. The left matrix is modified in-place. Right matrix is not mutated.
        /// <code>
        /// Complexit: 
        /// </code>
        /// </summary>
        /// <param name="Left"></param>
        /// <param name="Right"></param>
        /// <returns></returns>
        public static Matrix operator +(Matrix Left, Matrix Right)
        {
            // if both matrices are the same shape we can perform member-wise addition and subtraction
            if (Left.Columns == Right.Columns && Left.Rows == Right.Rows)
            {
                MatrixOperations.PerformMemberWiseCombinedOperation<int>(Left, Right, (ref int left, ref int right) => left += right);
            }
            else
            {
                // since the two matrices are different sizes we must perform matrix addition
                throw new NotImplementedException();
            }

            return Left;
        }

        [Obsolete("Multiplying an Integer Matrix by a float would cause loss of information on either the matrix or the float due to rounding. Use NeuroSharp.Float.Matrix instead for precise numbers.")]
        public static Matrix operator *(float Scalar, Matrix Matrix)
        {
            throw new InvalidOperationException("Attempted to multiply a matrix that contains integers with a float");
        }

        [Obsolete("Multiplying an Integer Matrix by a float would cause loss of information on either the matrix or the float due to rounding. Use NeuroSharp.Float.Matrix instead for precise numbers.")]
        public static Matrix operator *(Matrix Matrix, float Scalar)
        {
            throw new InvalidOperationException("Attempted to multiply a matrix that contains integers with a float");
        }

        [Obsolete("Multiplying an Integer Matrix by a decimal would cause loss of information on either the matrix or the decimal due to rounding. Use NeuroSharp.Decimal.Matrix instead for precise numbers.")]
        public static Matrix operator *(decimal Scalar, Matrix Matrix)
        {
            throw new InvalidOperationException("Attempted to multiply a matrix that contains integers with a decimal");
        }

        [Obsolete("Multiplying an Integer Matrix by a decimal would cause loss of information on either the matrix or the decimal due to rounding. Use NeuroSharp.Decimal.Matrix instead for precise numbers.")]
        public static Matrix operator *(Matrix Matrix, decimal Scalar)
        {
            throw new InvalidOperationException("Attempted to multiply a matrix that contains integers with a decimal");
        }

        [Obsolete("Multiplying an Integer Matrix by a double would cause loss of information on either the matrix or the double due to rounding. Use NeuroSharp.double.Matrix instead for precise numbers.")]
        public static Matrix operator *(double Scalar, Matrix Matrix)
        {
            throw new InvalidOperationException("Attempted to multiply a matrix that contains integers with a double");
        }

        [Obsolete("Multiplying an Integer Matrix by a double would cause loss of information on either the matrix or the double due to rounding. Use NeuroSharp.double.Matrix instead for precise numbers.")]
        public static Matrix operator *(Matrix Matrix, double Scalar)
        {
            throw new InvalidOperationException("Attempted to multiply a matrix that contains integers with a double.");
        }

        //
        [Obsolete("Adding an Integer Matrix by a float would cause loss of information on either the matrix or the float due to rounding. Use NeuroSharp.Float.Matrix instead for precise numbers.")]
        public static Matrix operator +(float Scalar, Matrix Matrix)
        {
            throw new InvalidOperationException("Attempted to add a matrix that contains integers with a float");
        }

        [Obsolete("Adding an Integer Matrix by a float would cause loss of information on either the matrix or the float due to rounding. Use NeuroSharp.Float.Matrix instead for precise numbers.")]
        public static Matrix operator +(Matrix Matrix, float Scalar)
        {
            throw new InvalidOperationException("Attempted to add a matrix that contains integers with a float");
        }

        [Obsolete("Adding an Integer Matrix by a decimal would cause loss of information on either the matrix or the decimal due to rounding. Use NeuroSharp.Decimal.Matrix instead for precise numbers.")]
        public static Matrix operator +(decimal Scalar, Matrix Matrix)
        {
            throw new InvalidOperationException("Attempted to add a matrix that contains integers with a decimal");
        }

        [Obsolete("Adding an Integer Matrix by a decimal would cause loss of information on either the matrix or the decimal due to rounding. Use NeuroSharp.Decimal.Matrix instead for precise numbers.")]
        public static Matrix operator +(Matrix Matrix, decimal Scalar)
        {
            throw new InvalidOperationException("Attempted to add a matrix that contains integers with a decimal");
        }

        [Obsolete("Adding an Integer Matrix by a double would cause loss of information on either the matrix or the double due to rounding. Use NeuroSharp.double.Matrix instead for precise numbers.")]
        public static Matrix operator +(double Scalar, Matrix Matrix)
        {
            throw new InvalidOperationException("Attempted to add a matrix that contains integers with a double");
        }

        [Obsolete("Adding an Integer Matrix by a double would cause loss of information on either the matrix or the double due to rounding. Use NeuroSharp.double.Matrix instead for precise numbers.")]
        public static Matrix operator +(Matrix Matrix, double Scalar)
        {
            throw new InvalidOperationException("Attempted to add a matrix that contains integers with a double.");
        }

        //
        [Obsolete("Subtracting an Integer Matrix by a float would cause loss of information on either the matrix or the float due to rounding. Use NeuroSharp.Float.Matrix instead for precise numbers.")]
        public static Matrix operator -(float Scalar, Matrix Matrix)
        {
            throw new InvalidOperationException("Attempted to subtract a matrix that contains integers with a float");
        }

        [Obsolete("Subtracting an Integer Matrix by a float would cause loss of information on either the matrix or the float due to rounding. Use NeuroSharp.Float.Matrix instead for precise numbers.")]
        public static Matrix operator -(Matrix Matrix, float Scalar)
        {
            throw new InvalidOperationException("Attempted to subtract a matrix that contains integers with a float");
        }

        [Obsolete("Subtracting an Integer Matrix by a decimal would cause loss of information on either the matrix or the decimal due to rounding. Use NeuroSharp.Decimal.Matrix instead for precise numbers.")]
        public static Matrix operator -(decimal Scalar, Matrix Matrix)
        {
            throw new InvalidOperationException("Attempted to subtract a matrix that contains integers with a decimal");
        }

        [Obsolete("Subtracting an Integer Matrix by a decimal would cause loss of information on either the matrix or the decimal due to rounding. Use NeuroSharp.Decimal.Matrix instead for precise numbers.")]
        public static Matrix operator -(Matrix Matrix, decimal Scalar)
        {
            throw new InvalidOperationException("Attempted to subtract a matrix that contains integers with a decimal");
        }

        [Obsolete("Subtracting an Integer Matrix by a double would cause loss of information on either the matrix or the double due to rounding. Use NeuroSharp.double.Matrix instead for precise numbers.")]
        public static Matrix operator -(double Scalar, Matrix Matrix)
        {
            throw new InvalidOperationException("Attempted to subtract a matrix that contains integers with a double");
        }

        [Obsolete("Subtracting an Integer Matrix by a double would cause loss of information on either the matrix or the double due to rounding. Use NeuroSharp.double.Matrix instead for precise numbers.")]
        public static Matrix operator -(Matrix Matrix, double Scalar)
        {
            throw new InvalidOperationException("Attempted to subtract a matrix that contains integers with a double.");
        }
    }
}
