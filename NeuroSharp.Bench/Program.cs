using BenchmarkDotNet;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Environments;
using BenchmarkDotNet.Exporters;
using BenchmarkDotNet.Exporters.Csv;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;
using NeuroSharp.Bench;
using System;

BenchmarkRunner.Run(
    typeof(LockPerformance),
    DefaultConfig.Instance
    .AddDiagnoser(MemoryDiagnoser.Default)
    .AddExporter(RPlotExporter.Default)
    .AddExporter(CsvExporter.Default)
);