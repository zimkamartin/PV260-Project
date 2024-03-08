namespace StockAnalysisTests;

public class Tests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void Test1()
    {
        Assert.IsTrue(DummyClass.Not(false));
        Assert.IsFalse(DummyClass.Not(true));
    }
}