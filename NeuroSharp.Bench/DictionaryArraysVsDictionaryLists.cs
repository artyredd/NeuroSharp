using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
namespace NeuroSharp.Bench
{

    public class DictionaryArraysVsDictionaryLists
    {

        [Benchmark]
        public Span<int> Control()
        {
            Dictionary<int, List<int>> dict = new();

            dict.Add(13, new() { 1, 2, 3, 4, 5, 6, 7, 8, 9 });

            List<int> arr = dict[13];

            arr.Add(0);

            return new(arr.ToArray());
        }

        [Benchmark]
        public Span<int> Test()
        {
            Dictionary<int, int[]> dict = new();

            dict.Add(13, new int[9] { 1, 2, 3, 4, 5, 6, 7, 8, 9 });

            int[] arr = dict[13];

            Array.Resize(ref arr, 10);

            arr[9] = 0;

            dict[13] = arr;

            return new(arr);
        }
    }
}
