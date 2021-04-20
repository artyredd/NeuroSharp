using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuroSharp.Decimal
{
    public class Matrix : BaseMatrix<decimal>, IMatrix<decimal>
    {
        /// <summary>
        /// Creates a new float32 Matrix.
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
        public Matrix(int Rows, int Columns) : base(Rows, Columns) => AssignOperations();

        /// <summary>
        /// Instantiates a new Matrix array with the given <paramref name="Rows"/> and <paramref name="Columns"/>. For each new item in all rows <paramref name="T_Provider"/> is invoked to set the value of the float32 in the spot.
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
        public Matrix(int Rows, int Columns, Func<decimal> T_Delegate) : base(Rows, Columns, T_Delegate) => AssignOperations();

        /// <summary>
        /// Instantiates a new Matrix array with the given <paramref name="Rows"/> and <paramref name="Columns"/>. For each new item in all rows <paramref name="T_ProviderUsingRow"/> is invoked to set the value of the in32 in the spot, this <paramref name="T_ProviderUsingRow"/> is invoked with a <see cref="float"/> Row Index where that element is contained.
        /// <para>
        /// Example Usage(where i = Row Index where the element is located):
        /// <code>
        /// var matrix = new float.Matrix( Rows: 2, Columns: 3, DelegateThatReturnsTUsingRowNumber: (i)=&gt; i);
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
        public Matrix(int Rows, int Columns, Func<int, decimal> T_DelegateUsingRowIndex) : base(Rows, Columns, T_DelegateUsingRowIndex) => AssignOperations();

        /// <summary>
        /// Instantiates a new Matrix array with the given <paramref name="Rows"/> and <paramref name="Columns"/>. For each new item in all rows <paramref name="T_ProviderUsingIndexes"/> is invoked to set the value of the float32 in the spot, this <paramref name="T_ProviderUsingIndexes"/> is invoked with a <see cref="float"/> Row Index where that element is contained.
        /// <para>
        /// Example Usage(where x = Row Index where the element is located, and y = Column Index where the element is located):
        /// <code>
        /// var matrix = new float.Matrix( Rows: 2, Columns: 3, DelegateThatReturnsTUsingRowNumberAndColumnNumber: (x,y)=&gt; x + y);
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
        public Matrix(int Rows, int Columns, Func<int, int, decimal> T_DelegateUsingIndices) : base(Rows, Columns, T_DelegateUsingIndices) => AssignOperations();

        /// <summary>
        /// Instantiates a new Matrix array with the given <paramref name="Rows"/> and <paramref name="Columns"/>. Each new item in the array is set to the provided <paramref name="DefaultValue"/>
        /// <para>
        /// Example Usage:
        /// <code>
        /// var matrix = new float.Matrix( Rows: 2, Columns: 3, DefaultValue: 12);
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
        public Matrix(int Rows, int Columns, decimal DefaultValue) : base(Rows, Columns, DefaultValue) => AssignOperations();

        /// <summary>
        /// Instantiates a new Matrix array with the given <paramref name="Rows"/> and <paramref name="Columns"/>. Each new item in the array is set using the value returned from an iteration from <paramref name="Enumerable"/>. If the enumerable object fails to produce enough items to fill the entire matrix, all remaining values will be set to their default values. If <paramref name="MaxEnumeration"/> is not set then the enumerator will be attempted to be called for every item. If <paramref name="MaxEnumeration"/> IS set, the enumerable will only be used for <paramref name="MaxEnumeration"/> n elements before no longer being used to fill values in the matrix.
        /// <para>
        /// Example Usage:
        /// <code>
        /// var matrix = new Matrix&lt;float&gt;( Rows: 2, Columns: 3, Enumerable: new float[]{ 0, 1, 2, 3 });
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
        public Matrix(int Rows, int Columns, IEnumerable<decimal> Enumerable, int? MaxEnumeration = null) : base(Rows, Columns, Enumerable, MaxEnumeration) => AssignOperations();

        /// <summary>
        /// Instantiates a new Matrix array with the given <paramref name="Rows"/> and <paramref name="Columns"/>. Items are filled floato the matrix by slicing the provided <see cref="Span{T}"/>. If <paramref name="AsEnumerable"/> is set to true the <see cref="Span{T}"/> is used an an <see cref="IEnumerable{T}"/> object instead of single-dimensional contigous memory block. If the <see cref="Span{T}"/> is being used as an <see cref="IEnumerable{T}"/> fails to produce enough items to fill the entire matrix, all remaining values will be set to their <see langword="default"/> values.
        /// <para>
        /// Example Usage:
        /// <code>
        /// var matrix = new Matrix&lt;float&gt;( Rows: 2, Columns: 3, Values: new Span(new float[]{ 0, 1, 2, 3 }));
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
        public Matrix(int Rows, int Columns, Span<decimal> Values, bool AsEnumerable) : base(Rows, Columns, Values, AsEnumerable) => AssignOperations();

        public override OperationSet<decimal, decimal> DecimalOperations => SameTypedOperations;

        private void AssignOperations()
        {
            SameTypedOperations = new()
            {
                ReferenceAdder = (decimal Value) => (ref decimal element) => element += Value,
                ReferenceSubtractor = (decimal Value) => (ref decimal element) => element -= Value,
                ReferenceMultiplier = (decimal Scalar) => (ref decimal element) => element *= Scalar,
                TwoValueAdder = (ref decimal Left, ref decimal Right) => Left + Right,
                TwoValueMultiplier = (ref decimal Left, ref decimal Right) => Left * Right,
                TwoRefenceAdder = (ref decimal Left, ref decimal Right) => Left += Right,
                TwoRefenceSubtractor = (ref decimal Left, ref decimal Right) => Left -= Right,
                TwoReferenceMultiplier = (ref decimal Left, ref decimal Right) => Left *= Right,
            };
            IntegerOperations = new()
            {
                ReferenceAdder = (int Value) => (ref decimal element) => element += Value,
                ReferenceSubtractor = (int Value) => (ref decimal element) => element -= Value,
                ReferenceMultiplier = (int Scalar) => (ref decimal element) => element *= Scalar,
                TwoValueAdder = (ref decimal Left, ref decimal Right) => Left + Right,
                TwoValueMultiplier = (ref decimal Left, ref decimal Right) => Left * Right,
                TwoRefenceAdder = (ref decimal Left, ref decimal Right) => Left += Right,
                TwoRefenceSubtractor = (ref decimal Left, ref decimal Right) => Left -= Right,
                TwoReferenceMultiplier = (ref decimal Left, ref decimal Right) => Left *= Right,
            };
            FloatOperations = new()
            {
                ReferenceAdder = (float Value) => (ref decimal element) => element += (decimal)Value,
                ReferenceSubtractor = (float Value) => (ref decimal element) => element -= (decimal)Value,
                ReferenceMultiplier = (float Scalar) => (ref decimal element) => element *= (decimal)Scalar,
                TwoValueAdder = (ref decimal Left, ref decimal Right) => Left + Right,
                TwoValueMultiplier = (ref decimal Left, ref decimal Right) => Left * Right,
                TwoRefenceAdder = (ref decimal Left, ref decimal Right) => Left += Right,
                TwoRefenceSubtractor = (ref decimal Left, ref decimal Right) => Left -= Right,
                TwoReferenceMultiplier = (ref decimal Left, ref decimal Right) => Left *= Right,
            };
            DoubleOperations = new()
            {
                ReferenceAdder = (double Value) => (ref decimal element) => element += (decimal)Value,
                ReferenceSubtractor = (double Value) => (ref decimal element) => element -= (decimal)Value,
                ReferenceMultiplier = (double Scalar) => (ref decimal element) => element *= (decimal)Scalar,
                TwoValueAdder = (ref decimal Left, ref decimal Right) => Left + Right,
                TwoValueMultiplier = (ref decimal Left, ref decimal Right) => Left * Right,
                TwoRefenceAdder = (ref decimal Left, ref decimal Right) => Left += Right,
                TwoRefenceSubtractor = (ref decimal Left, ref decimal Right) => Left -= Right,
                TwoReferenceMultiplier = (ref decimal Left, ref decimal Right) => Left *= Right,
            };
        }
    }
}
