using StockAnalysis.Download;
using StockAnalysis.HoldingsConfig;


namespace StockAnalysisTests.DownloadTests;

public class DownloadTests
{
    [Test]
    public async Task DownloadSingleCsv()
    {
        HoldingInformation[] holdings =
        {
            new("ARKK-Holdings",
                "https://ark-funds.com/wp-content/uploads/funds-etf-csv/ARK_INNOVATION_ETF_ARKK_HOLDINGS.csv")
        };
        var manager = new DownloadManager(".");
        using var client = new HttpClient();
        
        // This is necessary, otherwise the website will reject our request.
        client.DefaultRequestHeaders.Add("User-Agent", "Other");
        
        Assert.That(await manager.DownloadHoldingsCsv(holdings, client), Is.True);
        Assert.That(File.Exists("./ARKK-Holdings.csv"), Is.True);
        
        // Cleanup.
        File.Delete("./ARKK-Holdings.csv");
        Assert.That(File.Exists("./ARKK-Holdings.csv"), Is.False);
    }

    [Test]
    public async Task DownloadMultipleCsv()
    {
        HoldingInformation[] holdings =
        {
            new("ARKK-Holdings",
                "https://ark-funds.com/wp-content/uploads/funds-etf-csv/ARK_INNOVATION_ETF_ARKK_HOLDINGS.csv"),
            new("ARKG-Holdings",
                "https://ark-funds.com/wp-content/uploads/funds-etf-csv/ARK_GENOMIC_REVOLUTION_ETF_ARKG_HOLDINGS.csv")
        };
        var manager = new DownloadManager(".");
        using var client = new HttpClient();
        
        // This is necessary, otherwise the website will reject our request.
        client.DefaultRequestHeaders.Add("User-Agent", "Other");
        
        Assert.That(await manager.DownloadHoldingsCsv(holdings, client), Is.True);
        Assert.That(File.Exists("./ARKK-Holdings.csv"), Is.True);
        Assert.That(File.Exists("./ARKG-Holdings.csv"), Is.True);
        
        // Cleanup.
        File.Delete("./ARKK-Holdings.csv");
        Assert.That(File.Exists("./ARKK-Holdings.csv"), Is.False);
        File.Delete("./ARKG-Holdings.csv");
        Assert.That(File.Exists("./ARKG-Holdings.csv"), Is.False);
        
    }
}