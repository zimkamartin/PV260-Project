using StockAnalysis.Diff.Data;
using StockAnalysis.Diff.Store;
using StockAnalysis.Utilities;
using ApprovalTests;

namespace StockAnalysisTests.DiffTests;

public class HtmlDiffStoreTests
{
    [Test]
    public async Task StoreDiff_WhenCalledRight_ShouldCreateFile()
    {
        //arrange
        IDiffStore storage = new HtmlDiffStore();
        var data = MockDiffGenerator.MockDiffData();

        var testDataPath = PathResolver.GetTestDataPath();
        var totalPath = Path.Join(testDataPath, "test_diff.html");

        //act
        await storage.StoreDiff(data, testDataPath, "test_diff");

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
        var data = MockDiffGenerator.MockDiffData();
        var testDataPath = PathResolver.GetTestDataPath();
        var totalPath = Path.Join(testDataPath, "test_diff.html");

        //act
        await storage.StoreDiff(data, testDataPath, "test_diff");

        //assert
        Approvals.VerifyFile(totalPath);

        //cleanup
        File.Delete(totalPath);
        Assert.That(File.Exists(totalPath), Is.False);
    }

}