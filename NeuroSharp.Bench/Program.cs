using BenchmarkDotNet;
using BenchmarkDotNet.Running;
using NeuroSharp.Bench;
using System;

BenchmarkRunner.Run<ArrayFillBenchmark<int>>();
/*
|  Method |     Mean |   Error |  StdDev |
|-------- |---------:|--------:|--------:|
| Control | 154.7 ms | 3.07 ms | 7.35 ms |
|    Test | 188.1 ms | 3.73 ms | 8.42 ms |
*/
BenchmarkRunner.Run<ArrayFillBenchmark<float>>();
/*
|  Method |     Mean |   Error |   StdDev |
|-------- |---------:|--------:|---------:|
| Control | 154.0 ms | 3.07 ms |  6.34 ms |
|    Test | 260.1 ms | 5.13 ms | 10.37 ms | 
*/
BenchmarkRunner.Run<ArrayFillBenchmark<decimal>>();
/*
|  Method |     Mean |   Error |   StdDev |   Median |
|-------- |---------:|--------:|---------:|---------:|
| Control | 224.4 ms | 5.80 ms | 16.27 ms | 216.3 ms |
|    Test | 384.2 ms | 7.67 ms | 17.46 ms | 380.6 ms | 
*/
int[][] testArray = new int[3][];

var emptyArray = new int[3];

Array.Fill(emptyArray, 13);

Array.Fill(testArray, emptyArray.Clone());

foreach (var item in testArray)
{
    foreach (var item1 in item)
    {
        if (item1.Equals(13) is false)
        {
            throw new OperationCanceledException();
        }
    }
}