using BenchmarkDotNet;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Running;
using NeuroSharp.Bench;
using System;

BenchmarkRunner.Run<MultiplyScalarBenchmark>(DefaultConfig.Instance.AddDiagnoser(MemoryDiagnoser.Default));