using StockAnalysis.Diff.Compute;
using StockAnalysis.Diff.Data;

namespace StockAnalysisTests.DiffTests.DiffComputerTests;


public class CsvDiffComputerTests
{
    private static bool CompareData(IReadOnlyList<DiffData> data1, 
                                    IReadOnlyList<DiffData> data2)
    {
        if (data1.Count != data2.Count)
        {
            return false;
        }

        for (var i = 0; i < data1.Count; i++)
        {
            if (data1[i].Ticker != data2[i].Ticker)
            {
                return false;
            }

            const double tolerance = 0.0001;
            if (Math.Abs(data1[i].SharesChange - data2[i].SharesChange) > tolerance)
            {
                return false;
            }

            if (Math.Abs(data1[i].MarketValueChange - data2[i].MarketValueChange) > tolerance)
            {
                return false;
            }

            if (Math.Abs(data1[i].Weight - data2[i].Weight) > tolerance)
            {
                return false;
            }
        }

        return true;
    }
    
    // If changes are computed correctly
    [Test]
    public void ComputeChanges_ShouldCalculateChangesCorrectly()
    {
        // Generate test data
        var oldData = DataGenerator.GenerateData(2);
        var newData = DataGenerator.GenerateData(oldData,1);

        // Act
        var changes = CsvDiffComputer.ComputeChanges(oldData, newData);
        var diffData = changes.ToList();
        
        // Assert
        Assert.That(CompareData(diffData, DataGenerator.GenerateExpectedResult(oldData, newData)));
    }

    [Test]
    public void ComputeChanges_ShouldCalculateChangesCorrectly_WhenOldDataIsEmpty()
    {
        // Generate test data
        var oldData = DataGenerator.GenerateData(0);
        var newData = DataGenerator.GenerateData(5);

        // Act
        var changes = CsvDiffComputer.ComputeChanges(oldData, newData);
        var diffData = changes.ToList();
        
        // Assert
        Assert.That(CompareData(diffData, DataGenerator.GenerateExpectedResult(oldData, newData)));
    }
    
    [Test]
    public void ComputeChanges_ShouldCalculateChangesCorrectly_WhenNewDataIsSmaller()
    {
        // Generate test data
        var data1 = DataGenerator.GenerateData(6);
        var data2 = DataGenerator.GenerateData(data1,4);

        // Act
        var changes = CsvDiffComputer.ComputeChanges(data2, data1);
        var diffData = changes.ToList();
        
        // Assert
        Assert.That(CompareData(diffData, DataGenerator.GenerateExpectedResult(data2, data1)));
    }
    
}