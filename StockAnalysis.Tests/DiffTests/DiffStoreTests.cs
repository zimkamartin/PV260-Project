using System.Globalization;
using NUnit.Framework.Internal;
using StockAnalysis.Constants;
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

    [Test]
    public async Task StoreDiff_called_rightContentInFile()
    {
        //act
        List<DiffData> data = DiffComputer.CreateDiff(Path.Combine(_projectRoot, "test.csv"));
        bool result = await DiffStore.StoreDiffToCsv(data, _projectRoot, "test_diff");

        var totalPath = Path.Join(_projectRoot, "test_diff.csv");
        using var reader = new StreamReader(totalPath);
        string? line = reader.ReadLine();

        //assert
        Assert.That(line,
            Is.EqualTo("sep=" + Constants.CsvSeparator));
        line = reader.ReadLine();
        Assert.That(line,
            Is.EqualTo("New positions:" + Constants.CsvSeparator + Constants.CsvSeparator + Constants.CsvSeparator));
        line = reader.ReadLine();
        Assert.That(line,
            Is.EqualTo("Company name" +
                       Constants.CsvSeparator + "ticker" + Constants.CsvSeparator + "#shares" +
                       Constants.CsvSeparator + "weight(%)"));
        List<DiffData> newEntries = data.Where(a => a.NewEntry).ToList();
        List<DiffData> oldEntriesNegative = data.Where(a => !a.NewEntry && a.SharesChange < 0).ToList();
        foreach (var datavar in newEntries)
        {
            line = reader.ReadLine();
            Assert.That(line,
                Is.EqualTo(datavar.Company + Constants.CsvSeparator + datavar.Ticker + Constants.CsvSeparator +
                           datavar.SharesChange + Constants.CsvSeparator + datavar.Weight));
        }

        line = reader.ReadLine();
        Assert.That(line,
            Is.EqualTo(
                "Increased positions:" + Constants.CsvSeparator + Constants.CsvSeparator + Constants.CsvSeparator));

        //go to last line
        while (reader.EndOfStream == false)
        {
            line = reader.ReadLine();
        }

        //check last line
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