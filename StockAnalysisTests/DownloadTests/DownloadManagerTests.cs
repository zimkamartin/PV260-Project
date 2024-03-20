using StockAnalysis.Download;
using StockAnalysis.Download.Getter;
using StockAnalysis.Download.Store;
using StockAnalysis.HoldingsConfig;


namespace StockAnalysisTests.DownloadTests;

public class DownloadManagerTests
{
    [Test]
    public async Task GetHoldings_ValidUriSingleCsv_Succeeds()
    {
        // Arrange
        HoldingInformation[] holdings =
        {
            new("ARKK-Holdings",
                "https://ark-funds.com/wp-content/uploads/funds-etf-csv/ARK_INNOVATION_ETF_ARKK_HOLDINGS.csv")
        };
        var manager = new DownloadManager(".", new CsvDownload(), new CsvStorage());
        using var client = new HttpClient();
        
        // This is necessary, otherwise the website will reject our request.
        client.DefaultRequestHeaders.Add("User-Agent", "Other");

        // Act
        var result = await manager.GetHoldings(holdings, client);
        
        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.True);
            Assert.That(File.Exists("./ARKK-Holdings-new.csv"), Is.True);
        });

        // Cleanup.
        File.Delete("./ARKK-Holdings-new.csv");
        Assert.That(File.Exists("./ARKK-Holdings-new.csv"), Is.False);
    }

    [Test]
    public async Task GetHoldings_ValidUriMultipleCsv_Succeeds()
    {
        // Arrange
        HoldingInformation[] holdings =
        {
            new("ARKK-Holdings",
                "https://ark-funds.com/wp-content/uploads/funds-etf-csv/ARK_INNOVATION_ETF_ARKK_HOLDINGS.csv"),
            new("ARKG-Holdings",
                "https://ark-funds.com/wp-content/uploads/funds-etf-csv/ARK_GENOMIC_REVOLUTION_ETF_ARKG_HOLDINGS.csv")
        };
        var manager = new DownloadManager(".", new CsvDownload(), new CsvStorage());
        using var client = new HttpClient();
        
        // This is necessary, otherwise the website will reject our request.
        client.DefaultRequestHeaders.Add("User-Agent", "Other");
        
        // Act
        var result = await manager.GetHoldings(holdings, client);
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