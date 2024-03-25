using System.Globalization;
using CsvHelper;
using StockAnalysis.Diff.Data;

namespace StockAnalysis.Diff.Load;

public class CsvHoldingLoader : IHoldingLoader
{
    /// <summary>
    /// Loads data from a csv file.
    /// </summary>
    public IEnumerable<FundData> LoadData(string path)
    {
        try
        {
            using var reader = new StreamReader(path);
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
            return csv.GetRecords<FundData>().ToList();
        }
        catch (Exception e)
        {
            throw new HoldingLoaderException(e.Message, e.InnerException);
        }

        
    }
}