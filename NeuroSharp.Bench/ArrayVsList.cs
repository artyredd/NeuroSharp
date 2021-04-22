using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
namespace NeuroSharp.Bench
{
    public class ArrayVsList
    {
        [Benchmark]
        public Span<ushort> Control()
        {
            List<ushort> vals = new List<ushort>() { 1, 2, 3, 4, 5, 6, 7, 8, 9 };

            vals.Add(0);

            ushort[] arr = vals.ToArray();

            return new(arr);
        }

        [Benchmark]
        public Span<ushort> Test()
        {
            ushort[] vals = new ushort[9] { 1, 2, 3, 4, 5, 6, 7, 8, 9 };

            Array.Resize(ref vals, 10);

            vals[9] = 0;

            return new(vals);
        }
    }
}
