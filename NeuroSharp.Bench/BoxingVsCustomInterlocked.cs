using BenchmarkDotNet.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NeuroSharp.Bench
{
    [RPlotExporter]
    [SimpleJob(warmupCount: 10, targetCount: 10)]
    public class BoxingVsCustomInterlocked
    {
        [Params(1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 12, 14, 16, 18, 20)]
        public int N { get; set; }

        // control
        private volatile int NextNodeNumber = 0;
        // control

        // test
        private volatile ushort NextUShort = 0;
        private readonly object NextUShortLock = new();
        // test

        [Benchmark]
        public ushort Control()
        {
            ushort x = 0;
            for (int i = 0; i < (2 << N); i++)
            {
                x = IncrementNextNodeId();
            }
            return x;
        }

        [Benchmark]
        public ushort Test()
        {
            ushort x = 0;
            for (int i = 0; i < (2 << N); i++)
            {
                lock (NextUShortLock)
                {
                    x = NextUShort++;
                }
            }
            return x;
        }
        public ushort IncrementNextNodeId()
        {
            return (ushort)Interlocked.Increment(ref NextNodeNumber);
        }
    }
}
