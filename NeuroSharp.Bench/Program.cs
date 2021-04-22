using BenchmarkDotNet;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Running;
using NeuroSharp.Bench;
using System;


BenchmarkRunner.Run(typeof(DictionaryArraysVsDictionaryLists), DefaultConfig.Instance.AddDiagnoser(MemoryDiagnoser.Default));
//BenchmarkRunner.Run(typeof(HashCodevsCustomHash), DefaultConfig.Instance.AddDiagnoser(MemoryDiagnoser.Default));
//BenchmarkRunner.Run(typeof(MatriceSByte), DefaultConfig.Instance.AddDiagnoser(MemoryDiagnoser.Default));
//BenchmarkRunner.Run(typeof(MatriceByte), DefaultConfig.Instance.AddDiagnoser(MemoryDiagnoser.Default));
//BenchmarkRunner.Run(typeof(MatriceShort), DefaultConfig.Instance.AddDiagnoser(MemoryDiagnoser.Default));
//BenchmarkRunner.Run(typeof(MatriceInt), DefaultConfig.Instance.AddDiagnoser(MemoryDiagnoser.Default));
//BenchmarkRunner.Run(typeof(MatriceLong), DefaultConfig.Instance.AddDiagnoser(MemoryDiagnoser.Default));
//BenchmarkRunner.Run(typeof(MatriceFloat), DefaultConfig.Instance.AddDiagnoser(MemoryDiagnoser.Default));
//BenchmarkRunner.Run(typeof(MatriceDouble), DefaultConfig.Instance.AddDiagnoser(MemoryDiagnoser.Default));
//BenchmarkRunner.Run(typeof(MatriceDecimal),  DefaultConfig.Instance.AddDiagnoser(MemoryDiagnoser.Default));