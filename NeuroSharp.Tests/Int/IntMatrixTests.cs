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
            IMatrix<int> matrix = new Matrix(2, 2, new int[] { 0, 1, 2, 3 });

            matrix.IntegerOperations.ReferenceMultiplier = (int scalar) => ((ref int matrixElement) => matrixElement *= scalar);

            matrix *= 2;

            Assert.Equal(0, matrix[0, 0]);
            Assert.Equal(2, matrix[0, 1]);
            Assert.Equal(6, matrix[1, 1]);
            Assert.Equal(4, matrix[1, 0]);
        }

        [Fact]
        public void Add_Matrix_OperatorWorks()
        {
            IMatrix<int> matrix = new Matrix(2, 2, new int[] { 0, 1, 2, 3 });

            IMatrix<int> right = new Matrix(2, 2, new int[] { 3, 2, 1, 0 });

            matrix += right;

            Assert.Equal(3, matrix[0, 0]);
            Assert.Equal(3, matrix[0, 1]);
            Assert.Equal(3, matrix[1, 1]);
            Assert.Equal(3, matrix[1, 0]);
        }

        [Fact]
        public void Subtract_Matrix_OperatorWorks()
        {
            IMatrix<int> left = new Matrix(2, 2, new int[] { 0, 1, 2, 3 });

            IMatrix<int> right = new Matrix(2, 2, new int[] { 3, 2, 1, 0 });

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
            IMatrix<int> matrix = new Matrix(2, 2, new int[] { 0, 1, 2, 3 });

            matrix += 2;

            Assert.Equal(2, matrix[0, 0]);
            Assert.Equal(3, matrix[0, 1]);
            Assert.Equal(4, matrix[1, 0]);
            Assert.Equal(5, matrix[1, 1]);
        }

        [Fact]
        public void Subtract_Constant_OperatorWorks()
        {
            IMatrix<int> matrix = new Matrix(2, 2, new int[] { 0, 1, 2, 3 });

            matrix -= 2;

            Assert.Equal(-2, matrix[0, 0]);
            Assert.Equal(-1, matrix[0, 1]);
            Assert.Equal(0, matrix[1, 0]);
            Assert.Equal(1, matrix[1, 1]);
        }


        [Fact]
        public void Multiply_MultipleMatricesWorks()
        {
            IMatrix<int> left = new Matrix(2, 4, new int[] { 0, 1, 2, 3, 4, 5, 6, 7 });
            IMatrix<int> right = new Matrix(4, 2, new int[] { 0, 1, 2, 3, 4, 5, 6, 7 });

            IMatrix<int> result = left * right;

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

            var array = MatrixOperations<int>.MultiplyMatrices(left, right, left.SameTypedOperations.TwoValueMultiplier, left.SameTypedOperations.TwoRefenceAdder
                );

            foreach (var item in array)
            {
                Assert.Equal(6, item);
            }
        }

        [Fact]
        public void Multiply_InvalidMatriciesThrowsError()
        {
            // the multiply function should throw if the matrices don't qualify to be multiplied

            // only square and left matrice left rows = right cols
            IMatrix<int> left = new Matrix(2, 3, 12);
            IMatrix<int> right = new Matrix(1, 3, 1);

            Assert.Throws<InvalidOperationException>(() => { left = left * right; });
        }
    }
}
