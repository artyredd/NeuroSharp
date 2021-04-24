using BenchmarkDotNet.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuroSharp.Bench
{
    public class ArrayResizeVsSpan
    {
        [Benchmark]
        public int Control()
        {
            int[] arr = new int[9] { 1, 2, 3, 4, 5, 6, 7, 8, 9 };

            Array.Resize(ref arr, 10);

            arr[9] = 3;

            return arr[9];
        }

        [Benchmark]
        public int Test()
        {
            int[] arr = new int[9] { 1, 2, 3, 4, 5, 6, 7, 8, 9 };

            Span<int> copSpan = new(new int[arr.Length + 1]);

            arr.CopyTo(copSpan);

            arr = copSpan.ToArray();

            arr[9] = 3;

            return arr[9];
        }
    }
}
