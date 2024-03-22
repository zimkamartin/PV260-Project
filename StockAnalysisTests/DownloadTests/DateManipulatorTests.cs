using StockAnalysis.Download.PeriodicalDownload;
using StockAnalysis.Utilities;

namespace StockAnalysisTests.DownloadTests;

public class DateManipulatorTests
{
    [Test]
    public void GetFolderName_Today_Succeeds()
    {
        // Arrange
        var date = DateOnly.FromDateTime(DateTime.Today);
        
        // Act
        var folderName = DateManipulator.GetFolderName(date);
        
        // Assert
        Assert.That(folderName, Is.EqualTo(DateOnly.FromDateTime((DateTime.Today)).ToString()));
    }

    [Test]
    public void GetFolderName_Yesterday_Succeeds()
    {
        // Arrange
        var date = DateOnly.FromDateTime(DateTime.Today);
        
        // Act
        var yesterdayFolderName = DateManipulator.GetFolderName(date, -1, PeriodType.Daily);
        
        // Assert
        Assert.That(yesterdayFolderName, Is.EqualTo(date.AddDays(-1).ToString()));
    }

    [Test]
    public void GetFolderName_Tommorow_Succeeds()
    {
        // Arrange
        var date = DateOnly.FromDateTime(DateTime.Today);

        // Act
        var tommorowFolderName = DateManipulator.GetFolderName(date, 1, PeriodType.Daily);
        
        // Assert
        Assert.That(tommorowFolderName, Is.EqualTo(date.AddDays(1).ToString()));
    }
}