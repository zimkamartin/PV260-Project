using StockAnalysis.Diff.Data;
using StockAnalysis.Utilities;
using Const = StockAnalysis.Constants.Constants;

namespace StockAnalysis.Diff.Store;

public class CsvDiffStore : IDiffStore
{
    public async Task StoreDiff(IEnumerable<DiffData> data, string path, string name)
    {
        //divide data to new, oldPositive, oldNegative entries
        var (newEntries, oldEntriesPositive, oldEntriesNegative) = DataExtractor.ExtractEntries(data);
        //change shares to absolute number - would be negative - comment if not wanted
        oldEntriesNegative.ForEach(a => a.SharesChange = double.Abs(a.SharesChange));

        var finalPath = Path.Combine(path, name + Const.CsvExtension);
        try
        {
            await using var fileWriter = new StreamWriter(finalPath);
            {
                await fileWriter.WriteAsync("sep=" + Const.CsvSeparator + "\n");
                await WriteDiffPositions(fileWriter, newEntries, "New", "");
                await WriteDiffPositions(fileWriter, oldEntriesPositive, "Increased", Const.CsvSharesUpIndicator);
                await WriteDiffPositions(fileWriter, oldEntriesNegative, "Reduced", Const.CsvSharesDownIndicator);
            }
        }
        catch (Exception e)
        {
            throw new DiffStoreException(e.Message);
        }
        finally
        {
            oldEntriesNegative.ForEach(a => a.SharesChange = -a.SharesChange);
        }
    }

    private static async Task WriteDiffPositions(TextWriter fileWriter, IEnumerable<DiffData> entries,
                                                 string type, string sharesFormat)
    {
        await fileWriter.WriteAsync(CreateCsvHeader(type, sharesFormat));
        foreach (var entry in entries)
        {
            await fileWriter.WriteAsync(CreateCsvLine(entry));
        }
    }

    private static string CreateCsvLine(DiffData entry)
    {
        return entry.Company + Const.CsvSeparator + entry.Ticker + Const.CsvSeparator + entry.SharesChange +
               Const.CsvSeparator + entry.Weight + "\n";
    }

    private static string CreateCsvHeader(string type, string sharesFormat)
    {
        return type + " positions:" + Const.CsvSeparator + Const.CsvSeparator + Const.CsvSeparator + "\nCompany name" +
               Const.CsvSeparator + "ticker" + Const.CsvSeparator + "#shares" + sharesFormat + Const.CsvSeparator +
               "weight(%)\n";
    }
}