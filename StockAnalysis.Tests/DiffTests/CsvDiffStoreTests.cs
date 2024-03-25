using StockAnalysis.Constants;
using StockAnalysis.Diff.Compute;
using StockAnalysis.Diff.Data;
using StockAnalysis.Diff.Load;
using StockAnalysis.Diff.Store;

namespace StockAnalysisTests.DiffTests;

public class CsvDiffStoreTests
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
        IDiffStore storage = new CsvDiffStore();
        IDiffCompute computer = new CsvDiffComputer(new CsvHoldingLoader());
        var data = computer.CreateDiff(
            Path.Combine(_testdataRoot!, "testfiles_new", "test.csv"), 
            null);
        var totalPath = Path.Join(_testdataRoot, "test_diff.csv");
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
        IDiffStore storage = new CsvDiffStore();
        IDiffCompute computer = new CsvDiffComputer(new CsvHoldingLoader());
        var data = computer.CreateDiff(
            Path.Combine(_testdataRoot!, "testfiles_new", "test.csv"),
            Path.Combine(_testdataRoot!, "testfiles_old", "test.csv"));
        var diffData = data.ToList();
        var totalPath = Path.Join(_testdataRoot, "test_diff.csv");

        //act
        await storage.StoreDiff(diffData, _testdataRoot!, "test_diff");
        using var reader = new StreamReader(totalPath);
            var line = await reader.ReadLineAsync();

        //assert
        Assert.That(line,
            Is.EqualTo("sep=" + Constants.CsvSeparator));
        line = await reader.ReadLineAsync();
        Assert.That(line,
            Is.EqualTo("New positions:" + Constants.CsvSeparator + Constants.CsvSeparator + Constants.CsvSeparator));
        line = await reader.ReadLineAsync();
        Assert.That(line,
            Is.EqualTo("Company name" +
                       Constants.CsvSeparator + "ticker" + Constants.CsvSeparator + "#shares" +
                       Constants.CsvSeparator + "weight(%)"));
        var newEntries = diffData.Where(a => a.NewEntry).ToList();
        var oldEntriesNegative = diffData.Where(
            a => a is { NewEntry: false, SharesChange: < 0 })
            .ToList();
        foreach (var entry in newEntries)
        {
            line = await reader.ReadLineAsync();
            Assert.That(line,
                Is.EqualTo(entry.Company + Constants.CsvSeparator + entry.Ticker + Constants.CsvSeparator +
                           entry.SharesChange + Constants.CsvSeparator + entry.Weight));
        }

        line = await reader.ReadLineAsync();
        Assert.That(line,
            Is.EqualTo(
                "Increased positions:" + Constants.CsvSeparator + Constants.CsvSeparator + Constants.CsvSeparator));

        //go to last line
        while (reader.EndOfStream == false)
        {
            line = await reader.ReadLineAsync();
        }

        //check last line
        Console.WriteLine(line);
        Assert.That(line,
            Is.EqualTo(oldEntriesNegative.Last().Company + Constants.CsvSeparator + oldEntriesNegative.Last().Ticker +
                       Constants.CsvSeparator +
                       double.Abs(oldEntriesNegative.Last().SharesChange) + Constants.CsvSeparator +
                       oldEntriesNegative.Last().Weight));
        //cleanup
        reader.Close();
        File.Delete(totalPath);
        Assert.That(File.Exists(totalPath), Is.False);
    }
}