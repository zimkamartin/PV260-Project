using StockAnalysis.Diff.Data;

namespace StockAnalysisTests.DiffTests.DiffStoreTests;

public static class MockDiffGenerator
{
    public static IEnumerable<DiffData> MockDiffData()
    {
        return new List<DiffData>
        {
            new()
            {
                Company = "Skoda",
                Ticker = "SK",
                SharesChange = 1.11,
                MarketValueChange = 11.1,
                Weight = 123,
                NewEntry = true
            },
            new()
            {
                Company = "Volkswagen",
                Ticker = "VW",
                SharesChange = -5,
                MarketValueChange = -3.33,
                Weight = 17,
                NewEntry = true
            }
        };
    }   
}