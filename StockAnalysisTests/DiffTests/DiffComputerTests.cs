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
}