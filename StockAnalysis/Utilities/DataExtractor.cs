using StockAnalysis.Diff.Data;

namespace StockAnalysis.Utilities;

public static class DataExtractor
{
    /// <summary>
    /// Extracts the following entries from the data: new, old with positive share change, old with negative share change.
    /// </summary>
    /// <param name="data">The data that will be used for extraction.</param>
    /// <returns>A triple of (newEntries, oldEntriesPositiveChange, oldEntriesNegativeChange).</returns>
    public static (List<DiffData>, List<DiffData>, List<DiffData>) ExtractEntries(IEnumerable<DiffData> data)
    {
        var dataArray = data as DiffData[] ?? data.ToArray();
        var newEntries = dataArray.Where(a => a.NewEntry).ToList();
        var oldEntriesPositive = dataArray.Where(
            a => a is { NewEntry: false, SharesChange: >= 0 }).ToList();
        var oldEntriesNegative = dataArray.Where(
            a => a is { NewEntry: false, SharesChange: < 0 }).ToList();
        return (newEntries, oldEntriesPositive, oldEntriesNegative);
    }
}