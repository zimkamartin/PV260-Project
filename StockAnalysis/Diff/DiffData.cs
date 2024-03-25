namespace StockAnalysis.Diff;

public class DiffData
{
    public string Company { get; set; }
    public string Ticker { get; set; }
    public double SharesChange { get; set; }
    public double MarketValueChange { get; set; }
    public double Weight { get; set; }
    public bool NewEntry { get; set; }
}