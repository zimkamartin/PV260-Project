using StockAnalysis.Constants;
using StockAnalysis.Diff.Compute;
using StockAnalysis.Diff.Data;
using StockAnalysis.Diff.Load;
using StockAnalysis.Diff.Store;

namespace StockAnalysisTests.DiffTests;

public class HtmlDiffStoreTests
{
    private string? _testdataRoot;

    [SetUp]
    public void Setup()
    {
        var current = Environment.CurrentDirectory;
        var projectDirectory = Directory.GetParent(current);
        _testdataRoot = current;
        if (projectDirectory is not null)
        {
            _testdataRoot = Path.Combine(projectDirectory.Parent!.Parent!.FullName, "TestData");
        }
    }

    [Test]
    public async Task StoreDiff_WhenCalledRight_ShouldCreateFile()
    {
        //arrange
        IDiffStore storage = new HtmlDiffStore();
        IDiffCompute computer = new CsvDiffComputer(new CsvHoldingLoader());
        var data = computer.CreateDiff(
            Path.Combine(_testdataRoot!, "testfiles_new", "test.csv"), 
            null);
        var totalPath = Path.Join(_testdataRoot, "test_diff.html");
        //act
        await storage.StoreDiff(data, _testdataRoot!, "test_diff");

        //assert
        Assert.That(File.Exists(totalPath), Is.True);

        //cleanup
        File.Delete(totalPath);
        Assert.That(File.Exists(totalPath), Is.False);
    }

    [Test]
    public async Task StoreDiff_WhenCalledRight_ShouldBeRightContentInCreatedFile()
    {
        // Arrange
        IDiffStore storage = new HtmlDiffStore();
        IDiffCompute computer = new CsvDiffComputer(new CsvHoldingLoader());
        var data = computer.CreateDiff(
            Path.Combine(_testdataRoot!, "testfiles_new", "test.csv"),
            Path.Combine(_testdataRoot!, "testfiles_old", "test.csv"));
        var diffData = data.ToList();
        var totalPath = Path.Join(_testdataRoot, "test_diff.html");

        //act
        await storage.StoreDiff(diffData, _testdataRoot!, "test_diff");
        using var reader = new StreamReader(totalPath);
        
        var newEntries = data.Where(a => a.NewEntry).ToList();
        var oldEntriesPositive = data.Where(
            a => a is { NewEntry: false, SharesChange: >= 0 }).ToList();
        var oldEntriesNegative = data.Where(
            a => a is { NewEntry: false, SharesChange: < 0 }).ToList();
        oldEntriesNegative.ForEach(a => a.SharesChange = double.Abs(a.SharesChange));
        
        //assert
        var line = await reader.ReadLineAsync();
        Assert.That(line, Is.EqualTo("<html>"));
        line = await reader.ReadLineAsync();
        Assert.That(line, Is.EqualTo("<body>"));
        
        await StoreDiff_AssertDiffPositionsExtracted(reader, newEntries, "New positions");
        await StoreDiff_AssertDiffPositionsExtracted(reader, oldEntriesPositive, "Increased positions");
        await StoreDiff_AssertDiffPositionsExtracted(reader, oldEntriesNegative, "Reduced positions");
        
        line = await reader.ReadLineAsync();
        Assert.That(line, Is.EqualTo("</body>"));
        line = await reader.ReadLineAsync();
        Assert.That(line, Is.EqualTo("</html>"));
        
        //cleanup
        oldEntriesNegative.ForEach(a => a.SharesChange = -a.SharesChange);
        reader.Close();
        File.Delete(totalPath);
        Assert.That(File.Exists(totalPath), Is.False);
    }
    
    private async Task StoreDiff_AssertDiffPositionsExtracted(StreamReader reader, List<DiffData> entries, string header)
    {
        var line = await reader.ReadLineAsync();
        Assert.That(line, Is.EqualTo($"<h2>{header}</h2>"));

        if (entries.Count == 0)
        {
            return;
        }

        line = await reader.ReadLineAsync();
        Assert.That(line, Is.EqualTo("<table border=1 frame=void rules=rows,columns>"));
        line = await reader.ReadLineAsync();
        Assert.That(line, Is.EqualTo("<tr><th>Company name</th><th>ticker</th><th>#shares</th><th>weight(%)</th></tr>"));
        foreach (var e in entries)
        {
            line = await reader.ReadLineAsync();
            Assert.That(line, Is.EqualTo($"<tr><td>{e.Company}</td><td>{e.Ticker}</td><td>{e.SharesChange}</td><td>{e.Weight}</td></tr>"));
        }
        line = await reader.ReadLineAsync();
        Assert.That(line, Is.EqualTo("</table>"));
    }
}