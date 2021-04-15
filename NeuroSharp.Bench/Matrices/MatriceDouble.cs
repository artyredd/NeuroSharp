using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NeuroSharp.Double;

namespace NeuroSharp.Bench
{
    public class MatriceDouble
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
        public MatriceDouble()
        {

        }

        [BenchmarkDotNet.Attributes.Benchmark]
        public IMatrix<double> Test_0()
        {
            IMatrix<double> left = new Matrix(MatrixDimensions[0].Rows, MatrixDimensions[0].Columns, GetRandom());
            IMatrix<double> right = new Matrix(MatrixDimensions[0].Columns, MatrixDimensions[0].Rows, GetRandom());

            return left * right;
        }

        [BenchmarkDotNet.Attributes.Benchmark]
        public IMatrix<double> Test_1()
        {
            int num = 1;

            IMatrix<double> left = new Matrix(MatrixDimensions[num].Rows, MatrixDimensions[num].Columns, GetRandom());
            IMatrix<double> right = new Matrix(MatrixDimensions[num].Columns, MatrixDimensions[num].Rows, GetRandom());

            return left * right;
        }

        [BenchmarkDotNet.Attributes.Benchmark]
        public IMatrix<double> Test_2()
        {
            int num = 2;

            IMatrix<double> left = new Matrix(MatrixDimensions[num].Rows, MatrixDimensions[num].Columns, GetRandom());
            IMatrix<double> right = new Matrix(MatrixDimensions[num].Columns, MatrixDimensions[num].Rows, GetRandom());

            return left * right;
        }

        [BenchmarkDotNet.Attributes.Benchmark]
        public IMatrix<double> Test_3()
        {
            int num = 3;

            IMatrix<double> left = new Matrix(MatrixDimensions[num].Rows, MatrixDimensions[num].Columns, GetRandom());
            IMatrix<double> right = new Matrix(MatrixDimensions[num].Columns, MatrixDimensions[num].Rows, GetRandom());

            return left * right;
        }

        [BenchmarkDotNet.Attributes.Benchmark]
        public IMatrix<double> Test_4()
        {
            int num = 4;

            IMatrix<double> left = new Matrix(MatrixDimensions[num].Rows, MatrixDimensions[num].Columns, GetRandom());
            IMatrix<double> right = new Matrix(MatrixDimensions[num].Columns, MatrixDimensions[num].Rows, GetRandom());

            return left * right;
        }

        [BenchmarkDotNet.Attributes.Benchmark]
        public IMatrix<double> Test_5()
        {
            int num = 5;

            IMatrix<double> left = new Matrix(MatrixDimensions[num].Rows, MatrixDimensions[num].Columns, GetRandom());
            IMatrix<double> right = new Matrix(MatrixDimensions[num].Columns, MatrixDimensions[num].Rows, GetRandom());

            return left * right;
        }

        [BenchmarkDotNet.Attributes.Benchmark]
        public IMatrix<double> Test_6()
        {
            int num = 6;

            IMatrix<double> left = new Matrix(MatrixDimensions[num].Rows, MatrixDimensions[num].Columns, GetRandom());
            IMatrix<double> right = new Matrix(MatrixDimensions[num].Columns, MatrixDimensions[num].Rows, GetRandom());

            return left * right;
        }

        [BenchmarkDotNet.Attributes.Benchmark]
        public IMatrix<double> Test_7()
        {
            int num = 7;

            IMatrix<double> left = new Matrix(MatrixDimensions[num].Rows, MatrixDimensions[num].Columns, GetRandom());
            IMatrix<double> right = new Matrix(MatrixDimensions[num].Columns, MatrixDimensions[num].Rows, GetRandom());

            return left * right;
        }

        [BenchmarkDotNet.Attributes.Benchmark]
        public IMatrix<double> Test_8()
        {
            int num = 8;

            IMatrix<double> left = new Matrix(MatrixDimensions[num].Rows, MatrixDimensions[num].Columns, GetRandom());
            IMatrix<double> right = new Matrix(MatrixDimensions[num].Columns, MatrixDimensions[num].Rows, GetRandom());

            return left * right;
        }

        [BenchmarkDotNet.Attributes.Benchmark]
        public IMatrix<double> Test_9()
        {
            int num = 9;

            IMatrix<double> left = new Matrix(MatrixDimensions[num].Rows, MatrixDimensions[num].Columns, GetRandom());
            IMatrix<double> right = new Matrix(MatrixDimensions[num].Columns, MatrixDimensions[num].Rows, GetRandom());

            return left * right;
        }

        private readonly Random rng = new();
        IEnumerable<double> GetRandom()
        {
            yield return rng.NextDouble();
        }
    }
}
