using Moq;
using StockAnalysis.Download;
using StockAnalysis.Download.Getter;
using StockAnalysis.Download.PeriodicalDownload;
using StockAnalysis.Download.Store;
using StockAnalysis.HoldingsConfig;
using StockAnalysis.Utilities;

namespace StockAnalysisTests.DownloadTests.PeriodicalDownloadTests;

[TestFixture]
[TestOf(typeof(PeriodicalDownloader))]
public class PeriodicalDownloaderTest
{
    // [Ignore("Not deterministic test.")]
    [Test]
    // Main point was to test if the 'GetData' method is called during some period X times.
    // It is not deterministic - sometimes it can fail because of the uncertainty of time.
    // It seems to work as expected, but maybe testing should be done in a different way.
    public void SchedulePeriodicDownload_WhenCustomPeriodIsSet_ShouldPeriodicallyDownload()
    {
        // Arrange
        HoldingInformation[] holdings =
        {
            new("ARKK-Holdings",
                "https://ark-funds.com/wp-content/uploads/funds-etf-csv/ARK_INNOVATION_ETF_ARKK_HOLDINGS.csv")
        };
        using var client = new HttpClient();
        
        var downloaderManagerMock = new Mock<DownloadManager>(".", new CsvDownload(), new CsvStorage());
        downloaderManagerMock.Setup(x => x.GetHoldings(holdings, client, ".")).ReturnsAsync(true);

        var start = new DateTime(2024, 3, 10);
        const int count = 3;
        var interval = TimeSpan.FromSeconds(1);
        var period = new Period(start, interval);

        var dateTimeProviderMock = new Mock<IDateTimeProvider>();
        dateTimeProviderMock.Setup(x => x.UtcNow()).Returns(start);

        var periodicalDownloaderMock =
            new Mock<PeriodicalDownloader>(downloaderManagerMock.Object, period, dateTimeProviderMock.Object, holdings, client);

        // Act
        var timer = periodicalDownloaderMock.Object.SchedulePeriodicDownload();
        // Wait 
        Thread.Sleep((count + 1) * 1000);

        timer.Dispose();

        // Assert
        downloaderManagerMock.Verify(x => x.GetHoldings(holdings, client, "."), Times.AtLeast(count));
        // periodicalDownloaderMock.Verify(x => x.Downloader.GetData(), Times.Exactly(count));
    }
}