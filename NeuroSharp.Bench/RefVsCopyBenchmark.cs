using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuroSharp.Bench
{
    public class RefVsCopyBenchmark<T>
    {
        public int[] RowSteps = {
                   10,
                  100,
                1_000,
               10_000
        };

        public RefVsCopyBenchmark()
        {

        }

        [BenchmarkDotNet.Attributes.Benchmark]
        public void Control()
        {
            foreach (var item in RowSteps)
            {
                var matrix = ControlMatrix<T>(item, item);

                ApplyMemberWiseOperation((ref T val) => val, matrix, item, item);
            }
        }

        [BenchmarkDotNet.Attributes.Benchmark]
        public void Test()
        {
            foreach (var item in RowSteps)
            {
                var matrix = ControlMatrix<T>(item, item);

                ApplyMemberWiseOperation((val) => val, matrix, item, item);
            }
        }

        public delegate T MemberWiseOperation(ref T Member);
        public void ApplyMemberWiseOperation(MemberWiseOperation Operation, T[][] _Matrix, int Rows, int Columns)
        {
            for (int row = 0; row < Rows; row++)
            {
                for (int column = 0; column < Columns; column++)
                {
                    Operation(ref _Matrix[row][column]);
                }
            }
        }

        public void ApplyMemberWiseOperation(Func<T, T> Operation, T[][] _Matrix, int Rows, int Columns)
        {
            for (int row = 0; row < Rows; row++)
            {
                for (int column = 0; column < Columns; column++)
                {
                    _Matrix[row][column] = Operation(_Matrix[row][column]);
                }
            }
        }

        public T[][] Control_RowParams()
        {
            foreach (var MatrixSize in RowSteps)
            {
                T[][] testArray = ControlMatrix<T>(MatrixSize, MatrixSize);

                // iterate the array to make sure the references were valid
                foreach (var item in testArray)
                {
                    _ = item;
                }
            }

            return default;
        }

        public T[][] Test_RowParams()
        {
            for (int i = 0; i < RowSteps.Length; i++)
            {
                ref int MatrixSize = ref RowSteps[i];

                T[][] testArray = RefMatrix<T>(ref MatrixSize, ref MatrixSize);

                // iterate the array to make sure the references were valid
                foreach (var item in testArray)
                {
                    _ = item;
                }
            }

            return default;
        }

        U[][] ControlMatrix<U>(int Rows, int Columns)
        {
            var matrix = new U[Rows][];

            for (int i = 0; i < Rows; i++)
            {
                matrix[i] = new U[Columns];
                matrix[i].Initialize();
            }

            return matrix;
        }

        U[][] RefMatrix<U>(ref int Rows, ref int Columns)
        {
            var matrix = new U[Rows][];

            for (int i = 0; i < Rows; i++)
            {
                matrix[i] = new U[Columns];
                matrix[i].Initialize();
            }

            return matrix;
        }
    }
}
