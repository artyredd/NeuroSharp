using NeuroSharp.Int;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuroSharp.Bench
{
    public class MultiplyScalarBenchmark
    {
        const int Rows = 10_000;

        const int Columns = 10_000;

        private readonly Random rng = new();

        public MultiplyScalarBenchmark()
        {

        }

        [BenchmarkDotNet.Attributes.Benchmark]
        public int[] Control()
        {
            IMatrix<int> matrix = new BaseMatrix<int>(Rows, Columns, GetRandom())
            {
                IntegerOperations = new()
            };

            matrix.IntegerOperations.ReferenceMultiplier = (int scalar) => ((ref int matrixElement) => matrixElement *= scalar);

            matrix *= 2;

            var arr = matrix.ToOneDimension();

            foreach (var item in arr)
            {
                _ = item;
            }

            return arr;
        }

        [BenchmarkDotNet.Attributes.Benchmark]
        public int[] Test()
        {
            IMatrix<int> matrix = new Matrix(Rows, Columns, GetRandom());

            matrix = matrix * 2;

            var arr = matrix.ToOneDimension();

            foreach (var item in arr)
            {
                _ = item;
            }

            return arr;
        }

        IEnumerable<int> GetRandom()
        {
            yield return rng.Next();
        }
    }
}
