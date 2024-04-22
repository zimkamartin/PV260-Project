namespace StockAnalysis.HoldingsConfig;

public struct Holdings
{
    public IEnumerable<HoldingInformation> HoldingInfo { get; set; }
}

/// <summary>
/// A structure that contains information about individual ETF holdings.
/// The name is used to later construct files for downloaded data, while the
/// uri itself must be the endpoint for a GET request that can download data
/// about the given ETF holding.
/// </summary>
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