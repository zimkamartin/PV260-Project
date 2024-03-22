using StockAnalysis.Download;
using StockAnalysis.Download.Getter;
using StockAnalysis.Download.Store;
using StockAnalysis.HoldingsConfig;


namespace StockAnalysisTests.DownloadTests;

public class DownloadManagerTests
{
    private const string StoragePath = ".";
    private const string StorageDir = "download-test";
    
    [Test]
    public async Task GetHoldings_ValidUriSingleCsv_Succeeds()
    {
        // Arrange
        HoldingInformation[] holdings =
        {
            new("ARKK-Holdings",
                "https://ark-funds.com/wp-content/uploads/funds-etf-csv/ARK_INNOVATION_ETF_ARKK_HOLDINGS.csv")
        };
        var resultName = holdings[0].Name + ".csv";
        var resultDir = Path.Combine(StoragePath, StorageDir);
        var fullResultPath = Path.Combine(resultDir, resultName);
        
        var manager = new DownloadManager(StoragePath, new CsvDownload(), new CsvStorage());
        using var client = new HttpClient();
        
        // This is necessary, otherwise the website will reject our request.
        client.DefaultRequestHeaders.Add("User-Agent", "Other");

        // Act
        var result = await manager.GetHoldings(holdings, client, StorageDir);
        
        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.True);
            Assert.That(File.Exists(fullResultPath), Is.True);
        });

        // Cleanup.
        File.Delete(fullResultPath);
        Directory.Delete(resultDir);
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
        var resultDir = Path.Combine(StoragePath, StorageDir);
        var manager = new DownloadManager(".", new CsvDownload(), new CsvStorage());
        using var client = new HttpClient();
        
        // This is necessary, otherwise the website will reject our request.
        client.DefaultRequestHeaders.Add("User-Agent", "Other");
        
        // Act
        var result = await manager.GetHoldings(holdings, client, "download-test");
        Assert.Multiple(() =>
        {
            // Assert
            Assert.That(result, Is.True);
            Assert.That(File.Exists(Path.Combine(resultDir,"ARKK-Holdings.csv")), Is.True);
            Assert.That(File.Exists(Path.Combine(resultDir,"ARKG-Holdings.csv")), Is.True);
        });

        // Cleanup.
        File.Delete(Path.Combine(resultDir,"ARKK-Holdings.csv"));
        File.Delete(Path.Combine(resultDir,"ARKG-Holdings.csv"));
        Directory.Delete(resultDir);
    }
}