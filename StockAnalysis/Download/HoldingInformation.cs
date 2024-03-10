namespace StockAnalysis.Download;

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