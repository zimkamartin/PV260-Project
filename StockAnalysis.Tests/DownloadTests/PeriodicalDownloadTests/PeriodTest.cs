using StockAnalysis.Download.PeriodicalDownload;

namespace StockAnalysisTests.DownloadTests.PeriodicalDownloadTests;

[TestFixture]
[TestOf(typeof(Period))]
public class PeriodTest
{
    [Test]
    public void SetIntervalFromType_WhenTypePeriodBasic_ShouldReturnAppropriateValue()
    {
        // Arrange
        var start = new DateTime(2024, 3, 10);
        var period = new Period(PeriodType.Daily, start);

        // Act
        var expected = TimeSpan.FromDays(1);

        // Assert
        Assert.That(period.SetIntervalFromType(), Is.EqualTo(expected));
    }

    [Test]
    public void SetIntervalFromType_WhenTypePeriodCustom_ShouldThrowArgumentOutOfRangeException()
    {
        // Arrange
        var start = new DateTime(2024, 3, 10);
        var interval = TimeSpan.FromDays(3);
        var period = new Period(start, interval);

        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => period.SetIntervalFromType());
    }

    [Test]
    public void TimeToGo_WhenCurrentIsBeforeStart_ShouldReturnCorrectTimeSpan()
    {
        // Arrange
        var start = new DateTime(2024, 3, 10);
        var period = new Period(PeriodType.Weekly, start);
        var current = new DateTime(2024, 3, 7);

        // Act
        var expected = TimeSpan.FromDays(3);

        // Assert
        Assert.That(period.TimeToGo(current), Is.EqualTo(expected));
    }

    [Test]
    public void TimeToGo_WhenCurrentIsAfterStart_ShouldReturnCorrectTimeSpan()
    {
        // Arrange
        var start = new DateTime(2024, 3, 10);
        var period = new Period(PeriodType.Weekly, start);
        var current = new DateTime(2024, 3, 14);

        // Act
        var expected = TimeSpan.FromDays(3);

        // Assert
        Assert.That(period.TimeToGo(current), Is.EqualTo(expected));
    }
}