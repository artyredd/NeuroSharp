using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using NeuroSharp.Int;

namespace NeuroSharp.Tests
{
    public class IntMatrixTests
    {
        [Fact]
        public void Multiply_Scalar_OperatorWorks()
        {
            var matrix = new Matrix(2, 2, new int[] { 0, 1, 2, 3 });

            matrix *= 2;

            Assert.Equal(0, matrix[0, 0]);
            Assert.Equal(2, matrix[0, 1]);
            Assert.Equal(6, matrix[1, 1]);
            Assert.Equal(4, matrix[1, 0]);
        }

        [Fact]
        public void Add_Matrix_OperatorWorks()
        {
            var matrix = new Matrix(2, 2, new int[] { 0, 1, 2, 3 });

            var right = new Matrix(2, 2, new int[] { 3, 2, 1, 0 });

            matrix += right;

            Assert.Equal(3, matrix[0, 0]);
            Assert.Equal(3, matrix[0, 1]);
            Assert.Equal(3, matrix[1, 1]);
            Assert.Equal(3, matrix[1, 0]);
        }

        [Fact]
        public void Subtract_Matrix_OperatorWorks()
        {
            var left = new Matrix(2, 2, new int[] { 0, 1, 2, 3 });

            var right = new Matrix(2, 2, new int[] { 3, 2, 1, 0 });

            left -= right;

            Assert.Equal(-3, left[0, 0]);
            Assert.Equal(-1, left[0, 1]);
            Assert.Equal(1, left[1, 0]);
            Assert.Equal(3, left[1, 1]);

            right -= left;

            Assert.Equal(6, right[0, 0]);
            Assert.Equal(3, right[0, 1]);
            Assert.Equal(0, right[1, 0]);
            Assert.Equal(-3, right[1, 1]);
        }

        [Fact]
        public void Add_Constant_OperatorWorks()
        {
            var matrix = new Matrix(2, 2, new int[] { 0, 1, 2, 3 });

            matrix += 2;

            Assert.Equal(2, matrix[0, 0]);
            Assert.Equal(3, matrix[0, 1]);
            Assert.Equal(4, matrix[1, 0]);
            Assert.Equal(5, matrix[1, 1]);
        }

        [Fact]
        public void Subtract_Constant_OperatorWorks()
        {
            var matrix = new Matrix(2, 2, new int[] { 0, 1, 2, 3 });

            matrix -= 2;

            Assert.Equal(-2, matrix[0, 0]);
            Assert.Equal(-1, matrix[0, 1]);
            Assert.Equal(0, matrix[1, 0]);
            Assert.Equal(1, matrix[1, 1]);
        }

        [Fact]
        public void InvalidOperationsThrows()
        {
            var matrix = new Matrix(2, 2);
            void ShouldThrow(Action op)
            {
                op();
            }

            Assert.Throws<InvalidOperationException>(() => ShouldThrow(() => { matrix *= 2.0f; }));
            Assert.Throws<InvalidOperationException>(() => ShouldThrow(() => { matrix *= 2.0m; }));
            Assert.Throws<InvalidOperationException>(() => ShouldThrow(() => { matrix *= 2.0d; }));
            Assert.Throws<InvalidOperationException>(() => ShouldThrow(() => { matrix += 2.0f; }));
            Assert.Throws<InvalidOperationException>(() => ShouldThrow(() => { matrix += 2.0m; }));
            Assert.Throws<InvalidOperationException>(() => ShouldThrow(() => { matrix += 2.0d; }));
            Assert.Throws<InvalidOperationException>(() => ShouldThrow(() => { matrix -= 2.0f; }));
            Assert.Throws<InvalidOperationException>(() => ShouldThrow(() => { matrix -= 2.0m; }));
            Assert.Throws<InvalidOperationException>(() => ShouldThrow(() => { matrix -= 2.0d; }));
            Assert.Throws<InvalidOperationException>(() => ShouldThrow(() => { matrix = 2.0f * matrix; }));
            Assert.Throws<InvalidOperationException>(() => ShouldThrow(() => { matrix = 2.0d * matrix; }));
            Assert.Throws<InvalidOperationException>(() => ShouldThrow(() => { matrix = 2.0m * matrix; }));
            Assert.Throws<InvalidOperationException>(() => ShouldThrow(() => { matrix = 2.0f - matrix; }));
            Assert.Throws<InvalidOperationException>(() => ShouldThrow(() => { matrix = 2.0d - matrix; }));
            Assert.Throws<InvalidOperationException>(() => ShouldThrow(() => { matrix = 2.0m - matrix; }));
            Assert.Throws<InvalidOperationException>(() => ShouldThrow(() => { matrix = 2.0f + matrix; }));
            Assert.Throws<InvalidOperationException>(() => ShouldThrow(() => { matrix = 2.0d + matrix; }));
            Assert.Throws<InvalidOperationException>(() => ShouldThrow(() => { matrix = 2.0m + matrix; }));
        }

        [Fact]
        public void Multiply_MultipleMatricesWorks()
        {
            var left = new Matrix(2, 4, new int[] { 0, 1, 2, 3, 4, 5, 6, 7 });
            var right = new Matrix(4, 2, new int[] { 0, 1, 2, 3, 4, 5, 6, 7 });

            var result = left * right;

            Assert.Equal(28, result[0, 0]);
            Assert.Equal(34, result[0, 1]);
            Assert.Equal(76, result[1, 0]);
            Assert.Equal(98, result[1, 1]);

            left = new Matrix(1, 2, 1);
            right = new Matrix(2, 10, 3);

            /*
                Input:
                [ 1 1 ]  x  [ 3 3 3 3 3 3 3 3 3 3 ]
                            [ 3 3 3 3 3 3 3 3 3 3 ]
                Expected:
                [ 6 6 6 6 6 6 6 6 6 6 ]
            */

            var array = (left * right).ToOneDimension();

            foreach (var item in array)
            {
                Assert.Equal(6, item);
            }
        }

        public void Multiply_InvalidMatriciesThrowsError()
        {
            // the multiply function should throw if the matrices don't qualify to be multiplied

            // only square and left matrice left rows = right cols
            var left = new Matrix(2, 3, 12);
            var right = new Matrix(3, 3, 1);

            Assert.Throws<InvalidOperationException>(() => { left = left * right; });
        }
    }
}
