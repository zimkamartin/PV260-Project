using CsvHelper.Configuration.Attributes;

namespace StockAnalysis.Diff.Data;

public class FundData
{
    [Name("date")] public DateTime Date { get; set; }

    [Name("fund")] public string Fund { get; set; }

    [Name("company")] public string Company { get; set; }

    [Name("ticker")] public string Ticker { get; set; }

    [Name("cusip")] public string Cusip { get; set; }

    [Name("shares")] public string Shares { get; set; }

    [Name("market value ($)")] public string MarketValue { get; set; }

    [Name("weight (%)")] public string Weight { get; set; }
}