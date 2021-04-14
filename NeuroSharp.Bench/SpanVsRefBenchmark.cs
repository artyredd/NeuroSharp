using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BenchmarkDotNet.Configs;
using NeuroSharp.Int;

namespace NeuroSharp.Bench
{
    [BenchmarkDotNet.Attributes.MemoryDiagnoser]
    public class SpanVsRefBenchmark
    {
        const int Rows = 10_000;
        const int Columns = 11_000;

        readonly Random rng = new();

        public SpanVsRefBenchmark()
        {
            /*
                we should see if tranposing a matrix with spans is faster than using indicies mapping is faster 
            */
        }

        [BenchmarkDotNet.Attributes.Benchmark]
        public int Normal()
        {
            Matrix matrix = new(Rows, Columns, GetRandom());

            var transposed = matrix.Transpose();

            int sum = 0;

            for (int rows = 0; rows < transposed.Rows; rows++)
            {
                for (int cols = 0; cols < transposed.Columns; cols++)
                {
                    sum += transposed[rows, cols];
                }
            }

            return sum;
        }

        [BenchmarkDotNet.Attributes.Benchmark]
        public int Span()
        {
            Matrix matrix = new(Rows, Columns, GetRandom());

            var transposed = matrix.TransposeWithMutableSpan();

            int sum = 0;

            for (int rows = 0; rows < transposed.Rows; rows++)
            {
                for (int cols = 0; cols < transposed.Columns; cols++)
                {
                    sum += transposed[rows, cols];
                }
            }

            return sum;
        }

        IEnumerable<int> GetRandom()
        {
            yield return rng.Next();
        }
    }
}
