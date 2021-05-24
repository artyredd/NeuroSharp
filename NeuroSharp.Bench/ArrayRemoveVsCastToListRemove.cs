using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuroSharp.Bench
{
    [SimpleJob(RunStrategy.Monitoring,
        warmupCount: 10, targetCount: 50)]
    public class ArrayRemoveVsCastToListRemove
    {
        [Params(2, 4, 8, 16, 28)]
        public int N { get; set; }

        private int[] numbers;
        private int[] numbersToRemove;

        [GlobalSetup]
        public void GlobalSetup()
        {
            // fill the numbers list with random numbers
            numbers = Helpers.Random.NextIntArray(1 << N, -100, 100);
            numbersToRemove = Helpers.Random.NextIntArray(1 << N, -100, 100);
        }

        [GlobalCleanup]
        public void GlobalCleanup()
        {
            numbers = null;
            numbersToRemove = null;
        }

        [Benchmark]
        public int[] Control()
        {
            List<int> nums = new(numbers);
            nums.RemoveAll(x => numbersToRemove.Contains(x));
            return nums.ToArray();
        }

        [Benchmark]
        public int[] Test()
        {
            Span<int> nums = numbersToRemove;

            int[] copy = new int[numbers.Length];
            Array.Copy(numbers, copy, numbers.Length);

            Helpers.Array.RemoveValues(ref copy, ref nums);
            return copy;
        }
    }
}
