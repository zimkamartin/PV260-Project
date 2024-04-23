using StockAnalysis.Diff.Store;

namespace StockAnalysisTests.DiffTests;

public class DiffStoreCreatorTests
{
    [Test]
    public void CreateStore_Csv_RegularFormat_Succeeds()
    {
        try
        {
            _ = DiffStoreCreator.CreateStore(".csv");
        }
        catch (Exception)
        {
            Assert.Fail("The method threw an unexpected exception.");
        }
    }

    [Test]
    public void CreateStore_Csv_AllCaps_Succeeds()
    {
        try
        {
            _ = DiffStoreCreator.CreateStore(".CSV");
        }
        catch (Exception)
        {
            Assert.Fail("The method threw an unexpected exception.");
        }
    }

    [Test]
    public void CreateStore_UnknownExtension_Throws()
    {
        try
        {
            _ = DiffStoreCreator.CreateStore("UNKNOWN");
            Assert.Fail("The method was supposed to throw.");
        }
        catch (NotImplementedException)
        {
        }
    }
}