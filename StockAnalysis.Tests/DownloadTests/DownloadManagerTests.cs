using StockAnalysis.Download.Getter;
using StockAnalysis.Download.Manager;
using StockAnalysis.Download.Store;
using StockAnalysis.HoldingsConfig;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;

namespace StockAnalysisTests.DownloadTests;

public class DownloadManagerTests
{
    private const string StoragePath = ".";
    private const string StorageDir = "download-test";
    private WireMockServer server;

    [SetUp]
    public void Setup()
    {
        server = WireMockServer.Start(9876);
        // Most of the body is simply arbitrary data and has no effect on tests.
        server.Given(Request.Create().WithPath("/ARK_INNOVATION_ETF_ARKK_HOLDINGS.csv").UsingGet()
        ).RespondWith(
            Response
                    .Create()
                    .WithStatusCode(200)
                    .WithHeader("Content-Type", "text/csv")
                    .WithBody("date,fund,company,ticker,cusip,shares,\"market value ($)\",\"weight (%)\"\n" +
                              "04/16/2024,ARKK,\"TESLA INC\",TSLA,88160R101,\"4,028,071\",\"$650,452,905.08\",9.83%\n" +
                              "04/16/2024,ARKK,\"COINBASE GLOBAL INC -CLASS A\",COIN,19260Q107,\"2,630,233\",\"$587,620,354.53\",8.88%\n" +
                              "04/16/2024,ARKK,\"ROKU INC\",ROKU,77543R102,\"8,941,303\",\"$527,000,398.82\",7.96%\n" +
                              "04/16/2024,ARKK,\"BLOCK INC\",SQ,852234103,\"6,171,325\",\"$453,592,387.50\",6.85%\n")
            );
        server.Given(Request.Create().WithPath("/ARK_GENOMIC_REVOLUTION_ETF_ARKG_HOLDINGS.csv").UsingGet()
        ).RespondWith(
            Response
                .Create()
                .WithStatusCode(200)
                .WithHeader("Content-Type", "text/csv")
                .WithBody("date,fund,company,ticker,cusip,shares,\"market value ($)\",\"weight (%)\"\n" +
                          "04/16/2024,ARKG,\"TESLA INC\",TSLA,88160R101,\"4,028,071\",\"$650,452,905.08\",9.83%\n" +
                          "04/16/2024,ARKG,\"COINBASE GLOBAL INC -CLASS A\",COIN,19260Q107,\"2,630,233\",\"$587,620,354.53\",8.88%\n" +
                          "04/16/2024,ARKG,\"ROKU INC\",ROKU,77543R102,\"8,941,303\",\"$527,000,398.82\",7.96%\n" +
                          "04/16/2024,ARKG,\"BLOCK INC\",SQ,852234103,\"6,171,325\",\"$453,592,387.50\",6.85%\n")
        );
    }

    [TearDown]
    public void Teardown()
    {
        server.Stop();
    }

    [Test]
    public async Task GetHoldings_ValidUriSingleCsv_Succeeds()
    {
        // Arrange
        HoldingInformation[] holdings =
        {
            new("ARKK-Holdings",
                "http://localhost:9876/ARK_INNOVATION_ETF_ARKK_HOLDINGS.csv")
        };
        var resultName = holdings[0].Name + ".csv";
        var resultDir = Path.Combine(StoragePath, StorageDir);
        var fullResultPath = Path.Combine(resultDir, resultName);

        using var client = new HttpClient();
        // This is necessary, otherwise the website will reject our request.
        client.DefaultRequestHeaders.Add("User-Agent", "Other");

        var manager = new DownloadManager(StoragePath, new CsvDownload(), new CsvStorage(), client);

        // Act
        var result = await manager.GetHoldings(holdings, StorageDir);

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
                "http://localhost:9876/ARK_INNOVATION_ETF_ARKK_HOLDINGS.csv"),
            new("ARKG-Holdings",
                "http://localhost:9876/ARK_GENOMIC_REVOLUTION_ETF_ARKG_HOLDINGS.csv")
        };
        var resultDir = Path.Combine(StoragePath, StorageDir);
        using var client = new HttpClient();
        // This is necessary, otherwise the website will reject our request.
        client.DefaultRequestHeaders.Add("User-Agent", "Other");

        var manager = new DownloadManager(".", new CsvDownload(), new CsvStorage(), client);

        // Act
        var result = await manager.GetHoldings(holdings, "download-test");
        Assert.Multiple(() =>
        {
            // Assert
            Assert.That(result, Is.True);
            Assert.That(File.Exists(Path.Combine(resultDir, "ARKK-Holdings.csv")), Is.True);
            Assert.That(File.Exists(Path.Combine(resultDir, "ARKG-Holdings.csv")), Is.True);
        });

        // Cleanup.
        File.Delete(Path.Combine(resultDir, "ARKK-Holdings.csv"));
        File.Delete(Path.Combine(resultDir, "ARKG-Holdings.csv"));
        Directory.Delete(resultDir);
    }
}