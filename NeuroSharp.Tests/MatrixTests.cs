using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using NeuroSharp;
using NeuroSharp.Extensions.Matrix;

namespace NeuroSharp.Tests
{
    public class MatrixTests
    {
        [Fact]
        public void TestMatrixConstructor()
        {
            Assert.True(true);
        }

        [Theory]
        [InlineData(1, 1)]
        [InlineData(4, 3)]
        [InlineData(99, 99)]
        public void Constructor_Normal_Works(int rows, int columns)
        {
            var matrix = new BaseMatrix<int>(rows, columns);

            Assert.Equal(matrix.Rows, rows);

            Assert.Equal(matrix.Columns, columns);
        }

        [Fact]
        public void Constructor_Normal_FillsDefault()
        {
            // it should be expected that when we create a new matrix all the values are defaulted to not-null
            var matrix = new BaseMatrix<int>(10, 10);

            foreach (var row in matrix)
            {
                foreach (var element in row)
                {
                    Assert.Equal(default, element);
                }
            }

            var floatMatrix = new BaseMatrix<float>(10, 10);

            foreach (var row in floatMatrix)
            {
                foreach (var element in row)
                {
                    Assert.Equal(default, element);
                }
            }

            var decimalmatrix = new BaseMatrix<decimal>(10, 10);

            foreach (var row in decimalmatrix)
            {
                foreach (var element in row)
                {
                    Assert.Equal(default, element);
                }
            }

            var byteMatrix = new BaseMatrix<byte>(10, 10);

            foreach (var row in byteMatrix)
            {
                foreach (var element in row)
                {
                    Assert.Equal(default, element);
                }
            }

            var doubleMatrix = new BaseMatrix<double>(10, 10);

            foreach (var row in doubleMatrix)
            {
                foreach (var element in row)
                {
                    Assert.Equal(default, element);
                }
            }
            var sbyteMatrix = new BaseMatrix<sbyte>(10, 10);

            foreach (var row in sbyteMatrix)
            {
                foreach (var element in row)
                {
                    Assert.Equal(default, element);
                }
            }
            var shortMatrix = new BaseMatrix<short>(10, 10);

            foreach (var row in shortMatrix)
            {
                foreach (var element in row)
                {
                    Assert.Equal(default, element);
                }
            }
            var ushortMatrix = new BaseMatrix<ushort>(10, 10);

            foreach (var row in ushortMatrix)
            {
                foreach (var element in row)
                {
                    Assert.Equal(default, element);
                }
            }
            var uintMatrix = new BaseMatrix<uint>(10, 10);

            foreach (var row in uintMatrix)
            {
                foreach (var element in row)
                {
                    Assert.Equal(default, element);
                }
            }
            var longMatrix = new BaseMatrix<long>(10, 10);

            foreach (var row in longMatrix)
            {
                foreach (var element in row)
                {
                    Assert.Equal(default, element);
                }
            }

            var ulongMatrix = new BaseMatrix<ulong>(10, 10);

            foreach (var row in ulongMatrix)
            {
                foreach (var element in row)
                {
                    Assert.Equal(default, element);
                }
            }
        }

        [Theory]
        [InlineData(0)]
        [InlineData(14)]
        [InlineData(99)]
        [InlineData(int.MaxValue)]
        [InlineData(int.MinValue)]
        [InlineData(44_444)]
        [InlineData(0_988)]
        [InlineData(1_000_000_000)]
        public void Constructor_Fill_FillsDefault(int data)
        {
            var matrix = new BaseMatrix<int>(10, 10, data);

            foreach (var row in matrix)
            {
                foreach (var element in row)
                {
                    Assert.Equal(data, element);
                }
            }
        }

        [Fact]
        public void Constructor_Func_NoParamFills()
        {
            bool called = false;
            int called_n_times = 0;

            int TestCallMethod()
            {
                called = true;
                called_n_times++;
                return called_n_times;
            }

            var matrix = new BaseMatrix<int>(10, 10, TestCallMethod);

            Assert.True(called);

            Assert.Equal(10 * 10, called_n_times);
        }

        [Fact]
        public void Constructor_Func_RowParamFills()
        {
            bool called = false;
            int called_n_times = 0;

            int TestCallMethod(int row)
            {
                called = true;
                called_n_times++;
                return row;
            }

            var matrix = new BaseMatrix<int>(10, 10, TestCallMethod);

            Assert.True(called);

            Assert.Equal(10 * 10, called_n_times);

            /*
             Should expect:
                       Column[0] Column[1] Column[n]
                Row[0]: _____ 0 _______ 0 _______ 0 
                Row[1]: _____ 1 _______ 1 _______ 1
                Row[n]: _____ n _______ n _______ n
            */

            for (int i = 0; i < 10; i++)
            {
                Assert.Equal(i, matrix[i, i]);
            }

            /*
             Above checks
                       Column[0] Column[1] Column[n]
                Row[0]: _____ 0 ____________________
                Row[1]: _______________ 1 __________
                Row[n]: _________________________ n 
            */
        }
        [Fact]
        public void Constructor_Func_RowAndColumnParamFills()
        {
            bool called = false;
            int called_n_times = 0;

            int TestCallMethod(int row, int column)
            {
                called = true;
                called_n_times++;
                return row + column;
            }

            var matrix = new BaseMatrix<int>(10, 10, TestCallMethod);

            Assert.True(called);

            Assert.Equal(10 * 10, called_n_times);

            /*
             Should expect:
                       Column[0] Column[1] Column[n]
                Row[0]: _____ 0 _______ 1 _______ n + 0 
                Row[1]: _____ 1 _______ 2 _______ n + 1
                Row[n]: _____ n _______ n + 1 ___ n + n
            */

            for (int i = 0; i < 10; i++)
            {
                Assert.Equal(i + i, matrix[i, i]);
            }

            /*
             Above checks
                       Column[0] Column[1] Column[n]
                Row[0]: _____ 0 ____________________
                Row[1]: _______________ 2 __________
                Row[n]: _________________________ n + n
            */
        }

        [Fact]
        public void Constructor_Enumerable_EnumerableFills()
        {
            var enumerable = new float[100];
            for (int i = 0; i < enumerable.Length; i++)
            {
                enumerable[i] = i * 0.5f;
            }

            var matrix = new BaseMatrix<float>(10, 10, enumerable);

            /*
             Should expect:
                       Column[0] Column[1]
                Row[0]: _____ 0 _______ 0.5f
                Row[1]: _____ 0.5f ____ 2f
            */
            for (int i = 0; i < 10; i++)
            {
                Assert.Equal(i * 0.5f, matrix[0, i]);
            }

            // make sure that null enumerables still fill the matrix with default values
            IEnumerable<decimal> NullEnumberable()
            {
                return null;
            }

            var stringMatrix = new BaseMatrix<decimal>(10, 10, NullEnumberable());

            foreach (var row in stringMatrix)
            {
                foreach (var element in row)
                {
                    Assert.Equal(default, element);
                }
            }

            IEnumerable<int> GetInts()
            {
                yield return 10;
                yield return 20;
                yield return 30;
            }

            BaseMatrix<int> boolMatrix = new(10, 10, GetInts());

            // if the enumerable runs out it should still fill the values

            for (int i = 0; i < boolMatrix.Count; i++)
            {
                switch (i)
                {
                    case 0:
                        Assert.Equal(10, boolMatrix[0, 0]);
                        break;
                    case 1:
                        Assert.Equal(20, boolMatrix[0, 1]);
                        break;
                    case 2:
                        Assert.Equal(30, boolMatrix[0, 2]);
                        break;
                    default:
                        Assert.Equal(0, boolMatrix[i, i]);
                        break;
                }
            }
        }

        [Fact]
        public void Indexing_ProperlyMapped()
        {
            // verify that the indexing of the matrix appropriately retrieves and sets the correct items
            var matrix = new BaseMatrix<int>(10, 10);

            // use jagged indices
            Assert.False(matrix[5, 5] == 1);

            matrix[5, 5] = 1;

            Assert.True(matrix[5, 5] == 1);

            // use dimensional
            Assert.False(matrix[9, 4] == 1);

            matrix[9, 4] = 1;

            Assert.True(matrix[9, 4] == 1);

            // make sure we can cross use(not that it should matter but this is why we test constants, so we know variables dont work)
            Assert.False(matrix[0, 7] == 1);

            // set using jagged
            matrix[0, 7] = 1;

            // read using dimensional
            Assert.True(matrix[0, 7] == 1);

            // make sure the row indexing works as well

            int[] firstRow = matrix[0];

            // check to see if the value we set in an earlier test is there when we get a row from the indexer
            Assert.True(firstRow[7] == 1);

            // modify the row
            firstRow[3] = 1;

            // make sure we clone it just incase we got a reference instead of a by-value copy
            matrix[0] = (int[])firstRow.Clone();

            // check to see if the index setter for rows work
            Assert.True(matrix[0, 3] == 1);
        }

        [Fact]
        public void Capacity_ProperlySets()
        {
            var matrix = new BaseMatrix<int>(10, 10);

            Assert.Equal(100, matrix.Capacity);
        }

        [Fact]
        public void Tranpose_Normal_Works()
        {
            // make sure the matrix rotats correctly
            int x = 0;
            var matrix = new BaseMatrix<int>(2, 3, () => x++);
            // expected 
            /*
                0 1 2
                3 4 5
            */

            // make sure the matrix was correly instantiated
            Assert.Equal(0, matrix[0, 0]);
            Assert.Equal(1, matrix[0, 1]);
            Assert.Equal(2, matrix[0, 2]);
            Assert.Equal(3, matrix[1, 0]);
            Assert.Equal(4, matrix[1, 1]);
            Assert.Equal(5, matrix[1, 2]);

            // tranpose it
            var tranposedMatrix = matrix.Transpose();

            /*
                Expected
            
                0 3
                1 4
                2 5
            */
            Assert.Equal(0, tranposedMatrix[0, 0]);
            Assert.Equal(3, tranposedMatrix[0, 1]);
            Assert.Equal(1, tranposedMatrix[1, 0]);
            Assert.Equal(4, tranposedMatrix[1, 1]);
            Assert.Equal(2, tranposedMatrix[2, 0]);
            Assert.Equal(5, tranposedMatrix[2, 1]);

            // test As Transformed
            Assert.Equal(0, matrix.Transposed[0, 0]);
            Assert.Equal(3, matrix.Transposed[0, 1]);
            Assert.Equal(1, matrix.Transposed[1, 0]);
            Assert.Equal(4, matrix.Transposed[1, 1]);
            Assert.Equal(2, matrix.Transposed[2, 0]);
            Assert.Equal(5, matrix.Transposed[2, 1]);
        }

        [Fact]
        public void AsTransposedWorks()
        {
            // make sure the matrix rotats correctly
            int x = 0;
            var matrix = new BaseMatrix<int>(2, 3, () => x++);
            // expected 
            /*
                0 1 2
                3 4 5
            */

            // make sure the matrix was correly instantiated
            Assert.Equal(0, matrix[0, 0]);
            Assert.Equal(1, matrix[0, 1]);
            Assert.Equal(2, matrix[0, 2]);
            Assert.Equal(3, matrix[1, 0]);
            Assert.Equal(4, matrix[1, 1]);
            Assert.Equal(5, matrix[1, 2]);

            Assert.Equal(3, matrix.Columns);
            Assert.Equal(2, matrix.Rows);

            /*
                Expected
            
                0 3
                1 4
                2 5
            */

            Assert.Equal(2, matrix.Transposed.Columns);
            Assert.Equal(3, matrix.Transposed.Rows);

            // test As Transformed
            Assert.Equal(0, matrix.Transposed[0, 0]);
            Assert.Equal(3, matrix.Transposed[0, 1]);
            Assert.Equal(1, matrix.Transposed[1, 0]);
            Assert.Equal(4, matrix.Transposed[1, 1]);
            Assert.Equal(2, matrix.Transposed[2, 0]);
            Assert.Equal(5, matrix.Transposed[2, 1]);
        }

        [Fact]
        public void AsTransposedIterable()
        {
            // make sure we can construct a for and a foreach loop
            var matrix = new BaseMatrix<int>(3, 2, (r, c) => r + c);

            // make sure the values mapped correctly
            for (int row = 0; row < matrix.Rows; row++)
            {
                for (int column = 0; column < matrix.Columns; column++)
                {
                    Assert.Equal(row + column, matrix[row, column]);
                }
            }

            // try to iterate as a transposed version
            for (int row = 0; row < matrix.Transposed.Rows; row++)
            {
                for (int column = 0; column < matrix.Transposed.Columns; column++)
                {
                    Assert.Equal(row + column, matrix.Transposed[row, column]);
                }
            }

            /*
                Normal:
                0 1
                1 2
                2 3
                Expected:
                0 1 2
                1 2 3
            */

            // make sure manually to double check
            Assert.Equal(0, matrix.Transposed[0, 0]);
            Assert.Equal(1, matrix.Transposed[0, 1]);
            Assert.Equal(2, matrix.Transposed[0, 2]);
            Assert.Equal(1, matrix.Transposed[1, 0]);
            Assert.Equal(2, matrix.Transposed[1, 1]);
            Assert.Equal(3, matrix.Transposed[1, 2]);
        }

        [Fact]
        public void TransposedMatrix_DuplicateWorks()
        {
            var matrix = new BaseMatrix<int>(2, 2, new int[] { 1, 2, 3, 4 });

            // verify that the duplicated matrix is transposed AND not a reference
            var dupe = matrix.Transposed.Duplicate();

            /*
                Expected
            
                1 3
                2 4
            */

            Assert.Equal(1, dupe[0, 0]);
            Assert.Equal(3, dupe[0, 1]);
            Assert.Equal(2, dupe[1, 0]);
            Assert.Equal(4, dupe[1, 1]);


        }

        [Fact]
        public void ApplyMemberWiseOperation_Works()
        {
            System.Random random = new();

            var sourceMatrix = new BaseMatrix<int>(100, 100, () => random.Next(int.MinValue >> 3, int.MaxValue >> 3));

            int[] nums = sourceMatrix.ToOneDimension();

            var matrix = new BaseMatrix<int>(100, 100, nums);

            // make sure the list mapped correctly by sampling one number
            Assert.Equal(nums[10], matrix[0][10]);

            matrix.PerformMemberWise((ref int x) => x *= 2);

            // iterate through the matrix and verify that the memberwise function applied correctly
            for (int row = 0; row < 100; row++)
            {
                for (int column = 0; column < 100; column++)
                {
                    Assert.True(matrix[row, column] == (nums[(row * 100) + column] * 2), $"Expected:{nums[(row * 100) + column] * 2}\nGot:{matrix[row, column]}\n Row:{row} Column:{column}");
                }
            }
        }

        [Fact]
        public void DuplicateWorks()
        {
            var matrix = new BaseMatrix<int>(2, 3, new int[] { 0, 1, 2, 3, 4, 5 });

            // make sure the matrix mapped the values correctly
            Assert.True(matrix[0, 0] == 0);
            Assert.True(matrix[0, 2] == 2);
            Assert.True(matrix[1, 2] == 5);

            // attempt to duplicate the object
            var dupe = matrix.Duplicate();

            // verify the values are correct
            Assert.True(dupe[0, 0] == 0);
            Assert.True(dupe[0, 2] == 2);
            Assert.True(dupe[1, 2] == 5);

            // make sure that they are copies and NOT references

            // double check duplicated value before changing
            Assert.True(dupe[0, 2] == 2);

            // modify the original array and not the dupe
            matrix[0, 2] = 9;

            // check to see if the duplicate array was changed
            Assert.True(dupe[0, 2] == 2);
        }
        [Fact]
        public void Multiply_Hadamard()
        {
            var first = new double[] { 2, 4, 6, 8 };
            var second = new double[] { 16, 32, 64, 128 };

            IMatrix<double> left = new Double.Matrix(2, 2, first);
            IMatrix<double> right = new Double.Matrix(2, 2, second);

            /*
                Expected Result:
                2 * 16 | 4 * 32
                6 * 64 | 8 * 128
            */

            var result = left % right;

            Assert.Equal(2 * 16, result[0, 0]);
            Assert.Equal(4 * 32, result[0, 1]);
            Assert.Equal(6 * 64, result[1, 0]);
            Assert.Equal(8 * 128, result[1, 1]);

            // verify that the operator did not mutate the originals
            IMatrix<double> left_verification = new Double.Matrix(2, 2, first);
            IMatrix<double> right_verification = new Double.Matrix(2, 2, second);

            Assert.Equal(left_verification, left);

            Assert.Equal(right_verification, right);

            // change value in result and verify immutability of original martices
            result[0, 0] = 420;

            Assert.Equal(left_verification, left);

            Assert.Equal(right_verification, right);

            // change value in left verify that equals override is properly working and we can trust the results of this test
            left[0, 0] = 120;

            Assert.NotEqual(left_verification, left);
        }

        [Theory]
        [InlineData(12, 11, 12, 34)]
        [InlineData(3, 4, 4, 6)]
        [InlineData(1, 1, 1, 1)]
        [InlineData(4, 4, 4, 4)]
        [InlineData(11, 99, 1, 99)]
        [InlineData(2, 2, 2, 1)]
        public void Multiply_Product(int LeftRows, int LeftColumns, int RightRows, int RightCols)
        {

            IMatrix<float> left = new Float.Matrix(LeftRows, LeftColumns, 1.0f);
            IMatrix<float> right = new Float.Matrix(RightRows, RightCols, 0);

            // store these later to verify that originals are not mutated
            IMatrix<float> leftVerification = left.Duplicate();
            IMatrix<float> rightVerification = right.Duplicate();

            // make sure if the params given should throw they do
            if (LeftColumns != RightRows)
            {

                IMatrix<float> ShouldThrow()
                {
                    return left * right;
                }

                Assert.Throws<InvalidOperationException>(ShouldThrow);
                return;
            }

            IMatrix<float> result = left * right;

            // the result should always have the rows of the first and the colums of the second
            Assert.Equal(left.Rows, result.Rows);

            Assert.Equal(right.Columns, result.Columns);

            // since the inputs are ones and 0s make sure they are all zeros since that's 1 x 0

            foreach (float[] row in result)
            {
                foreach (var item in row)
                {
                    Assert.Equal(0, item);
                }
            }

            // verify that neither original matrices were mutated by the multiplication
            Assert.Equal(leftVerification, left);
            Assert.Equal(rightVerification, right);

            // change values in the original to verify .Equals is working
            left[0, 0] = 14;
            right[0, 0] = 10;

            Assert.NotEqual(leftVerification, left);
            Assert.NotEqual(rightVerification, right);

            // complete a defaul multiplication independant of params to verify specific mechanics
            left = new Float.Matrix(3, 4, new float[] { 12, 11, 10, 9, 8, 7, 6, 5, 4, 3, 2, 1 });
            right = new Float.Matrix(4, 2, new float[] { 1, 2, 3, 4, 5, 6, 7, 8 });

            leftVerification = left.Duplicate();
            rightVerification = right.Duplicate();

            result = left * right;

            // make sure the shape of the result is what we expect
            // we should expect the rows of the left and the cols of the right
            Assert.Equal(left.Rows, result.Rows);

            Assert.Equal(right.Columns, result.Columns);

            // verify the values as correct, these are manually asserted to extra-verify the accuracy of this test
            // this is probably the most important test here
            var flat = result.ToOneDimension();

            Assert.Equal(158, flat[0]);
            Assert.Equal(200, flat[1]);
            Assert.Equal(94, flat[2]);
            Assert.Equal(120, flat[3]);
            Assert.Equal(30, flat[4]);
            Assert.Equal(40, flat[5]);
        }

        [Fact]
        public void Operator_Add_DoesNotMutate()
        {
            IMatrix<double> left = new Double.Matrix(2, 2, new double[] { 1, 2, 3, 4 });
            IMatrix<double> right = new Double.Matrix(2, 2, new double[] { 4, 3, 2, 1 });

            IMatrix<double> leftStored = left.Duplicate();
            IMatrix<double> rightStored = right.Duplicate();

            // subtract
            IMatrix<double> result = left - right;

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
        public void MultiplyWorks()
        {
            IMatrix<double> left = new Double.Matrix(2, 2, new double[] { -0.79, -0.56, 0.46, 0.11 });
            IMatrix<double> right = new Double.Matrix(2, 1, new double[] { 1, 0 });

            IMatrix<double> result = left * right;

            var onedim = result.ToOneDimension();

            Assert.Equal(-0.79, onedim[0]);
            Assert.Equal(0.46, onedim[1]);

            left = new Double.Matrix(1, 2, new double[] { 0.52, -0.27 });
            right = new Double.Matrix(2, 1, new double[] { 0.68, 0.38 });

            result = left * right;

            onedim = result.ToOneDimension();

            Assert.Equal(0.251, onedim[0]);
        }
    }
}
