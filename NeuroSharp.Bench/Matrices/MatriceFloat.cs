using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NeuroSharp.Float;

namespace NeuroSharp.Bench
{
    public class MatriceFloat
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
        public MatriceFloat()
        {

        }

        [BenchmarkDotNet.Attributes.Benchmark]
        public IMatrix<float> Test_0()
        {
            IMatrix<float> left = new Matrix(MatrixDimensions[0].Rows, MatrixDimensions[0].Columns, GetRandom());
            IMatrix<float> right = new Matrix(MatrixDimensions[0].Columns, MatrixDimensions[0].Rows, GetRandom());

            return left * right;
        }

        [BenchmarkDotNet.Attributes.Benchmark]
        public IMatrix<float> Test_1()
        {
            int num = 1;

            IMatrix<float> left = new Matrix(MatrixDimensions[num].Rows, MatrixDimensions[num].Columns, GetRandom());
            IMatrix<float> right = new Matrix(MatrixDimensions[num].Columns, MatrixDimensions[num].Rows, GetRandom());

            return left * right;
        }

        [BenchmarkDotNet.Attributes.Benchmark]
        public IMatrix<float> Test_2()
        {
            int num = 2;

            IMatrix<float> left = new Matrix(MatrixDimensions[num].Rows, MatrixDimensions[num].Columns, GetRandom());
            IMatrix<float> right = new Matrix(MatrixDimensions[num].Columns, MatrixDimensions[num].Rows, GetRandom());

            return left * right;
        }

        [BenchmarkDotNet.Attributes.Benchmark]
        public IMatrix<float> Test_3()
        {
            int num = 3;

            IMatrix<float> left = new Matrix(MatrixDimensions[num].Rows, MatrixDimensions[num].Columns, GetRandom());
            IMatrix<float> right = new Matrix(MatrixDimensions[num].Columns, MatrixDimensions[num].Rows, GetRandom());

            return left * right;
        }

        [BenchmarkDotNet.Attributes.Benchmark]
        public IMatrix<float> Test_4()
        {
            int num = 4;

            IMatrix<float> left = new Matrix(MatrixDimensions[num].Rows, MatrixDimensions[num].Columns, GetRandom());
            IMatrix<float> right = new Matrix(MatrixDimensions[num].Columns, MatrixDimensions[num].Rows, GetRandom());

            return left * right;
        }

        [BenchmarkDotNet.Attributes.Benchmark]
        public IMatrix<float> Test_5()
        {
            int num = 5;

            IMatrix<float> left = new Matrix(MatrixDimensions[num].Rows, MatrixDimensions[num].Columns, GetRandom());
            IMatrix<float> right = new Matrix(MatrixDimensions[num].Columns, MatrixDimensions[num].Rows, GetRandom());

            return left * right;
        }

        [BenchmarkDotNet.Attributes.Benchmark]
        public IMatrix<float> Test_6()
        {
            int num = 6;

            IMatrix<float> left = new Matrix(MatrixDimensions[num].Rows, MatrixDimensions[num].Columns, GetRandom());
            IMatrix<float> right = new Matrix(MatrixDimensions[num].Columns, MatrixDimensions[num].Rows, GetRandom());

            return left * right;
        }

        [BenchmarkDotNet.Attributes.Benchmark]
        public IMatrix<float> Test_7()
        {
            int num = 7;

            IMatrix<float> left = new Matrix(MatrixDimensions[num].Rows, MatrixDimensions[num].Columns, GetRandom());
            IMatrix<float> right = new Matrix(MatrixDimensions[num].Columns, MatrixDimensions[num].Rows, GetRandom());

            return left * right;
        }

        [BenchmarkDotNet.Attributes.Benchmark]
        public IMatrix<float> Test_8()
        {
            int num = 8;

            IMatrix<float> left = new Matrix(MatrixDimensions[num].Rows, MatrixDimensions[num].Columns, GetRandom());
            IMatrix<float> right = new Matrix(MatrixDimensions[num].Columns, MatrixDimensions[num].Rows, GetRandom());

            return left * right;
        }

        [BenchmarkDotNet.Attributes.Benchmark]
        public IMatrix<float> Test_9()
        {
            int num = 9;

            IMatrix<float> left = new Matrix(MatrixDimensions[num].Rows, MatrixDimensions[num].Columns, GetRandom());
            IMatrix<float> right = new Matrix(MatrixDimensions[num].Columns, MatrixDimensions[num].Rows, GetRandom());

            return left * right;
        }

        private readonly Random rng = new();
        IEnumerable<float> GetRandom()
        {
            yield return (float)rng.NextDouble();
        }
    }
}
