using BenchmarkDotNet;
using BenchmarkDotNet.Running;
using NeuroSharp.Bench;
using System;

BenchmarkRunner.Run<RefVsCopyBenchmark<byte>>();

BenchmarkRunner.Run<RefVsCopyBenchmark<short>>();

BenchmarkRunner.Run<RefVsCopyBenchmark<int>>();

BenchmarkRunner.Run<RefVsCopyBenchmark<float>>();

BenchmarkRunner.Run<RefVsCopyBenchmark<decimal>>();
