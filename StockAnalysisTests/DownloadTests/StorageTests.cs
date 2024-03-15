using System.Text;
using StockAnalysis.Download;

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
        var totalPath = Path.Join(_projectRoot, "test.txt");
        UnicodeEncoding encoding = new();
        const string text = "This is a sample text.";
        var bytes = encoding.GetBytes(text);
        using var memoryStream = new MemoryStream(bytes);
        
        // Act
        try
        {
            await Storage.WriteToFileSystem(memoryStream, _projectRoot!, "test.txt");
        }
        catch (Exception e)
        {
            // Assert
            Assert.Fail("Method threw an exception when it was not supposed to: " + e.Message);
        }
        var actualBytes = await File.ReadAllBytesAsync(totalPath);

        // Assert
        Assert.That(File.Exists(totalPath), Is.True);
        Assert.That(actualBytes, Is.EqualTo(bytes));
        
        // Cleanup.
        File.Delete(totalPath);
        Assert.That(File.Exists(totalPath), Is.False);
    }
}