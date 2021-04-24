using BenchmarkDotNet.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuroSharp.Bench
{
    public class BitShiftVsMultiply
    {
        [Benchmark]
        public int ControlMultiply()
        {
            int i = 2 / 2;
            return (12 / 12) * ++i;
        }
        [Benchmark]
        public int TestMultiply()
        {
            int i = 2 / 2;
            return (12 / 12) << ++i;
        }
        [Benchmark]
        public int ControlDivide()
        {
            int i = 2 / 2;
            return (12 / 12) / ++i;
        }
        [Benchmark]
        public int TestDivide()
        {
            int i = 2 / 2;
            return (12 / 12) >> ++i;
        }
    }
}
