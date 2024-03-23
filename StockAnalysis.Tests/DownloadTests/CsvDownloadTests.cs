using StockAnalysis.Download;
using StockAnalysis.Download.Getter;

namespace StockAnalysisTests.DownloadTests;

public class CsvDownloadTests
{
    [Test]
    public async Task Download_GetCsv_Succeeds()
    {
        // Arrange
        // It might be better to mock a simple rest API service instead of sending requests to ark-funds.
        // Will improve this if I get the time.
        const string uri = "https://ark-funds.com/wp-content/uploads/funds-etf-csv/ARK_INNOVATION_ETF_ARKK_HOLDINGS.csv";
        using var client = new HttpClient();
        
        // This is necessary, otherwise the website will reject our request.
        client.DefaultRequestHeaders.Add("User-Agent", "Other");

        var downloader = new CsvDownload();

        try
        {
            // Act
            await using var resultStream = await downloader.Get(uri, client);
            
            // Assert
            // There is some data in the stream.
            Assert.That( resultStream.Length, Is.GreaterThan(0) );
            // Needed properties of the stream.
            Assert.Multiple(() =>
            {
                Assert.That(resultStream.CanRead, Is.True);
                Assert.That(resultStream.CanSeek, Is.True);
            });
        }
        catch (Exception e)
        {
            // Assert
            Assert.Fail("Method threw an exception when it shouldn't have: " + e.Message);
        }
    }
}