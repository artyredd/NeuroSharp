using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuroSharp.Bench
{
    public class SpanSliceVsIndexer
    {
        [Params(1, 2, 4, 6, 8, 10, 12, 14, 18, 20, 22, 28)]
        public int N { get; set; }

        [Benchmark]
        public Span<byte> Control()
        {
            Span<byte> span = new(new byte[1 << N]);
            return span.Slice(0, span.Length - 1);
        }

        [Benchmark]
        public Span<byte> Test()
        {
            Span<byte> span = new(new byte[1 << N]);
            return span[0..^1];
        }
    }
}
