using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using NeuroSharp.NEAT;
namespace NeuroSharp.Bench
{
    public class HashCodevsCustomHash
    {
        Innovation Inn = new()
        {
            Enabled = true,
            InputNode = 2,
            OutputNode = 4,
            Weight = 0.887199928829d
        };
        [Benchmark]
        public int Control()
        {
            return Inn.GetHashCode();
        }
        [Benchmark]
        public string Test()
        {
            return Inn.Hash();
        }
    }
}
