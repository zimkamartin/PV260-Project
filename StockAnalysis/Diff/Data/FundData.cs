using CsvHelper.Configuration.Attributes;

namespace StockAnalysis.Diff.Data;

public class FundData
{
    [Name("date")] public DateTime Date { get; set; }

    [Name("fund")] public string Fund { get; set; } = null!;

    [Name("company")] public string Company { get; set; } = null!;

    [Name("ticker")] public string Ticker { get; set; } = null!;

    [Name("cusip")] public string Cusip { get; set; } = null!;

    [Name("shares")] public string Shares { get; set; } = null!;

    [Name("market value ($)")] public string MarketValue { get; set; } = null!;

    [Name("weight (%)")] public string Weight { get; set; } = null!;
}