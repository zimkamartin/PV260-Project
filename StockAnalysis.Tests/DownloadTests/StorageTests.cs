using System.Text;
using StockAnalysis.Download.Store;
using StockAnalysisTests.Utility;

namespace StockAnalysisTests.DownloadTests;

public class StorageTests
{
    [Test]
    public async Task CsvStorage_WriteToFileSystem_WritesStreamToTextFile()
    {
        // Arrange
        const string directory = "StorageTest";
        const string fileName = "store";
        var storage = new CsvStorage();
        var dirPath = Path.Join(PathResolver.GetRoot(), directory);
        var totalPath = Path.Join(dirPath, fileName + ".csv");
        UnicodeEncoding encoding = new();
        const string text = "This is a sample text.";
        var bytes = encoding.GetBytes(text);
        using var memoryStream = new MemoryStream(bytes);

        // Act
        // Not checking for exceptions as they are not meant to be thrown here.
        await storage.Store(memoryStream, PathResolver.GetRoot(), directory, fileName);
        var actualBytes = await File.ReadAllBytesAsync(totalPath);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(File.Exists(totalPath), Is.True);
            Assert.That(actualBytes, Is.Not.Empty);
        });

        // Cleanup.
        File.Delete(totalPath);
        Directory.Delete(dirPath);
    }
}