using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuroSharp.Bench
{
    public class ArrayForeachBenchmark<T>
    {
        public int MaxRows { get; set; } = 10_000;

        public int MaxColumns { get; set; } = 10_000;

        public ArrayForeachBenchmark()
        {
            // PURPOSE: To see if applying a function member-wise is more efficient using build inlibrary or manually
        }

        [BenchmarkDotNet.Attributes.Benchmark]
        public T[][] Control()
        {
            T[][] testArray = new T[MaxRows][];

            T[] Converter(T[] arr)
            {
                for (int j = 0; j < arr.Length; j++)
                {
                    arr[j] = GenerateValue();
                }
                return arr;
            }

            for (int i = 0; i < MaxRows; i++)
            {
                testArray[i] = new T[MaxColumns];
                Array.ConvertAll(testArray, Converter);
            }

            return testArray;
        }

        [BenchmarkDotNet.Attributes.Benchmark]
        public T[][] Test()
        {
            T[][] testArray = new T[MaxRows][];

            for (int i = 0; i < MaxRows; i++)
            {

                testArray[i] = new T[MaxColumns];

                // create the default values
                for (int j = 0; j < MaxColumns; j++)
                {
                    testArray[i][j] = GenerateValue() ?? default;
                }
            }

            return testArray;
        }

        private T GenerateValue()
        {
            return default;
        }
    }
}
