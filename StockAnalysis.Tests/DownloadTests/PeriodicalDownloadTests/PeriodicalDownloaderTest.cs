using Moq;
using StockAnalysis.Constants;
using StockAnalysis.Diff.Compute;
using StockAnalysis.Diff.Load;
using StockAnalysis.Diff.Store;
using StockAnalysis.Download;
using StockAnalysis.Download.Getter;
using StockAnalysis.Download.Manager;
using StockAnalysis.Download.PeriodicalDownload;
using StockAnalysis.Download.Store;
using StockAnalysis.HoldingsConfig;
using StockAnalysis.Utilities;
using StockAnalysisConsole;
using StockAnalysisConsole.Utils.Paths;

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
        var extension = Constants.CsvExtension;

        var start = new DateTime(2024, 3, 10);
        const int count = 3;
        var interval = TimeSpan.FromSeconds(1);
        var period = new Period(start, interval);

        var manager = new DownloadManager(Paths.GetDownloadFolderPath(), 
                                            new CsvDownload(),
                                            new CsvStorage(),
                                            client);
        
        var analysisManagerMock = new Mock<AnalysisManager>(manager, 
                                                            new CsvDiffComputer(new CsvHoldingLoader()),
                                                            new CsvDiffStore());
        // ReSharper disable once AccessToDisposedClosure
        analysisManagerMock.Setup(x => x.PerformAnalysis(client, extension, extension, period)).ReturnsAsync(new List<string>());

        var dateTimeProviderMock = new Mock<IDateTimeProvider>();
        dateTimeProviderMock.Setup(x => x.UtcNow()).Returns(start);

        var periodicalDownloaderMock =
            new Mock<PeriodicalDownloader>(period, dateTimeProviderMock.Object, holdings, client, extension, 
                extension,
#pragma warning disable CS8974 // Converting method group to non-delegate type
                analysisManagerMock.Object.PerformAnalysis);
#pragma warning restore CS8974 // Converting method group to non-delegate type

        // Act
        var timer = periodicalDownloaderMock.Object.SchedulePeriodicDownload();

        // Wait 
        Thread.Sleep((count + 1) * 1000);

        timer.Dispose();

        // Assert
        // ReSharper disable once AccessToDisposedClosure
        analysisManagerMock.Verify(x => x.PerformAnalysis(client, extension, extension, period), Times.AtLeast(count));
    }
}