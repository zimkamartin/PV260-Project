using System.Diagnostics;
using StockAnalysis.Download.Getter;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;

namespace StockAnalysisTests.DownloadTests;

public class CsvDownloadTests
{
    private WireMockServer _server = null!;

    [SetUp]
    public void Setup()
    {
        _server = WireMockServer.Start(9876);
        // Most of the body is simply arbitrary data and has no effect on tests.
        _server.Given(Request.Create().WithPath("/ARK_INNOVATION_ETF_ARKK_HOLDINGS.csv").UsingGet()
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
        _server.Given(Request.Create().WithPath("/ARK_GENOMIC_REVOLUTION_ETF_ARKG_HOLDINGS.csv").UsingGet()
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
        _server.Stop();
    }

    [Test]
    public async Task Download_GetCsv_Succeeds()
    {
        // Arrange
        const string uri =
            "http://localhost:9876/ARK_INNOVATION_ETF_ARKK_HOLDINGS.csv";
        using var client = new HttpClient();

        // This is necessary, otherwise the website will reject our request.
        client.DefaultRequestHeaders.Add("User-Agent", "Other");

        var downloader = new CsvDownload();

        // Act
        await using var resultStream = await downloader.Get(uri, client);
        Assert.Multiple(() =>
        {

            // Assert
            // The stream is not null and there is some data in the stream.
            Assert.That(resultStream is not null);
            Assert.That(resultStream!.Length, Is.GreaterThan(0));
        });
        // Needed properties of the stream.
        Assert.Multiple(() =>
        {
            Assert.That(resultStream!.CanRead, Is.True);
            Assert.That(resultStream.CanSeek, Is.True);
        });
    }
}