using StockAnalysis.Download;
using StockAnalysis.HoldingsConfig;


namespace StockAnalysisTests.DownloadTests;

public class DownloadManagerTests
{
    [Test]
    public async Task DownloadHoldingsCsv_Succeeds()
    {
        // Arrange
        HoldingInformation[] holdings =
        {
            new("ARKK-Holdings",
                "https://ark-funds.com/wp-content/uploads/funds-etf-csv/ARK_INNOVATION_ETF_ARKK_HOLDINGS.csv")
        };
        var manager = new DownloadManager(".");
        using var client = new HttpClient();
        
        // This is necessary, otherwise the website will reject our request.
        client.DefaultRequestHeaders.Add("User-Agent", "Other");

        // Act
        var result = await manager.DownloadHoldingsCsv(holdings, client);
        
        // Assert
        Assert.That(result, Is.True);
        Assert.That(File.Exists("./ARKK-Holdings-new.csv"), Is.True);
        
        // Cleanup.
        File.Delete("./ARKK-Holdings-new.csv");
        Assert.That(File.Exists("./ARKK-Holdings-new.csv"), Is.False);
    }

    [Test]
    public async Task ManageDownload_MultipleCsv_Succeeds()
    {
        // Arrange
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
        
        // Act
        var result = await manager.DownloadHoldingsCsv(holdings, client);
        Assert.Multiple(() =>
        {

            // Assert
            Assert.That(result, Is.True);
            Assert.That(File.Exists("./ARKK-Holdings-new.csv"), Is.True);
            Assert.That(File.Exists("./ARKG-Holdings-new.csv"), Is.True);
        });

        // Cleanup.
        File.Delete("./ARKK-Holdings-new.csv");
        Assert.That(File.Exists("./ARKK-Holdings-new.csv"), Is.False);
        File.Delete("./ARKG-Holdings-new.csv");
        Assert.That(File.Exists("./ARKG-Holdings-new.csv"), Is.False);
    }
}