using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NeuroSharp.Short;

namespace NeuroSharp.Bench
{
    public class MatriceShort
    {
        public (int Rows, int Columns)[] MatrixDimensions = {
            (2,3),
            (4,5),
            (8,9),
            (16,17),
            (32,33),
            (64,65),
            (128,129),
            (256,257),
            (512,513)
        };
        public MatriceShort()
        {

        }

        [BenchmarkDotNet.Attributes.Benchmark]
        public IMatrix<short> Test_0()
        {
            IMatrix<short> left = new Matrix(MatrixDimensions[0].Rows, MatrixDimensions[0].Columns, GetRandom());
            IMatrix<short> right = new Matrix(MatrixDimensions[0].Columns, MatrixDimensions[0].Rows, GetRandom());

            return left * right;
        }

        [BenchmarkDotNet.Attributes.Benchmark]
        public IMatrix<short> Test_1()
        {
            int num = 1;

            IMatrix<short> left = new Matrix(MatrixDimensions[num].Rows, MatrixDimensions[num].Columns, GetRandom());
            IMatrix<short> right = new Matrix(MatrixDimensions[num].Columns, MatrixDimensions[num].Rows, GetRandom());

            return left * right;
        }

        [BenchmarkDotNet.Attributes.Benchmark]
        public IMatrix<short> Test_2()
        {
            int num = 2;

            IMatrix<short> left = new Matrix(MatrixDimensions[num].Rows, MatrixDimensions[num].Columns, GetRandom());
            IMatrix<short> right = new Matrix(MatrixDimensions[num].Columns, MatrixDimensions[num].Rows, GetRandom());

            return left * right;
        }

        [BenchmarkDotNet.Attributes.Benchmark]
        public IMatrix<short> Test_3()
        {
            int num = 3;

            IMatrix<short> left = new Matrix(MatrixDimensions[num].Rows, MatrixDimensions[num].Columns, GetRandom());
            IMatrix<short> right = new Matrix(MatrixDimensions[num].Columns, MatrixDimensions[num].Rows, GetRandom());

            return left * right;
        }

        [BenchmarkDotNet.Attributes.Benchmark]
        public IMatrix<short> Test_4()
        {
            int num = 4;

            IMatrix<short> left = new Matrix(MatrixDimensions[num].Rows, MatrixDimensions[num].Columns, GetRandom());
            IMatrix<short> right = new Matrix(MatrixDimensions[num].Columns, MatrixDimensions[num].Rows, GetRandom());

            return left * right;
        }

        [BenchmarkDotNet.Attributes.Benchmark]
        public IMatrix<short> Test_5()
        {
            int num = 5;

            IMatrix<short> left = new Matrix(MatrixDimensions[num].Rows, MatrixDimensions[num].Columns, GetRandom());
            IMatrix<short> right = new Matrix(MatrixDimensions[num].Columns, MatrixDimensions[num].Rows, GetRandom());

            return left * right;
        }

        [BenchmarkDotNet.Attributes.Benchmark]
        public IMatrix<short> Test_6()
        {
            int num = 6;

            IMatrix<short> left = new Matrix(MatrixDimensions[num].Rows, MatrixDimensions[num].Columns, GetRandom());
            IMatrix<short> right = new Matrix(MatrixDimensions[num].Columns, MatrixDimensions[num].Rows, GetRandom());

            return left * right;
        }

        [BenchmarkDotNet.Attributes.Benchmark]
        public IMatrix<short> Test_7()
        {
            int num = 7;

            IMatrix<short> left = new Matrix(MatrixDimensions[num].Rows, MatrixDimensions[num].Columns, GetRandom());
            IMatrix<short> right = new Matrix(MatrixDimensions[num].Columns, MatrixDimensions[num].Rows, GetRandom());

            return left * right;
        }

        [BenchmarkDotNet.Attributes.Benchmark]
        public IMatrix<short> Test_8()
        {
            int num = 8;

            IMatrix<short> left = new Matrix(MatrixDimensions[num].Rows, MatrixDimensions[num].Columns, GetRandom());
            IMatrix<short> right = new Matrix(MatrixDimensions[num].Columns, MatrixDimensions[num].Rows, GetRandom());

            return left * right;
        }

        [BenchmarkDotNet.Attributes.Benchmark]
        public IMatrix<short> Test_9()
        {
            int num = 9;

            IMatrix<short> left = new Matrix(MatrixDimensions[num].Rows, MatrixDimensions[num].Columns, GetRandom());
            IMatrix<short> right = new Matrix(MatrixDimensions[num].Columns, MatrixDimensions[num].Rows, GetRandom());

            return left * right;
        }

        private readonly Random rng = new();
        IEnumerable<short> GetRandom()
        {
            yield return (short)rng.Next(-32_768, 32_768);
        }
    }
}
