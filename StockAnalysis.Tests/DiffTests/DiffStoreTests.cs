using System.Globalization;
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
    
    [Test]
    public async Task StoreDiff_shouldReturnTrueFileExists()
    {
        //act
        List<DiffData> data = DiffComputer.CreateDiff(Path.Combine(_projectRoot, "test.csv"));
        bool result = await DiffStore.StoreDiffToCsv(data, _projectRoot, "test_diff");
        
        //assert
        Assert.IsTrue(result);
        var totalPath = Path.Join(_projectRoot, "test_diff.csv");
        Assert.That(File.Exists(totalPath), Is.True);
        
        //cleanup
        File.Delete(totalPath);
        Assert.That(File.Exists(totalPath), Is.False);
    }
    
    // TODO check of content of file
    [Test]
    public async Task StoreDiff_called_rightContentInFile()
    {
        //act
        List<DiffData> data = DiffComputer.CreateDiff(Path.Combine(_projectRoot, "test.csv"));
        bool result = await DiffStore.StoreDiffToCsv(data, _projectRoot, "test_diff");
        
        var totalPath = Path.Join(_projectRoot, "test_diff.csv");
        using var reader = new StreamReader(totalPath);
        //TODO read lines and check content
        reader.ReadLine();
        //assert
        
        //cleanup
        File.Delete(totalPath);
        Assert.That(File.Exists(totalPath), Is.False);
    }
}