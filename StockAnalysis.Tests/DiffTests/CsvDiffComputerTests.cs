using System.Text;
using StockAnalysis.Diff.Compute;
using StockAnalysis.Diff.Data;

namespace StockAnalysisTests.DiffTests;


public class CsvDiffComputerTests
{
    
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
        Assert.That(DataGenerator.CompareData(diffData, DataGenerator.DiffCorrectCalculation(oldData, newData)));
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
        Assert.That(DataGenerator.CompareData(diffData, DataGenerator.DiffCorrectCalculation(oldData, newData)));
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
        Assert.That(DataGenerator.CompareData(diffData, DataGenerator.DiffCorrectCalculation(data2, data1)));
    }
    
}