using StockAnalysis.Download.PeriodicalDownload;
using StockAnalysis.Utilities;

namespace StockAnalysisTests.DownloadTests;

public class DateManipulatorTests
{
    [TestCase(2020, 04, 15, "15042020")]
    [TestCase(2021, 12, 31, "31122021")]
    public void GetFolderName_Simple(int year, int month, int day, string expected)
    {
        // Arrange
        var date = DateOnly.FromDateTime(new DateTime(year, month, day));
        
        // Act
        var folderName = DateManipulator.GetFolderName(date);
        
        // Assert
        Assert.That(folderName, Is.EqualTo(expected));
    }

    [TestCase(2020, 04, 15, 1, "16042020")]
    [TestCase(2021, 12, 31, 1,"01012022")]
    [TestCase(2021, 02, 05, -1, "04022021")]
    [TestCase(2021, 01, 01, -1, "31122020")]
    [TestCase(2021, 01, 01, 3, "04012021")]
    [TestCase(2021, 02, 03, -3, "31012021")]
    public void GetFolderName_DailyPeriod(int year, int month, int day, int step, string expected)
    {
        // Arrange
        var date = DateOnly.FromDateTime(new DateTime(year, month, day));
        // Act
        var yesterdayFolderName = DateManipulator.GetFolderName(date, step, PeriodType.Daily);
        
        // Assert
        Assert.That(yesterdayFolderName, Is.EqualTo(expected));
    }
    
    [TestCase(2020, 04, 15, 1, "15052020")]
    [TestCase(2020, 12, 15, 1, "15012021")]
    [TestCase(2020, 01, 15, -1, "15122019")]
    [TestCase(2020, 04, 15, 12, "15042021")]
    [TestCase(2020, 04, 15, -12, "15042019")]
    public void GetFolderName_MonthlyPeriod(int year, int month, int day, int step, string expected)
    {
        // Arrange
        var date = DateOnly.FromDateTime(new DateTime(year, month, day));
        // Act
        var yesterdayFolderName = DateManipulator.GetFolderName(date, step, PeriodType.Monthly);
        
        // Assert
        Assert.That(yesterdayFolderName, Is.EqualTo(expected));
    }
    
    [TestCase(2020, 04, 15, 1, "22042020")]
    [TestCase(2020, 12, 31, 1, "07012021")]
    [TestCase(2020, 01, 15, -1, "08012020")]
    [TestCase(2020, 01, 07, -1, "31122019")]
    public void GetFolderName_WeeklyPeriod(int year, int month, int day, int step, string expected)
    {
        // Arrange
        var date = DateOnly.FromDateTime(new DateTime(year, month, day));
        // Act
        var yesterdayFolderName = DateManipulator.GetFolderName(date, step, PeriodType.Weekly);
        
        // Assert
        Assert.That(yesterdayFolderName, Is.EqualTo(expected));
    }

    [TestCase(2020, 1, 1, 1, "01042020")]
    [TestCase(2020,2,1, 1, "01052020")]
    [TestCase(2020,3,1, 1, "01062020")]
    [TestCase(2020,4,1, 1,"01072020")]
    [TestCase(2020,10,1, 1, "01012021")]
    [TestCase(2020,11,1, 1, "01022021")]
    [TestCase(2020,12,1, 1, "01032021")]
    [TestCase(2020, 1, 1, -1, "01102019")]
    [TestCase(2020,2,1, -1,  "01112019")]
    [TestCase(2020,3,1, -1, "01122019")]
    [TestCase(2020,4,1, -1, "01012020")]
    [TestCase(2020,5,1, -1, "01022020")]
    public void GetFolderName_QuarterlyPeriod( int year, int month, int day, int step, string expected)
    {
        // Arrange
        var date = DateOnly.FromDateTime(new DateTime(year, month, day));
        
        // Act
        var actual = DateManipulator.GetFolderName(date, step, PeriodType.Quarterly);
        
        // Assert
        Assert.That(actual, Is.EqualTo(expected));
    }

    [TestCase(2020, 05, 01, 1, 2, "03052020" )]
    [TestCase(2020, 05, 01, 1, 3, "04052020" )]
    [TestCase(2020, 05, 01, 2, 2, "05052020" )]
    [TestCase(2020, 05, 01, -1, 2, "29042020" )]
    [TestCase(2020, 01, 01, -1, 2, "30122019" )]
    [TestCase(2020, 04, 15, 1, 7, "22042020")]
    [TestCase(2021, 12, 31, 1,1,"01012022")]
    public void GetFolderName_CustomPeriod(int year, int month, int day, int step, int spanDays, string expected)
    {
        // Arrange
        var span = new TimeSpan(spanDays, 0, 0, 0);
        var period = new Period(DateTime.Now, span);
        var date = DateOnly.FromDateTime(new DateTime(year, month, day));
        
        // Act
        var actual = DateManipulator.GetFolderName(date, step, period);
        
        // Assert
        Assert.That(actual, Is.EqualTo(expected));
    }
}