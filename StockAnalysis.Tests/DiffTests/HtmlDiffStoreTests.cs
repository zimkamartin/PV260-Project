using StockAnalysis.Diff.Compute;
using StockAnalysis.Diff.Data;
using StockAnalysis.Diff.Load;
using StockAnalysis.Diff.Store;
using StockAnalysis.Utilities;
using ApprovalTests;
using ApprovalTests.Reporters;

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
        var data = MockDiffData();
        
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
        var data = MockDiffData();
        
        var totalPath = Path.Join(_testdataRoot, "test_diff.html");

        //act
        await storage.StoreDiff(data, _testdataRoot!, "test_diff");
        //using var reader = new StreamReader(totalPath);
        
        //divide data to new, oldPositive, oldNegative entries
        var (newEntries, oldEntriesPositive, oldEntriesNegative) = DataExtractor.ExtractEntries(data);
        //change shares to absolute number - would be negative - comment if not wanted
        oldEntriesNegative.ForEach(a => a.SharesChange = double.Abs(a.SharesChange));

        //assert
        Approvals.VerifyFile(totalPath);
        // var line = await reader.ReadLineAsync();
        // Assert.That(line, Is.EqualTo("<html>"));
        // line = await reader.ReadLineAsync();
        // Assert.That(line, Is.EqualTo("<body>"));
        //
        // await StoreDiff_AssertDiffPositionsExtracted(reader, newEntries, "New positions");
        // await StoreDiff_AssertDiffPositionsExtracted(reader, oldEntriesPositive, "Increased positions");
        // await StoreDiff_AssertDiffPositionsExtracted(reader, oldEntriesNegative, "Reduced positions");
        //
        // line = await reader.ReadLineAsync();
        // Assert.That(line, Is.EqualTo("</body>"));
        // line = await reader.ReadLineAsync();
        // Assert.That(line, Is.EqualTo("</html>"));
        //
        //cleanup
        oldEntriesNegative.ForEach(a => a.SharesChange = -a.SharesChange);
        //reader.Close();
        File.Delete(totalPath);
        Assert.That(File.Exists(totalPath), Is.False);
    }

    private IEnumerable<DiffData> MockDiffData()
    {
        return new List<DiffData>
        {
            new DiffData
            {
                Company = "Skoda",
                Ticker = "SK",
                SharesChange = 1.11,
                MarketValueChange = 11.1,
                Weight = 123,
                NewEntry = true
            },
            new DiffData
            {
                Company = "Volkswagen",
                Ticker = "VW",
                SharesChange = -5,
                MarketValueChange = -3.33,
                Weight = 17,
                NewEntry = true
            }
        };
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