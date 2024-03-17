using System.Text;

namespace StockAnalysis.Diff;

public static class DiffStore
{
    private const string Separator = ";";
    public static async Task<bool> StoreDiff(List<DiffData> data, String path, String name)
    {
        //divide data to new, old, new entries
        List<DiffData> newEntries = data.Where(a => a.NewEntry).ToList();
        List<DiffData> oldEntriesPositive = data.Where(a => !a.NewEntry && a.SharesChange >= 0).ToList();
        List<DiffData> oldEntriesNegative = data.Where(a => !a.NewEntry && a.SharesChange < 0).ToList();
        
        //change shares to absolute number - would be negative
        oldEntriesNegative.ForEach(a => a.SharesChange = double.Abs(a.SharesChange));
        
        var finalPath = Path.Combine(path, name + ".csv");
        try
        {
            await using var fileWriter = new StreamWriter(finalPath);
            {
                await fileWriter.WriteAsync("sep=" + Separator + "\n");
                WriteDiffPositions(fileWriter, newEntries, "New", "");
                WriteDiffPositions(fileWriter, oldEntriesPositive, "Increased", " up%");
                WriteDiffPositions(fileWriter, oldEntriesNegative, "Reduced", " down%");
            }
            return true;
        }
        catch (Exception e) when (e is DirectoryNotFoundException
                                      or NotSupportedException
                                      or IOException)
        {
            return false;
        }
    }

    private static async void WriteDiffPositions(StreamWriter fileWriter, List<DiffData> entries, string which, string sharesFormat)
    {
        await fileWriter.WriteAsync(which + " positions:" + Separator + Separator + Separator + "\nCompany name" + 
                         Separator + "ticker" + Separator + "#shares" + sharesFormat + Separator + "weight(%)\n");
        foreach (var entry in entries)
        {
            await fileWriter.WriteAsync(entry.Company + Separator + entry.Ticker + Separator + entry.SharesChange + Separator + entry.Weight + "\n");
        }
    }
}