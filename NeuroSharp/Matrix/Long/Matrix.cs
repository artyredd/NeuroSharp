using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuroSharp.Long
{
    public class Matrix : BaseMatrix<long>, IMatrix<long>
    {
        /// <summary>
        /// Creates a new long32 Matrix.
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
        /// Instantiates a new Matrix array with the given <paramref name="Rows"/> and <paramref name="Columns"/>. For each new item in all rows <paramref name="T_Provider"/> is invoked to set the value of the long32 in the spot.
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
        public Matrix(int Rows, int Columns, Func<long> T_Delegate) : base(Rows, Columns, T_Delegate) => AssignOperations();

        /// <summary>
        /// Instantiates a new Matrix array with the given <paramref name="Rows"/> and <paramref name="Columns"/>. For each new item in all rows <paramref name="T_ProviderUsingRow"/> is invoked to set the value of the in32 in the spot, this <paramref name="T_ProviderUsingRow"/> is invoked with a <see cref="long"/> Row Index where that element is contained.
        /// <para>
        /// Example Usage(where i = Row Index where the element is located):
        /// <code>
        /// var matrix = new long.Matrix( Rows: 2, Columns: 3, DelegateThatReturnsTUsingRowNumber: (i)=&gt; i);
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
        public Matrix(int Rows, int Columns, Func<int, long> T_DelegateUsingRowIndex) : base(Rows, Columns, T_DelegateUsingRowIndex) => AssignOperations();

        /// <summary>
        /// Instantiates a new Matrix array with the given <paramref name="Rows"/> and <paramref name="Columns"/>. For each new item in all rows <paramref name="T_ProviderUsingIndexes"/> is invoked to set the value of the long32 in the spot, this <paramref name="T_ProviderUsingIndexes"/> is invoked with a <see cref="long"/> Row Index where that element is contained.
        /// <para>
        /// Example Usage(where x = Row Index where the element is located, and y = Column Index where the element is located):
        /// <code>
        /// var matrix = new long.Matrix( Rows: 2, Columns: 3, DelegateThatReturnsTUsingRowNumberAndColumnNumber: (x,y)=&gt; x + y);
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
        public Matrix(int Rows, int Columns, Func<int, int, long> T_DelegateUsingIndices) : base(Rows, Columns, T_DelegateUsingIndices) => AssignOperations();

        /// <summary>
        /// Instantiates a new Matrix array with the given <paramref name="Rows"/> and <paramref name="Columns"/>. Each new item in the array is set to the provided <paramref name="DefaultValue"/>
        /// <para>
        /// Example Usage:
        /// <code>
        /// var matrix = new long.Matrix( Rows: 2, Columns: 3, DefaultValue: 12);
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
        public Matrix(int Rows, int Columns, long DefaultValue) : base(Rows, Columns, DefaultValue) => AssignOperations();

        /// <summary>
        /// Instantiates a new Matrix array with the given <paramref name="Rows"/> and <paramref name="Columns"/>. Each new item in the array is set using the value returned from an iteration from <paramref name="Enumerable"/>. If the enumerable object fails to produce enough items to fill the entire matrix, all remaining values will be set to their default values. If <paramref name="MaxEnumeration"/> is not set then the enumerator will be attempted to be called for every item. If <paramref name="MaxEnumeration"/> IS set, the enumerable will only be used for <paramref name="MaxEnumeration"/> n elements before no longer being used to fill values in the matrix.
        /// <para>
        /// Example Usage:
        /// <code>
        /// var matrix = new Matrix&lt;long&gt;( Rows: 2, Columns: 3, Enumerable: new long[]{ 0, 1, 2, 3 });
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
        public Matrix(int Rows, int Columns, IEnumerable<long> Enumerable, int? MaxEnumeration = null) : base(Rows, Columns, Enumerable, MaxEnumeration) => AssignOperations();

        /// <summary>
        /// Instantiates a new Matrix array with the given <paramref name="Rows"/> and <paramref name="Columns"/>. Items are filled longo the matrix by slicing the provided <see cref="Span{T}"/>. If <paramref name="AsEnumerable"/> is set to true the <see cref="Span{T}"/> is used an an <see cref="IEnumerable{T}"/> object instead of single-dimensional contigous memory block. If the <see cref="Span{T}"/> is being used as an <see cref="IEnumerable{T}"/> fails to produce enough items to fill the entire matrix, all remaining values will be set to their <see langword="default"/> values.
        /// <para>
        /// Example Usage:
        /// <code>
        /// var matrix = new Matrix&lt;long&gt;( Rows: 2, Columns: 3, Values: new Span(new long[]{ 0, 1, 2, 3 }));
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
        public Matrix(int Rows, int Columns, Span<long> Values, bool AsEnumerable) : base(Rows, Columns, Values, AsEnumerable) => AssignOperations();

        public override OperationSet<long, long> LongOperations => SameTypedOperations;

        private void AssignOperations()
        {
            SameTypedOperations = new()
            {
                ReferenceAdder = (long Value) => (ref long element) => element += Value,
                ReferenceSubtractor = (long Value) => (ref long element) => element -= Value,
                ReferenceMultiplier = (long Scalar) => (ref long element) => element *= Scalar,
                TwoValueAdder = (ref long Left, ref long Right) => Left + Right,
                TwoValueMultiplier = (ref long Left, ref long Right) => Left * Right,
                TwoRefenceAdder = (ref long Left, ref long Right) => Left += Right,
                TwoRefenceSubtractor = (ref long Left, ref long Right) => Left -= Right,
                TwoReferenceMultiplier = (ref long Left, ref long Right) => Left *= Right,
            };
            IntegerOperations = new()
            {
                ReferenceAdder = (int Value) => (ref long element) => element += Value,
                ReferenceSubtractor = (int Value) => (ref long element) => element -= Value,
                ReferenceMultiplier = (int Scalar) => (ref long element) => element *= Scalar,
                TwoValueAdder = (ref long Left, ref long Right) => Left + Right,
                TwoValueMultiplier = (ref long Left, ref long Right) => Left * Right,
                TwoRefenceAdder = (ref long Left, ref long Right) => Left += Right,
                TwoRefenceSubtractor = (ref long Left, ref long Right) => Left -= Right,
                TwoReferenceMultiplier = (ref long Left, ref long Right) => Left *= Right,
            };
            ShortOperations = new()
            {
                ReferenceAdder = (short Value) => (ref long element) => element += Value,
                ReferenceSubtractor = (short Value) => (ref long element) => element -= Value,
                ReferenceMultiplier = (short Scalar) => (ref long element) => element *= Scalar,
                TwoValueAdder = (ref long Left, ref long Right) => Left + Right,
                TwoValueMultiplier = (ref long Left, ref long Right) => Left * Right,
                TwoRefenceAdder = (ref long Left, ref long Right) => Left += Right,
                TwoRefenceSubtractor = (ref long Left, ref long Right) => Left -= Right,
                TwoReferenceMultiplier = (ref long Left, ref long Right) => Left *= Right,
            };
            ByteOperations = new()
            {
                ReferenceAdder = (byte Value) => (ref long element) => element += Value,
                ReferenceSubtractor = (byte Value) => (ref long element) => element -= Value,
                ReferenceMultiplier = (byte Scalar) => (ref long element) => element *= Scalar,
                TwoValueAdder = (ref long Left, ref long Right) => Left + Right,
                TwoValueMultiplier = (ref long Left, ref long Right) => Left * Right,
                TwoRefenceAdder = (ref long Left, ref long Right) => Left += Right,
                TwoRefenceSubtractor = (ref long Left, ref long Right) => Left -= Right,
                TwoReferenceMultiplier = (ref long Left, ref long Right) => Left *= Right,
            };
            SByteOperations = new()
            {
                ReferenceAdder = (sbyte Value) => (ref long element) => element += Value,
                ReferenceSubtractor = (sbyte Value) => (ref long element) => element -= Value,
                ReferenceMultiplier = (sbyte Scalar) => (ref long element) => element *= Scalar,
                TwoValueAdder = (ref long Left, ref long Right) => Left + Right,
                TwoValueMultiplier = (ref long Left, ref long Right) => Left * Right,
                TwoRefenceAdder = (ref long Left, ref long Right) => Left += Right,
                TwoRefenceSubtractor = (ref long Left, ref long Right) => Left -= Right,
                TwoReferenceMultiplier = (ref long Left, ref long Right) => Left *= Right,
            };
        }
    }
}
