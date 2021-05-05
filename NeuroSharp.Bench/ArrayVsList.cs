using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Diagnostics.Windows.Configs;

namespace NeuroSharp.Bench
{
    [RPlotExporter]
    [SimpleJob(warmupCount: 4, targetCount: 5)]
    public class ArrayVsList
    {
        [Params(1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 15, 20)]
        public int N { get; set; }

        private int[] DataArray;
        private List<int> DataList;

        [Benchmark]
        public Span<int> Control()
        {
            DataArray = Helpers.NextIntArrayAsync(1 << N, -100, 101).Result;
            List<int> vals = DataList;

            vals.Add(0);

            int[] arr = vals.ToArray();

            return new(arr);
        }

        [Benchmark]
        public Span<int> Test()
        {
            int[] vals = DataArray;

            Array.Resize(ref vals, 10);

            vals[9] = 0;

            return new(vals);
        }
    }
}
