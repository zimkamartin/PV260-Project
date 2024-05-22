using StockAnalysis.Diff.Compute;

namespace StockAnalysisTests.DiffTests.DiffComputerTests;

public class DiffComputerCreatorTests
{
    [Test]
    public void CreateComputer_Csv_RegularFormat_Succeeds()
    {
        try
        {
            _ = DiffComputerCreator.CreateComputer(".csv");
        }
        catch (Exception)
        {
            Assert.Fail("The method threw an unexpected exception.");
        }
    }

    [Test]
    public void CreateComputer_Csv_AllCaps_Succeeds()
    {
        try
        {
            _ = DiffComputerCreator.CreateComputer(".CSV");
        }
        catch (Exception)
        {
            Assert.Fail("The method threw an unexpected exception.");
        }
    }

    [Test]
    public void CreateComputer_UnknownExtension_Throws()
    {
        try
        {
            _ = DiffComputerCreator.CreateComputer("UNKNOWN");
            Assert.Fail("The method was supposed to throw.");
        }
        catch (NotImplementedException)
        {
        }
    }
}