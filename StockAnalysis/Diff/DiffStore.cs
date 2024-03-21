using System.Text;

namespace StockAnalysis.Diff;

public class DiffStore : IDiffStore
{
    private const string Separator = ";";

    public static async Task<bool> StoreDiffToCsv(List<DiffData> data, String path, String name)
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
                await fileWriter.WriteAsync("sep=" + Separator + "\n");
                WriteDiffPositions(fileWriter, newEntries, "New", "");
                WriteDiffPositions(fileWriter, oldEntriesPositive, "Increased", " up%");
                WriteDiffPositions(fileWriter, oldEntriesNegative, "Reduced", " down%");
            }
            return true;
        }
        catch (Exception e) when (e is NotSupportedException
                                      or IOException
                                      or InvalidOperationException)
        {
            return false;
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
        return entry.Company + Separator + entry.Ticker + Separator + entry.SharesChange +
               Separator + entry.Weight + "\n";
    }

    private static string CreateCsvHeader(string type, string sharesFormat)
    {
        return type + " positions:" + Separator + Separator + Separator + "\nCompany name" +
               Separator + "ticker" + Separator + "#shares" + sharesFormat + Separator +
               "weight(%)\n";
    }
}