using NUnit.Framework.Internal;
using StockAnalysis.Diff;

namespace StockAnalysisTests.DiffTests;

public class DiffStoreTests
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
    [Test]
    public async Task StoreDiff_shouldReturnTrue()
    {
        List<DiffData> data = DiffComputer.CreateDiff(Path.Combine(_projectRoot, "test.csv"));
        bool result = await DiffStore.StoreDiffToCsv(data, _projectRoot, "test_diff");
        Assert.IsTrue(result);
    }
}