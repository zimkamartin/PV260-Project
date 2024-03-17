using StockAnalysis.Download;

namespace StockAnalysisTests.DownloadTests;

public class DownloadTests
{
    [Test]
    public async Task Download_GetCsv_Succeeds()
    {
        // Arrange
        // It might be better to mock a simple rest API service instead of sending requests to ark-funds.
        // Will improve this if I get the time.
        var uri = "https://ark-funds.com/wp-content/uploads/funds-etf-csv/ARK_INNOVATION_ETF_ARKK_HOLDINGS.csv";
        using var client = new HttpClient();
        
        // This is necessary, otherwise the website will reject our request.
        client.DefaultRequestHeaders.Add("User-Agent", "Other");

        try
        {
            // Act
            await using var resultStream = await Download.GetCsv(uri, client);
            
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