using FakeItEasy;
using StockAnalysis.Constants;
using StockAnalysis.Download.PeriodicalDownload;
using StockAnalysis.Utilities;
using StockAnalysisConsole;
using Times = FakeItEasy.Times;


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
        using var client = new HttpClient();
        const string extension = Constants.CsvExtension;

        var start = new DateTime(2024, 3, 10);
        const int count = 3;
        var interval = TimeSpan.FromSeconds(1);
        var period = new Period(start, interval);

        var analysisManager = A.Fake<AnalysisManager>();
        A.CallTo(() => analysisManager.PerformAnalysis(client, extension, extension, period))
            .Returns(new List<string>());

        var dateTimeProvider = A.Fake<IDateTimeProvider>();
        A.CallTo(() => dateTimeProvider.UtcNow()).Returns(start);

        var periodicalDownloader = new PeriodicalDownloader(period, dateTimeProvider, client, extension,
            extension,
            analysisManager.PerformAnalysis);
        // Act
        var timer = periodicalDownloader.SchedulePeriodicDownload();

        // Wait 
        Thread.Sleep((count + 1) * 1000);

        timer.Dispose();

        // Assert
        A.CallTo(() => analysisManager.PerformAnalysis(client, extension, extension, period)).MustHaveHappened(count, Times.OrMore);
    }

}