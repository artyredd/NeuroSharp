using BenchmarkDotNet.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuroSharp.Bench
{
    public class IndividualVsArrayRNG
    {
        // n = 2^1 - 2&20
        [Params(1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20)]
        public int ArraySize { get; set; }

        [Benchmark]
        public async Task<double[]> Control()
        {
            double[] nums = new double[2 << ArraySize];
            for (int i = 0; i < nums.Length; i++)
            {
                nums[i] = await Helpers.Random.NextDoubleAsync();
            }
            return nums;
        }

        [Benchmark]
        public async Task<double[]> Test()
        {
            return await Helpers.Random.NextDoubleArray(2 << ArraySize);
        }
    }
}
