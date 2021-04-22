using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;

namespace NeuroSharp.Bench
{
    public class SpanVsArrayIteration
    {
        [Benchmark]
        public int Control()
        {
            int[] nums = new int[10] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };

            int sum = 0;

            foreach (var item in nums)
            {
                sum += item;
            }

            return sum;
        }

        [Benchmark]
        public int Test()
        {
            int[] nums = new int[10] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };

            Span<int> numSpan = new(nums);

            int sum = 0;

            foreach (var item in numSpan)
            {
                sum += item;
            }

            return sum;
        }
    }
}
