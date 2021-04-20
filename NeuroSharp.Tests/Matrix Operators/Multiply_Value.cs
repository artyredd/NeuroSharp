using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace NeuroSharp.Tests.Matrix_Operators
{
    public class Multiply_Value
    {
        [Fact]
        public void SByte()
        {
            IMatrix<sbyte> left = new SByte.Matrix(2, 2, new sbyte[] { 1, 2, 3, 4 });
            IMatrix<sbyte> right = new SByte.Matrix(2, 2, new sbyte[] { 4, 3, 2, 1 });

            IMatrix<sbyte> leftStored = left.Duplicate();
            IMatrix<sbyte> rightStored = right.Duplicate();

            // subtract
            IMatrix<sbyte> result = (sbyte)1 * left;

            result *= (sbyte)2;

            // make sure the originals match the stored versions
            Assert.Equal(leftStored, left);
            Assert.Equal(rightStored, right);

            // chaneg a value in the result and verify that the result is a value and not a reference to the roginal matrix
            result[0, 0] = -1;

            Assert.Equal(leftStored, left);
            Assert.Equal(rightStored, right);

            // change values in the original and make sure that the equality comparisons are properly working
            left[0, 0] = -1;
            right[0, 0] = 2;

            Assert.NotEqual(leftStored, left);
            Assert.NotEqual(rightStored, right);

            // make sure the + oppertaor throws with ivalid matrices
            left = new SByte.Matrix(2, 3, 7);
            void ShouldThrow()
            {
                var x = left + right;
            }
            Assert.Throws<InvalidOperationException>(ShouldThrow);
        }

        [Fact]
        public void Byte()
        {
            IMatrix<byte> left = new Byte.Matrix(2, 2, new byte[] { 1, 2, 3, 4 });
            IMatrix<byte> right = new Byte.Matrix(2, 2, new byte[] { 4, 3, 2, 1 });

            IMatrix<byte> leftStored = left.Duplicate();
            IMatrix<byte> rightStored = right.Duplicate();

            // subtract
            // subtract
            IMatrix<byte> result = (byte)1 * left;

            result *= (byte)2;

            // make sure the originals match the stored versions
            Assert.Equal(leftStored, left);
            Assert.Equal(rightStored, right);

            // chaneg a value in the result and verify that the result is a value and not a reference to the roginal matrix
            result[0, 0] = 2;

            Assert.Equal(leftStored, left);
            Assert.Equal(rightStored, right);

            // change values in the original and make sure that the equality comparisons are properly working
            left[0, 0] = 3;
            right[0, 0] = 2;

            Assert.NotEqual(leftStored, left);
            Assert.NotEqual(rightStored, right);

            // make sure the + oppertaor throws with ivalid matrices
            left = new Byte.Matrix(2, 3, 7);
            void ShouldThrow()
            {
                var x = left + right;
            }
            Assert.Throws<InvalidOperationException>(ShouldThrow);
        }

        [Fact]
        public void Short()
        {
            IMatrix<short> left = new Short.Matrix(2, 2, new short[] { 1, 2, 3, 4 });
            IMatrix<short> right = new Short.Matrix(2, 2, new short[] { 4, 3, 2, 1 });

            IMatrix<short> leftStored = left.Duplicate();
            IMatrix<short> rightStored = right.Duplicate();

            // subtract
            // subtract
            // subtract
            IMatrix<short> result = 1 * left;

            result *= 2;

            // make sure the originals match the stored versions
            Assert.Equal(leftStored, left);
            Assert.Equal(rightStored, right);

            // chaneg a value in the result and verify that the result is a value and not a reference to the roginal matrix
            result[0, 0] = -1;

            Assert.Equal(leftStored, left);
            Assert.Equal(rightStored, right);

            // change values in the original and make sure that the equality comparisons are properly working
            left[0, 0] = -1;
            right[0, 0] = 2;

            Assert.NotEqual(leftStored, left);
            Assert.NotEqual(rightStored, right);

            // make sure the + oppertaor throws with ivalid matrices
            left = new Short.Matrix(2, 3, 7);
            void ShouldThrow()
            {
                var x = left + right;
            }
            Assert.Throws<InvalidOperationException>(ShouldThrow);
        }

        [Fact]
        public void Int()
        {
            IMatrix<int> left = new Int.Matrix(2, 2, new int[] { 1, 2, 3, 4 });
            IMatrix<int> right = new Int.Matrix(2, 2, new int[] { 4, 3, 2, 1 });

            IMatrix<int> leftStored = left.Duplicate();
            IMatrix<int> rightStored = right.Duplicate();

            // subtract
            IMatrix<int> result = 1 * left;

            result *= 2;

            // make sure the originals match the stored versions
            Assert.Equal(leftStored, left);
            Assert.Equal(rightStored, right);

            // chaneg a value in the result and verify that the result is a value and not a reference to the roginal matrix
            result[0, 0] = -1;

            Assert.Equal(leftStored, left);
            Assert.Equal(rightStored, right);

            // change values in the original and make sure that the equality comparisons are properly working
            left[0, 0] = -1;
            right[0, 0] = 2;

            Assert.NotEqual(leftStored, left);
            Assert.NotEqual(rightStored, right);

            // make sure the + oppertaor throws with ivalid matrices
            left = new Int.Matrix(2, 3, 7);
            void ShouldThrow()
            {
                var x = left + right;
            }
            Assert.Throws<InvalidOperationException>(ShouldThrow);
        }

        [Fact]
        public void Long()
        {
            IMatrix<long> left = new Long.Matrix(2, 2, new long[] { 1, 2, 3, 4 });
            IMatrix<long> right = new Long.Matrix(2, 2, new long[] { 4, 3, 2, 1 });

            IMatrix<long> leftStored = left.Duplicate();
            IMatrix<long> rightStored = right.Duplicate();

            // subtract
            IMatrix<long> result = 2 * left;

            result *= 2;

            // make sure the originals match the stored versions
            Assert.Equal(leftStored, left);
            Assert.Equal(rightStored, right);

            // chaneg a value in the result and verify that the result is a value and not a reference to the roginal matrix
            result[0, 0] = -1;

            Assert.Equal(leftStored, left);
            Assert.Equal(rightStored, right);

            // change values in the original and make sure that the equality comparisons are properly working
            left[0, 0] = -1;
            right[0, 0] = 2;

            Assert.NotEqual(leftStored, left);
            Assert.NotEqual(rightStored, right);

            // make sure the + oppertaor throws with ivalid matrices
            left = new Long.Matrix(2, 3, 7);
            void ShouldThrow()
            {
                var x = left + right;
            }
            Assert.Throws<InvalidOperationException>(ShouldThrow);
        }

        [Fact]
        public void Double()
        {
            IMatrix<double> left = new Double.Matrix(2, 2, new double[] { 1, 2, 3, 4 });
            IMatrix<double> right = new Double.Matrix(2, 2, new double[] { 4, 3, 2, 1 });

            IMatrix<double> leftStored = left.Duplicate();
            IMatrix<double> rightStored = right.Duplicate();

            // subtract
            IMatrix<double> result = 2.0d * left;

            result *= 1.0d;

            // make sure the originals match the stored versions
            Assert.Equal(leftStored, left);
            Assert.Equal(rightStored, right);

            // chaneg a value in the result and verify that the result is a value and not a reference to the roginal matrix
            result[0, 0] = -1;

            Assert.Equal(leftStored, left);
            Assert.Equal(rightStored, right);

            // change values in the original and make sure that the equality comparisons are properly working
            left[0, 0] = -1;
            right[0, 0] = 2;

            Assert.NotEqual(leftStored, left);
            Assert.NotEqual(rightStored, right);

            // make sure the + oppertaor throws with ivalid matrices
            left = new Double.Matrix(2, 3, 7);
            void ShouldThrow()
            {
                var x = left + right;
            }
            Assert.Throws<InvalidOperationException>(ShouldThrow);
        }

        [Fact]
        public void Float()
        {
            IMatrix<float> left = new Float.Matrix(2, 2, new float[] { 1, 2, 3, 4 });
            IMatrix<float> right = new Float.Matrix(2, 2, new float[] { 4, 3, 2, 1 });

            IMatrix<float> leftStored = left.Duplicate();
            IMatrix<float> rightStored = right.Duplicate();

            // subtract
            IMatrix<float> result = 1.0f * left;

            result *= 2.0f;

            // make sure the originals match the stored versions
            Assert.Equal(leftStored, left);
            Assert.Equal(rightStored, right);

            // chaneg a value in the result and verify that the result is a value and not a reference to the roginal matrix
            result[0, 0] = -1;

            Assert.Equal(leftStored, left);
            Assert.Equal(rightStored, right);

            // change values in the original and make sure that the equality comparisons are properly working
            left[0, 0] = -1;
            right[0, 0] = 2;

            Assert.NotEqual(leftStored, left);
            Assert.NotEqual(rightStored, right);

            // make sure the + oppertaor throws with ivalid matrices
            left = new Float.Matrix(2, 3, 7);
            void ShouldThrow()
            {
                var x = left + right;
            }
            Assert.Throws<InvalidOperationException>(ShouldThrow);
        }

        [Fact]
        public void Decimal()
        {
            IMatrix<decimal> left = new Decimal.Matrix(2, 2, new decimal[] { 1, 2, 3, 4 });
            IMatrix<decimal> right = new Decimal.Matrix(2, 2, new decimal[] { 4, 3, 2, 1 });

            IMatrix<decimal> leftStored = left.Duplicate();
            IMatrix<decimal> rightStored = right.Duplicate();

            // subtract
            IMatrix<decimal> result = 1.0m + right;

            result *= 2.0m;

            // make sure the originals match the stored versions
            Assert.Equal(leftStored, left);
            Assert.Equal(rightStored, right);

            // chaneg a value in the result and verify that the result is a value and not a reference to the roginal matrix
            result[0, 0] = -1;

            Assert.Equal(leftStored, left);
            Assert.Equal(rightStored, right);

            // change values in the original and make sure that the equality comparisons are properly working
            left[0, 0] = -1;
            right[0, 0] = 2;

            Assert.NotEqual(leftStored, left);
            Assert.NotEqual(rightStored, right);

            // make sure the + oppertaor throws with ivalid matrices
            left = new Decimal.Matrix(2, 3, 7);
            void ShouldThrow()
            {
                var x = left + right;
            }
            Assert.Throws<InvalidOperationException>(ShouldThrow);
        }
    }
}
