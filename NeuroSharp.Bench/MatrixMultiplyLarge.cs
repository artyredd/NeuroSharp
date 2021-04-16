using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BenchmarkDotNet;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;

namespace NeuroSharp.Bench
{
    [SimpleJob(RunStrategy.ColdStart, targetCount: 5)]
    public class MatrixMultiplyLarge
    {
        public (int Rows, int Columns)[] MatrixDimensions = {
            (1_024,1_025),
            (2_048,2_049),
            (4_096,4_097),
            (8_192,8_193),
            (16_384,16_385),
            (32_768,32_769),
            (65_536,65_537),
        };
        public MatrixMultiplyLarge()
        {

        }

        [BenchmarkDotNet.Attributes.Benchmark]
        public IMatrix<decimal> Test_0()
        {
            IMatrix<decimal> left = new Decimal.Matrix(MatriceController.MatrixDimensions[0].Rows, MatriceController.MatrixDimensions[0].Columns, GetRandom());
            IMatrix<decimal> right = new Decimal.Matrix(MatriceController.MatrixDimensions[0].Columns, MatriceController.MatrixDimensions[0].Rows, GetRandom());

            return left * right;
        }

        [BenchmarkDotNet.Attributes.Benchmark]
        public IMatrix<decimal> Test_1()
        {
            int num = 1;

            IMatrix<decimal> left = new Decimal.Matrix(MatriceController.MatrixDimensions[num].Rows, MatriceController.MatrixDimensions[num].Columns, GetRandom());
            IMatrix<decimal> right = new Decimal.Matrix(MatriceController.MatrixDimensions[num].Columns, MatriceController.MatrixDimensions[num].Rows, GetRandom());

            return left * right;
        }

        private readonly Random rng = new();
        IEnumerable<decimal> GetRandom()
        {
            yield return (decimal)rng.NextDouble();
        }
    }
}
