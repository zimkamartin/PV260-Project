using System.Globalization;
using CsvHelper;

namespace StockAnalysis.Diff;

public class HoldingLoader : IHoldingLoader
{
    /// <summary>
    /// Loads data from (now) a csv file.
    /// In the future, it could be extended to load data from other sources.
    /// </summary>
    private static List<FundData> LoadData(string filename)
    {
        using var reader = new StreamReader(filename);
        // TODO: We are passing .csv extension to PerformAnalysis, but here we are not checking it
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
        return csv.GetRecords<FundData>().ToList();
    }
}