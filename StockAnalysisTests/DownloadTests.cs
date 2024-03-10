using StockAnalysis.Download;

namespace StockAnalysisTests;

public class DownloadTests
{
    [SetUp]
    public void Setup()
    {
        
    }

    [Test]
    public async Task DownloadSingleCsv()
    {
        HoldingInformation[] holdings =
        {
            new HoldingInformation("ARKK-Holdings",
                "https://ark-funds.com/wp-content/uploads/funds-etf-csv/ARK_INNOVATION_ETF_ARKK_HOLDINGS.csv")
        };
        var manager = new DownloadManager(".");
        var client = new HttpClient();
        client.DefaultRequestHeaders.Add("User-Agent", "Other");
        Assert.That(await manager.DownloadHoldingsCsv(holdings, client), Is.True);
        Assert.That(File.Exists("./ARKK-Holdings.csv"), Is.True);
    }
}