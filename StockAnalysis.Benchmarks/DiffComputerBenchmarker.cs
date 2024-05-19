using BenchmarkDotNet.Attributes;
using StockAnalysis.Diff.Compute;
using StockAnalysis.Diff.Data;

namespace StockAnalysis.Benchmarks;

/// <summary>
/// Benchmarks for the diff computer.
/// </summary>
public class DiffComputerBenchmarker
{
    private List<FundData> _oldData = null!;
    private List<FundData> _newData = null!;

    [GlobalSetup]
    public void Setup()
    {
        _oldData = new List<FundData>
        {
            new() { Ticker = "TICK1", Shares = "100", MarketValue = "1000", Weight = "10" },
            new() { Ticker = "TICK2", Shares = "200", MarketValue = "2000", Weight = "20" }
        };

        _newData = new List<FundData>
        {
            new() { Ticker = "TICK1", Shares = "150", MarketValue = "1500", Weight = "15" },
            new() { Ticker = "TICK2", Shares = "250", MarketValue = "2500", Weight = "25" },
            new() { Ticker = "TICK3", Shares = "300", MarketValue = "3000", Weight = "30" }
        };
    }

    [Benchmark]
    public void BenchmarkComputeChanges()
    {
        CsvDiffComputer.ComputeChanges(_oldData, _newData);
    }
}