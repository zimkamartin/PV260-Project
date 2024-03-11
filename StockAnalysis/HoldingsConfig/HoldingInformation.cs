namespace StockAnalysis.Download;

public struct Holdings
{
    public IEnumerable<HoldingInformation> HoldingInfo { get; set;  }
}

public struct HoldingInformation
{
    public string Name { get; set; }
    public string Uri { get; set; }

    public HoldingInformation(string name, string uri)
    {
        Name = name;
        Uri = uri;
    }
}