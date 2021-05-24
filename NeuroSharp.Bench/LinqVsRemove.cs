using BenchmarkDotNet.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuroSharp.Bench
{
    public class LinqVsRemove
    {
        [Params(2, 4, 8, 16, 30)]
        public int N { get; set; }

        [Benchmark]
        public int[] Control()
        {
            int[] nums = new int[1 << N];

            nums[nums.Length >> 2] = 4;

            int numToRemove = 4;

            int numIndex = Array.IndexOf(nums, numToRemove);

            nums = nums.Where((val, idx) => idx != numIndex).ToArray();

            return nums;
        }

        [Benchmark]
        public int[] Test()
        {
            int[] nums = new int[1 << N];

            nums[nums.Length >> 2] = 4;

            int numToRemove = 4;

            return Helpers.Array.RemoveValue(ref nums, ref numToRemove);
        }
    }
}
