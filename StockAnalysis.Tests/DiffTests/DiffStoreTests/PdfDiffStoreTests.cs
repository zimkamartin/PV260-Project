using StockAnalysis.Diff.Store;
using StockAnalysisTests.Utility;

namespace StockAnalysisTests.DiffTests.DiffStoreTests;

public class PdfDiffStoreTests
{

    private static void CheckValidPdfFormat(string path)
    {
        using var document = new Aspose.Pdf.Document(path);
        Assert.That(document.Pages, Has.Count.GreaterThanOrEqualTo(3));
    }

    [Test]
    public async Task StoreDiff_WhenCalledRight_ShouldCreateFile()
    {
        var testDataPath = PathResolver.GetTestDataPath();
        IDiffStore storage = new PdfDiffStore();
        var data = MockDiffGenerator.MockDiffData();
        var totalPath = Path.Join(testDataPath, "test_diff.pdf");

        //act
        await storage.StoreDiff(data, testDataPath, "test_diff");
        
        //assert
        //I was unable to find ways to properly test the PDF file created by the nuget package we used - Aspose.pdf
        //If there's better tests in CheckValidPdfFormat, I succeeded in figuring it out.
        CheckValidPdfFormat(totalPath);
        
        //cleanup
        File.Delete(totalPath);
        Assert.That(File.Exists(totalPath), Is.False);
    }
}