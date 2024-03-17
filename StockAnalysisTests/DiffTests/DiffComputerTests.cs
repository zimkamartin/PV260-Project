using StockAnalysis.Diff;

namespace StockAnalysisTests.DiffTests;

public class DiffComputerTests
{
    private string? _projectRoot;
    
    [SetUp]
    public void Setup()
    {  
        var current = Environment.CurrentDirectory;
        var projectDirectory = Directory.GetParent(current);
        _projectRoot = current;
        if (projectDirectory is not null)
        {
            _projectRoot = projectDirectory.Parent!.Parent!.FullName;
        }
    }

    //creates file diff.csv
    //TODO
    [Test]
    public async Task DiffTest()
    {
        bool result = await DiffComputer.CreateAndStoreDiff(_projectRoot);
        Assert.IsTrue(result);
    }
    
    //If changes are computwd correctly
    [Test]
    public void ComputeChanges_ShouldCalculateChangesCorrectly()
    {
        // Vygenerovanie dat 
        var oldData = new List<FundData>
        {
            new FundData { Ticker = "TICK1", Shares = "100", MarketValue = "1000", Weight = "10" },
            new FundData { Ticker = "TICK2", Shares = "200", MarketValue = "2000", Weight = "20" }
        };

        var newData = new List<FundData>
        {
            new FundData { Ticker = "TICK1", Shares = "150", MarketValue = "1500", Weight = "15" },
            new FundData { Ticker = "TICK2", Shares = "250", MarketValue = "2500", Weight = "25" },
            new FundData { Ticker = "TICK3", Shares = "300", MarketValue = "3000", Weight = "30" }
        };

        // Act
        var changes = DiffComputer.ComputeChanges(oldData, newData);

        // Assert
        Assert.AreEqual(3, changes.Count);

        Assert.AreEqual("TICK1", changes[0].Ticker);
        Assert.AreEqual(50, changes[0].SharesChange);
        Assert.AreEqual(500, changes[0].MarketValueChange);
        Assert.AreEqual(15, changes[0].Weight);

        Assert.AreEqual("TICK2", changes[1].Ticker);
        Assert.AreEqual(50, changes[1].SharesChange);
        Assert.AreEqual(500, changes[1].MarketValueChange);
        Assert.AreEqual(25, changes[1].Weight);
    }
    
    
}