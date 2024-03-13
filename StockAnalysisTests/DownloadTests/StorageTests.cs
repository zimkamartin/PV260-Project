using System.Text;
using StockAnalysis.Download;
using StockAnalysis.HoldingsConfig;

namespace StockAnalysisTests.DownloadTests;

public class StorageTests
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
    public async Task SimpleStorageTest()
    {
        UnicodeEncoding encoding = new();
        const string text = "This is a sample text.";
        var bytes = encoding.GetBytes(text);

        using var memoryStream = new MemoryStream(bytes);
        await Storage.WriteToFileSystem(memoryStream, _projectRoot!, "test.txt");
        var totalPath = Path.Join(_projectRoot, "test.txt");
        Assert.That(File.Exists(totalPath), Is.True);
        var actualBytes = await File.ReadAllBytesAsync(totalPath);
        Assert.That(actualBytes, Is.EqualTo(bytes));
        
        // Cleanup.
        File.Delete(totalPath);
        Assert.That(File.Exists(totalPath), Is.False);
    }
}