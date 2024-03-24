using StockAnalysis.Diff;

namespace StockAnalysisTests.DiffTests;

public class DiffComputerTests
{
    // If changes are computed correctly
    [Test]
    public void ComputeChanges_ShouldCalculateChangesCorrectly()
    {
        // Generate test data
        var oldData = new List<FundData>
        {
            new() { Ticker = "TICK1", Shares = "100", MarketValue = "1000", Weight = "10" },
            new() { Ticker = "TICK2", Shares = "200", MarketValue = "2000", Weight = "20" }
        };

        var newData = new List<FundData>
        {
            new() { Ticker = "TICK1", Shares = "150", MarketValue = "1500", Weight = "15" },
            new() { Ticker = "TICK2", Shares = "250", MarketValue = "2500", Weight = "25" },
            new() { Ticker = "TICK3", Shares = "300", MarketValue = "3000", Weight = "30" }
        };

        // Act
        var changes = DiffComputer.ComputeChanges(oldData, newData);

        // Assert
        Assert.That(changes.Count, Is.EqualTo(3));

        Assert.That(changes[0].Ticker, Is.EqualTo("TICK1"));
        Assert.That(changes[0].SharesChange, Is.EqualTo(50));
        Assert.That(changes[0].MarketValueChange, Is.EqualTo(500));
        Assert.That(changes[0].Weight, Is.EqualTo(15));

        Assert.That(changes[1].Ticker, Is.EqualTo("TICK2"));
        Assert.That(changes[1].SharesChange, Is.EqualTo(50));
        Assert.That(changes[1].MarketValueChange, Is.EqualTo(500));
        Assert.That(changes[1].Weight, Is.EqualTo(25));
    }

    [Test]
    public void ComputeChanges_ShouldCalculateChangesCorrectly_WhenOldDataIsEmpty()
    {
        // Generate test data
        var oldData = new List<FundData>();

        var newData = new List<FundData>
        {
            new() { Ticker = "TICK1", Shares = "150", MarketValue = "1500", Weight = "15" },
            new() { Ticker = "TICK2", Shares = "250", MarketValue = "2500", Weight = "25" },
            new() { Ticker = "TICK3", Shares = "300", MarketValue = "3000", Weight = "30" }
        };

        // Act
        var changes = DiffComputer.ComputeChanges(oldData, newData);

        // Assert
        Assert.That(changes.Count, Is.EqualTo(3));

        Assert.That(changes[0].Ticker, Is.EqualTo("TICK1"));
        Assert.That(changes[0].SharesChange, Is.EqualTo(150));
        Assert.That(changes[0].MarketValueChange, Is.EqualTo(1500));
        Assert.That(changes[0].Weight, Is.EqualTo(15));

        Assert.That(changes[1].Ticker, Is.EqualTo("TICK2"));
        Assert.That(changes[1].SharesChange, Is.EqualTo(250));
        Assert.That(changes[1].MarketValueChange, Is.EqualTo(2500));
        Assert.That(changes[1].Weight, Is.EqualTo(25));
    }
}