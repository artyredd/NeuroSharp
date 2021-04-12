using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using NeuroSharp;

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
            var matrix = new Matrix<int>(rows, columns);

            Assert.Equal(matrix.Rows, rows);

            Assert.Equal(matrix.Columns, columns);
        }

        [Theory]
        [InlineData(0, 0)]
        [InlineData(-1, 0)]
        [InlineData(0, -1)]
        [InlineData(1, 0)]
        [InlineData(0, 1)]
        [InlineData(int.MinValue, int.MinValue)]
        [InlineData(1, int.MinValue)]
        [InlineData(int.MinValue, 1)]
        [InlineData(int.MaxValue, int.MaxValue)]
        [InlineData(1, int.MaxValue)]
        [InlineData(int.MaxValue, 1)]
        public void Constructor_Normal_Throws_Index(int rows, int columns)
        {
            // matrix should throw in certain circumstances
            void ShouldThrow()
            {
                var matrix = new Matrix<float>(rows, columns);
            }

            Assert.Throws<ArgumentOutOfRangeException>(ShouldThrow);
        }

        [Theory]
        [InlineData(2_147_000_000, 2_147_000_000)]
        public void Constructor_Normal_Throws_Memory(int rows, int columns)
        {
            // matrix should throw in certain circumstances
            void ShouldThrow()
            {
                var matrix = new Matrix<float>(rows, columns);
            }

            Assert.Throws<OutOfMemoryException>(ShouldThrow);
        }

        [Fact]
        public void Constructor_Normal_FillsDefault()
        {
            // it should be expected that when we create a new matrix all the values are defaulted to not-null
            var matrix = new Matrix<int>(10, 10);

            foreach (var row in matrix)
            {
                foreach (var element in row)
                {
                    Assert.Equal(default, element);
                }
            }

            var floatMatrix = new Matrix<float>(10, 10);

            foreach (var row in floatMatrix)
            {
                foreach (var element in row)
                {
                    Assert.Equal(default, element);
                }
            }

            var decimalmatrix = new Matrix<decimal>(10, 10);

            foreach (var row in decimalmatrix)
            {
                foreach (var element in row)
                {
                    Assert.Equal(default, element);
                }
            }

            var byteMatrix = new Matrix<byte>(10, 10);

            foreach (var row in byteMatrix)
            {
                foreach (var element in row)
                {
                    Assert.Equal(default, element);
                }
            }

            var doubleMatrix = new Matrix<double>(10, 10);

            foreach (var row in doubleMatrix)
            {
                foreach (var element in row)
                {
                    Assert.Equal(default, element);
                }
            }
            var sbyteMatrix = new Matrix<sbyte>(10, 10);

            foreach (var row in sbyteMatrix)
            {
                foreach (var element in row)
                {
                    Assert.Equal(default, element);
                }
            }
            var shortMatrix = new Matrix<short>(10, 10);

            foreach (var row in shortMatrix)
            {
                foreach (var element in row)
                {
                    Assert.Equal(default, element);
                }
            }
            var ushortMatrix = new Matrix<ushort>(10, 10);

            foreach (var row in ushortMatrix)
            {
                foreach (var element in row)
                {
                    Assert.Equal(default, element);
                }
            }
            var uintMatrix = new Matrix<uint>(10, 10);

            foreach (var row in uintMatrix)
            {
                foreach (var element in row)
                {
                    Assert.Equal(default, element);
                }
            }
            var longMatrix = new Matrix<long>(10, 10);

            foreach (var row in longMatrix)
            {
                foreach (var element in row)
                {
                    Assert.Equal(default, element);
                }
            }

            var ulongMatrix = new Matrix<ulong>(10, 10);

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
            var matrix = new Matrix<int>(10, 10, data);

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

            var matrix = new Matrix<int>(10, 10, TestCallMethod);

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

            var matrix = new Matrix<int>(10, 10, TestCallMethod);

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

            var matrix = new Matrix<int>(10, 10, TestCallMethod);

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

            var matrix = new Matrix<float>(10, 10, enumerable);

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

            var stringMatrix = new Matrix<decimal>(10, 10, NullEnumberable());

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

            Matrix<int> boolMatrix = new(10, 10, GetInts());

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
            var matrix = new Matrix<int>(10, 10);

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
            var matrix = new Matrix<int>(10, 10);

            Assert.Equal(100, matrix.Capacity);
        }

        [Fact]
        public void Tranpose_Normal_Works()
        {
            // make sure the matrix rotats correctly
            int x = 0;
            var matrix = new Matrix<int>(2, 3, () => x++);
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
        }

        [Fact]
        public void ApplyMemberWiseOperation_Works()
        {
            System.Random random = new();

            var sourceMatrix = new Matrix<int>(100, 100, () => random.Next(int.MinValue >> 3, int.MaxValue >> 3));

            int[] nums = sourceMatrix.ToArray();

            var matrix = new Matrix<int>(100, 100, nums);

            // make sure the list mapped correctly by sampling one number
            Assert.Equal(nums[10], matrix[0][10]);

            matrix.ApplyMemberWiseOperation((ref int x) => x *= 2);

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
            var matrix = new Matrix<int>(2, 3, new int[] { 0, 1, 2, 3, 4, 5 });

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
    }
}
