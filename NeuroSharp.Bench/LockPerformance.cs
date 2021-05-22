using BenchmarkDotNet.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuroSharp.Bench
{
    public class LockPerformance
    {
        TestClass x = new TestClass();

        [Benchmark]
        public bool Control()
        {
            return x.status;
        }

        [Benchmark]
        public bool Test()
        {
            lock (x)
            {
                return x.status;
            }
        }
    }
    class TestClass
    {
        public bool status => Helpers.Random.Next(0, 2) == 1 ? true : false;
    }
}
