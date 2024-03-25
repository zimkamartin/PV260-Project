namespace StockAnalysis.Diff.Data;

public class DiffData
{
    public string Company { get; set; } = null!;
    public string Ticker { get; set; } = null!;
    public double SharesChange { get; set; }
    public double MarketValueChange { get; set; }
    public double Weight { get; set; }
    public bool NewEntry { get; set; }
}