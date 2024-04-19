using StockAnalysis.Diff.Data;
using StockAnalysis.Diff.Store;
using StockAnalysis.Utilities;
using ApprovalTests;

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

        //assert
        Approvals.VerifyFile(totalPath);
        
        //cleanup
        File.Delete(totalPath);
        Assert.That(File.Exists(totalPath), Is.False);
    }

    private static IEnumerable<DiffData> MockDiffData()
    {
        return new List<DiffData>
        {
            new()
            {
                Company = "Skoda",
                Ticker = "SK",
                SharesChange = 1.11,
                MarketValueChange = 11.1,
                Weight = 123,
                NewEntry = true
            },
            new()
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
}