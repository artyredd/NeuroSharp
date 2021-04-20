using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuroSharp
{
    public class OperationSet<T, U> where T : unmanaged, IComparable<T>, IEquatable<T>
    {
        /// <summary>
        /// Represents a delegate that when called with U Value as a parameter, it returns a <see cref="MatrixOperations.SingleElementOperation{T}"/>.
        /// <para>
        /// Example:
        /// </para>
        /// <code>
        /// SingleElementParameterizedOperation op = (U Value) => (ref T MatrixElement) => MatrixElement + Value;
        /// </code>
        /// <code>
        /// op(12);
        /// </code>
        /// Output:
        /// <code>
        /// (ref T MatrixElement) => MatrixElement + 12;
        /// </code>
        /// </summary>
        /// <param name="Value"></param>
        /// <returns>
        /// delegate that returns a <see cref="MatrixOperations.SingleElementOperation{T}"/>
        /// </returns>
        public delegate MatrixOperations<T>.SingleElementOperation<T> SingleElementParameterizedOperation(U Value);

        /// <summary>
        /// Represents a delegate that when called with U Value as a parameter, it returns a <see cref="MatrixOperations.SingleElementOperation{T}"/>.
        /// <para>
        /// This delegate should take the incoming number U and the reference value T and add U to T and set T (the += operator);
        /// </para>
        /// <para>
        /// Example:
        /// </para>
        /// <code>
        /// SingleElementParameterizedOperation ReferenceAdder = (U Value) => (ref T MatrixElement) => MatrixElement += Value;
        /// </code>
        /// <code>
        /// ReferenceAdder(12);
        /// </code>
        /// Output:
        /// <code>
        /// (ref T MatrixElement) => MatrixElement += 12;
        /// </code>
        /// </summary>
        /// <param name="Value"></param>
        /// <returns>
        /// delegate that returns a <see cref="MatrixOperations.SingleElementOperation{T}"/>
        /// </returns>
        public SingleElementParameterizedOperation ReferenceAdder { get; set; }

        /// <summary>
        /// Represents a delegate that when called with U Value as a parameter, it returns a <see cref="MatrixOperations.SingleElementOperation{T}"/>.
        /// <para>
        /// This delegate should take the incoming number U and the reference value T and subtract U from T and set T (the - operator);
        /// </para>
        /// <para>
        /// Example:
        /// </para>
        /// <code>
        /// SingleElementParameterizedOperation ReferenceAdder = (U Value) => (ref T MatrixElement) => MatrixElement -= Value;
        /// </code>
        /// <code>
        /// ReferenceSubtractor(12);
        /// </code>
        /// Output:
        /// <code>
        /// (ref T MatrixElement) => MatrixElement -= 12;
        /// </code>
        /// </summary>
        /// <param name="Value"></param>
        /// <returns>
        /// delegate that returns a <see cref="MatrixOperations.SingleElementOperation{T}"/>
        /// </returns>
        public SingleElementParameterizedOperation ReferenceSubtractor { get; set; }

        /// <summary>
        /// Represents a delegate that when called with U Value as a parameter, it returns a <see cref="MatrixOperations.SingleElementOperation{T}"/>.
        /// <para>
        /// This delegate should take the incoming number U and the reference value T and multiply T with U and set T (the *= operator);
        /// </para>
        /// <para>
        /// Example:
        /// </para>
        /// <code>
        /// SingleElementParameterizedOperation ReferenceMultiplier = (U Value) => (ref T MatrixElement) => MatrixElement *= Value;
        /// </code>
        /// <code>
        /// ReferenceMultiplier(12);
        /// </code>
        /// Output:
        /// <code>
        /// (ref T MatrixElement) => MatrixElement *= 12;
        /// </code>
        /// </summary>
        /// <param name="Value"></param>
        /// <returns>
        /// delegate that returns a <see cref="MatrixOperations.SingleElementOperation{T}"/>
        /// </returns>
        public SingleElementParameterizedOperation ReferenceMultiplier { get; set; }

        /// <summary>
        /// Represents a delegate that when called adds two values of the given types together and returns T
        /// <para>
        /// This delegate should take an incoming refence value U and the reference value T and add them together and return the result.
        /// <code>
        /// This should not mutate either value!
        /// </code>
        /// </para>
        /// <para>
        /// Example:
        /// </para>
        /// <code>
        /// TwoElementOperation TwoValueAdder = (ref T Left, ref U Right) => Left + Right;
        /// </code>
        /// <code>
        /// TwoValueAdder(12,12);
        /// </code>
        /// Output:
        /// <code>
        /// 24
        /// </code>
        /// </summary>
        /// <param name="Value"></param>
        public MatrixOperations<T>.TwoElementOperation<T> TwoValueAdder { get; set; }

        /// <summary>
        /// Represents a delegate that when called multiplies two values of the given types together and returns T
        /// <para>
        /// This delegate should take an incoming refence value U and the reference value T and multiply them together and return the result.
        /// <code>
        /// This should not mutate either value!
        /// </code>
        /// </para>
        /// <para>
        /// Example:
        /// </para>
        /// <code>
        /// TwoElementOperation TwoValueMultiplier = (ref T Left, ref U Right) => Left * Right;
        /// </code>
        /// <code>
        /// TwoValueAdder(5,5);
        /// </code>
        /// Output:
        /// <code>
        /// 25
        /// </code>
        /// </summary>
        /// <param name="Value"></param>
        public MatrixOperations<T>.TwoElementOperation<T> TwoValueMultiplier { get; set; }

        /// <summary>
        /// Represents a delegate that when called adds two values of the given types together and mutates the incoming T reference value
        /// <para>
        /// This delegate should take an incoming refence value U and the reference value T and add them together and set T to the result.
        /// <code>
        /// This is expected to mutate the incoming T reference value
        /// </code>
        /// </para>
        /// <para>
        /// Example:
        /// </para>
        /// <code>
        /// TwoElementOperation TwoRefenceAdder = (ref T Left, ref U Right) => Left += Right;
        /// </code>
        /// <code>
        /// TwoRefenceAdder(7,7);
        /// </code>
        /// Output:
        /// <code>
        /// 14
        /// </code>
        /// </summary>
        /// <param name="Value"></param>
        public MatrixOperations<T>.TwoElementOperation<T> TwoRefenceAdder { get; set; }

        /// <summary>
        /// Represents a delegate that when called subtracts two values of the given types and mutates the incoming T reference value
        /// <para>
        /// This delegate should take an incoming refence value U and the reference value T and subtract them and set T to the result.
        /// <code>
        /// This is expected to mutate the incoming T reference value
        /// </code>
        /// </para>
        /// <para>
        /// Example:
        /// </para>
        /// <code>
        /// TwoElementOperation TwoRefenceSubtractor = (ref T Left, ref U Right) => Left -= Right;
        /// </code>
        /// <code>
        /// TwoRefenceSubtractor(8,4);
        /// </code>
        /// Output:
        /// <code>
        /// 4
        /// </code>
        /// </summary>
        /// <param name="Value"></param>
        public MatrixOperations<T>.TwoElementOperation<T> TwoRefenceSubtractor { get; set; }

        /// <summary>
        /// Represents a delegate that when called multiplies two values of the given types and mutates the incoming T reference value
        /// <para>
        /// This delegate should take an incoming refence value U and the reference value T and multiply them and set T to the result.
        /// <code>
        /// This is expected to mutate the incoming T reference value
        /// </code>
        /// </para>
        /// <para>
        /// Example:
        /// </para>
        /// <code>
        /// TwoElementOperation TwoReferenceMultiplier = (ref T Left, ref U Right) => Left *= Right;
        /// </code>
        /// <code>
        /// TwoReferenceMultiplier(3,3);
        /// </code>
        /// Output:
        /// <code>
        /// 9
        /// </code>
        /// </summary>
        public MatrixOperations<T>.TwoElementOperation<T> TwoReferenceMultiplier { get; set; }
    }
}
