using StockAnalysis.Download.Manager;

namespace StockAnalysisTests.DownloadTests;

public class ManagerCreatorTests
{
    private const string FilePath = "path";
    
    [Test]
    public void CreateManager_Csv_RegularFormat_Succeeds()
    {
        var client = new HttpClient();

        try
        {
            _ = ManagerCreator.CreateManager(FilePath, client, ".csv");
        }
        catch (NotImplementedException)
        {
            Assert.Fail("The method threw an unexpected exception.");
        }
    }

    [Test]
    public void CreateManager_Csv_AllCaps_Succeeds()
    {
        var client = new HttpClient();

        try
        {
            _ = ManagerCreator.CreateManager(FilePath, client, ".CSV");
        }
        catch (NotImplementedException)
        {
            Assert.Fail("The method threw an unexpected exception.");
        }
    }

    [Test]
    public void CreateManager_UnknownFormat_Throws()
    {
        var client = new HttpClient();

        try
        {
            _ = ManagerCreator.CreateManager(FilePath, client, "UNKNOWN");
            Assert.Fail("The method did not throw an exception.");
        }
        catch (NotImplementedException)
        {
        }
    }
}