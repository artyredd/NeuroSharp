using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NeuroSharp.SByte;

namespace NeuroSharp.Bench
{
    public class MatriceSByte
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
        public MatriceSByte()
        {

        }

        [BenchmarkDotNet.Attributes.Benchmark]
        public IMatrix<sbyte> Test_0()
        {
            IMatrix<sbyte> left = new Matrix(MatriceController.MatrixDimensions[0].Rows, MatriceController.MatrixDimensions[0].Columns, GetRandom());
            IMatrix<sbyte> right = new Matrix(MatriceController.MatrixDimensions[0].Columns, MatriceController.MatrixDimensions[0].Rows, GetRandom());

            return left * right;
        }

        [BenchmarkDotNet.Attributes.Benchmark]
        public IMatrix<sbyte> Test_1()
        {
            int num = 1;

            IMatrix<sbyte> left = new Matrix(MatriceController.MatrixDimensions[num].Rows, MatriceController.MatrixDimensions[num].Columns, GetRandom());
            IMatrix<sbyte> right = new Matrix(MatriceController.MatrixDimensions[num].Columns, MatriceController.MatrixDimensions[num].Rows, GetRandom());

            return left * right;
        }

        [BenchmarkDotNet.Attributes.Benchmark]
        public IMatrix<sbyte> Test_2()
        {
            int num = 2;

            IMatrix<sbyte> left = new Matrix(MatriceController.MatrixDimensions[num].Rows, MatriceController.MatrixDimensions[num].Columns, GetRandom());
            IMatrix<sbyte> right = new Matrix(MatriceController.MatrixDimensions[num].Columns, MatriceController.MatrixDimensions[num].Rows, GetRandom());

            return left * right;
        }

        [BenchmarkDotNet.Attributes.Benchmark]
        public IMatrix<sbyte> Test_3()
        {
            int num = 3;

            IMatrix<sbyte> left = new Matrix(MatriceController.MatrixDimensions[num].Rows, MatriceController.MatrixDimensions[num].Columns, GetRandom());
            IMatrix<sbyte> right = new Matrix(MatriceController.MatrixDimensions[num].Columns, MatriceController.MatrixDimensions[num].Rows, GetRandom());

            return left * right;
        }

        [BenchmarkDotNet.Attributes.Benchmark]
        public IMatrix<sbyte> Test_4()
        {
            int num = 4;

            IMatrix<sbyte> left = new Matrix(MatriceController.MatrixDimensions[num].Rows, MatriceController.MatrixDimensions[num].Columns, GetRandom());
            IMatrix<sbyte> right = new Matrix(MatriceController.MatrixDimensions[num].Columns, MatriceController.MatrixDimensions[num].Rows, GetRandom());

            return left * right;
        }

        [BenchmarkDotNet.Attributes.Benchmark]
        public IMatrix<sbyte> Test_5()
        {
            int num = 5;

            IMatrix<sbyte> left = new Matrix(MatriceController.MatrixDimensions[num].Rows, MatriceController.MatrixDimensions[num].Columns, GetRandom());
            IMatrix<sbyte> right = new Matrix(MatriceController.MatrixDimensions[num].Columns, MatriceController.MatrixDimensions[num].Rows, GetRandom());

            return left * right;
        }

        [BenchmarkDotNet.Attributes.Benchmark]
        public IMatrix<sbyte> Test_6()
        {
            int num = 6;

            IMatrix<sbyte> left = new Matrix(MatriceController.MatrixDimensions[num].Rows, MatriceController.MatrixDimensions[num].Columns, GetRandom());
            IMatrix<sbyte> right = new Matrix(MatriceController.MatrixDimensions[num].Columns, MatriceController.MatrixDimensions[num].Rows, GetRandom());

            return left * right;
        }

        [BenchmarkDotNet.Attributes.Benchmark]
        public IMatrix<sbyte> Test_7()
        {
            int num = 7;

            IMatrix<sbyte> left = new Matrix(MatriceController.MatrixDimensions[num].Rows, MatriceController.MatrixDimensions[num].Columns, GetRandom());
            IMatrix<sbyte> right = new Matrix(MatriceController.MatrixDimensions[num].Columns, MatriceController.MatrixDimensions[num].Rows, GetRandom());

            return left * right;
        }

        [BenchmarkDotNet.Attributes.Benchmark]
        public IMatrix<sbyte> Test_8()
        {
            int num = 8;

            IMatrix<sbyte> left = new Matrix(MatriceController.MatrixDimensions[num].Rows, MatriceController.MatrixDimensions[num].Columns, GetRandom());
            IMatrix<sbyte> right = new Matrix(MatriceController.MatrixDimensions[num].Columns, MatriceController.MatrixDimensions[num].Rows, GetRandom());

            return left * right;
        }

        [BenchmarkDotNet.Attributes.Benchmark]
        public IMatrix<sbyte> Test_9()
        {
            int num = 9;

            IMatrix<sbyte> left = new Matrix(MatriceController.MatrixDimensions[num].Rows, MatriceController.MatrixDimensions[num].Columns, GetRandom());
            IMatrix<sbyte> right = new Matrix(MatriceController.MatrixDimensions[num].Columns, MatriceController.MatrixDimensions[num].Rows, GetRandom());

            return left * right;
        }

        private readonly Random rng = new();
        IEnumerable<sbyte> GetRandom()
        {
            yield return (sbyte)rng.Next(-128, 128);
        }
    }
}
