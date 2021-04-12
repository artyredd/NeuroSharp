using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuroSharp.Bench
{

    public class ArrayFillBenchmark<T> where T : unmanaged, IComparable, IComparable<T>, IEquatable<T>, IConvertible
    {
        public int MaxRows { get; set; } = 10_000;

        public int MaxColumns { get; set; } = 10_000;

        public T DefaultValue { get; set; }

        public ArrayFillBenchmark()
        {
            // RESULT: FILL ALWAYS FASTER

            //BenchmarkRunner.Run<ArrayFillBenchmark<byte>>();
            ///*
            //|  Method |     Mean |    Error |   StdDev |
            //|-------- |---------:|---------:|---------:|
            //| Control | 67.52 ms | 0.819 ms | 0.766 ms |
            //|    Test | 88.69 ms | 1.746 ms | 1.793 ms |
            //*/
            //BenchmarkRunner.Run<ArrayFillBenchmark<int>>();
            ///*
            //|  Method |     Mean |   Error |  StdDev |
            //|-------- |---------:|--------:|--------:|
            //| Control | 154.7 ms | 3.07 ms | 7.35 ms |
            //|    Test | 188.1 ms | 3.73 ms | 8.42 ms |
            //*/
            //BenchmarkRunner.Run<ArrayFillBenchmark<float>>();
            ///*
            //|  Method |     Mean |   Error |   StdDev |
            //|-------- |---------:|--------:|---------:|
            //| Control | 154.0 ms | 3.07 ms |  6.34 ms |
            //|    Test | 260.1 ms | 5.13 ms | 10.37 ms | 
            //*/
            //BenchmarkRunner.Run<ArrayFillBenchmark<decimal>>();
            ///*
            //|  Method |     Mean |   Error |   StdDev |   Median |
            //|-------- |---------:|--------:|---------:|---------:|
            //| Control | 224.4 ms | 5.80 ms | 16.27 ms | 216.3 ms |
            //|    Test | 384.2 ms | 7.67 ms | 17.46 ms | 380.6 ms | 
            //*/
        }

        public T[][] Control()
        {
            // test built in array fill

            T[][] testArray = new T[MaxRows][];

            for (int i = 0; i < MaxRows; i++)
            {

                testArray[i] = new T[MaxColumns];
                Array.Fill(testArray[i], DefaultValue);
            }

            // iterate over array to force the bench to behave consistently
            VerifyResults(testArray);

            return testArray;
        }

        [BenchmarkDotNet.Attributes.Benchmark]
        public T[][] Control_Alternate()
        {
            // test built in array fill

            T[][] testArray = new T[MaxRows][];

            for (int i = 0; i < MaxRows; i++)
            {

                testArray[i] = new T[MaxColumns];
                testArray[i].Initialize();
            }

            // iterate over array to force the bench to behave consistently
            VerifyResults(testArray);

            return testArray;
        }

        [BenchmarkDotNet.Attributes.Benchmark]
        public T[][] Test()
        {
            T[][] testArray = new T[MaxRows][];

            var emptyArray = new T[MaxColumns];
            Array.Fill(emptyArray, DefaultValue);

            Array.Fill(testArray, emptyArray.Clone());

            // iterate over array to force the bench to behave consistently
            VerifyResults(testArray);

            return testArray;
        }

        private void VerifyResults(T[][] Result)
        {
            foreach (var item in Result)
            {
                foreach (var item1 in item)
                {
                    if (item1.Equals(DefaultValue) is false)
                    {
                        throw new OperationCanceledException();
                    }
                }
            }
        }
    }
}
