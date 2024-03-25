using System.Globalization;
using NUnit.Framework.Internal;
using StockAnalysis.Constants;
using StockAnalysis.Diff;

namespace StockAnalysisTests.DiffTests;

public class DiffStoreTests
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
    public async Task StoreDiff_WhenCalledRight_ShouldReturnTrue()
    {
        //act
        List<DiffData> data = DiffComputer.CreateDiff(Path.Combine(_testdataRoot, "testfiles_new", "test.csv"), null);
        bool result = await DiffStore.StoreDiff(data, _testdataRoot, "test_diff");

        //assert
        Assert.IsTrue(result);

        //cleanup
        var totalPath = Path.Join(_testdataRoot, "test_diff.csv");
        File.Delete(totalPath);
        Assert.That(File.Exists(totalPath), Is.False);
    }

    [Test]
    public async Task StoreDiff_WhenCalledRight_ShouldCreateFile()
    {
        //act
        List<DiffData> data = DiffComputer.CreateDiff(Path.Combine(_testdataRoot, "testfiles_new", "test.csv"), null);
        bool result = await DiffStore.StoreDiff(data, _testdataRoot, "test_diff");

        //assert
        var totalPath = Path.Join(_testdataRoot, "test_diff.csv");
        Assert.That(File.Exists(totalPath), Is.True);

        //cleanup
        File.Delete(totalPath);
        Assert.That(File.Exists(totalPath), Is.False);
    }

    [Test]
    public async Task StoreDiff_WhenCalledRight_ShouldBeRightContentInCreatedFile()
    {
        //act
        List<DiffData> data = DiffComputer.CreateDiff(Path.Combine(_testdataRoot, "testfiles_new", "test.csv"),
            Path.Combine(_testdataRoot, "testfiles_old", "test.csv"));
        bool result = await DiffStore.StoreDiff(data, _testdataRoot, "test_diff");

        var totalPath = Path.Join(_testdataRoot, "test_diff.csv");
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

        // go to last line
        while (reader.EndOfStream == false)
        {
            line = reader.ReadLine();
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