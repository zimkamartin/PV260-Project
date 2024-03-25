using System.Text;
using Const = StockAnalysis.Constants.Constants;

namespace StockAnalysis.Diff;

public class DiffStore : IDiffStore
{
    public static async Task<bool> StoreDiff(List<DiffData> data, String path, String name)
    {
        //divide data to new, old, new entries
        List<DiffData> newEntries = data.Where(a => a.NewEntry).ToList();
        List<DiffData> oldEntriesPositive = data.Where(a => !a.NewEntry && a.SharesChange >= 0).ToList();
        List<DiffData> oldEntriesNegative = data.Where(a => !a.NewEntry && a.SharesChange < 0).ToList();

        //change shares to absolute number - would be negative - comment if not wanted
        oldEntriesNegative.ForEach(a => a.SharesChange = double.Abs(a.SharesChange));

        var finalPath = Path.Combine(path, name + ".csv");
        //TODO check path format?
        try
        {
            await using var fileWriter = new StreamWriter(finalPath);
            {
                //set separator in csv for it to be readable
                await fileWriter.WriteAsync("sep=" + Const.CsvSeparator + "\n");
                WriteDiffPositions(fileWriter, newEntries, "New", "");
                WriteDiffPositions(fileWriter, oldEntriesPositive, "Increased", Const.CsvSharesUpIndicator);
                WriteDiffPositions(fileWriter, oldEntriesNegative, "Reduced", Const.CsvSharesDownIndicator);
            }
            return true;
        }
        catch (Exception e) when (e is NotSupportedException
                                      or IOException
                                      or InvalidOperationException)
        {
            return false;
        }
        finally
        {
            oldEntriesNegative.ForEach(a => a.SharesChange = -a.SharesChange);
        }
    }

    private static async void WriteDiffPositions(StreamWriter fileWriter, List<DiffData> entries, string type,
        string sharesFormat)
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