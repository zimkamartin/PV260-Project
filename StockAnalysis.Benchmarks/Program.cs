// See https://aka.ms/new-console-template for more information

using BenchmarkDotNet.Running;
using StockAnalysis.Benchmarks;

BenchmarkRunner.Run<DiffComputerBenchmarker>();