using System.Text;
using StockAnalysis.Download.Store;

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
    public async Task Storage_WriteToFileSystem_WritesStreamToTextFile()
    {
        // Arrange
        const string fileName = "store";
        var storage = new CsvStorage();
        var totalPath = Path.Join(_projectRoot, fileName + "-new.csv");
        UnicodeEncoding encoding = new();
        const string text = "This is a sample text.";
        var bytes = encoding.GetBytes(text);
        using var memoryStream = new MemoryStream(bytes);
        
        // Act
        // Not checking for exceptions as they are not meant to be thrown here.
        await storage.Store(memoryStream, _projectRoot!, fileName);
        var actualBytes = await File.ReadAllBytesAsync(totalPath);
        
        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(File.Exists(totalPath), Is.True);
            Assert.That(actualBytes, Is.Not.Empty);
        });

        // Cleanup.
        File.Delete(totalPath);
        Assert.That(File.Exists(totalPath), Is.False);
    }
}