using StockAnalysis.Download.Manager;

namespace StockAnalysisTests.DownloadTests;

public class ManagerCreatorTests
{
    [Test]
    public void CreateManager_Csv_RegularFormat_Succeeds()
    {
        var filePath = "path";
        var client = new HttpClient();
        
        try
        {
            _ = ManagerCreator.CreateManager(filePath, client, ".csv");
        }
        catch (NotImplementedException)
        {
            Assert.Fail("The method threw an unexpected exception.");
        }
    }

    [Test]
    public void CreateManager_Csv_AllCaps_Succeeds()
    {
        var filePath = "path";
        var client = new HttpClient();
        
        try
        {
            _ = ManagerCreator.CreateManager(filePath, client, ".CSV");
        }
        catch (NotImplementedException)
        {
            Assert.Fail("The method threw an unexpected exception.");
        }
    }

    [Test]
    public void CreateManager_UnknownFormat_Throws()
    {
        var filePath = "path";
        var client = new HttpClient();
        
        try
        {
            _ = ManagerCreator.CreateManager(filePath, client, "UNKNOWN");
            Assert.Fail("The method did not throw an exception.");
        }
        catch (NotImplementedException)
        {
        }
    }
}