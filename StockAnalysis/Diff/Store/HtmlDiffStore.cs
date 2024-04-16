using StockAnalysis.Diff.Data;
using Const = StockAnalysis.Constants.Constants;

namespace StockAnalysis.Diff.Store;

public class HtmlDiffStore : IDiffStore
{
    public async Task StoreDiff(IEnumerable<DiffData> data, string path, string name)
    {
        //divide data to new, oldPositive, oldNegative entries
        var newEntries = data.Where(a => a.NewEntry).ToList();
        var oldEntriesPositive = data.Where(
            a => a is { NewEntry: false, SharesChange: >= 0 }).ToList();
        var oldEntriesNegative = data.Where(
            a => a is { NewEntry: false, SharesChange: < 0 }).ToList();

        //change shares to absolute number - would be negative - comment if not wanted
        oldEntriesNegative.ForEach(a => a.SharesChange = double.Abs(a.SharesChange));

        var finalPath = Path.Combine(path, name + Const.HtmlExtension);
        try
        {
            await using var fileWriter = new StreamWriter(finalPath);
            
            await fileWriter.WriteAsync("<html>\n<body>\n");

            await WriteDiffPositions(fileWriter, newEntries, "New positions");
            await WriteDiffPositions(fileWriter, oldEntriesPositive, "Increased positions");
            await WriteDiffPositions(fileWriter, oldEntriesNegative, "Reduced positions");
            
            await fileWriter.WriteAsync("</body>\n</html>");
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

    private static async Task WriteDiffPositions(TextWriter fileWriter, List<DiffData> entries,
                                                 string header)
    {
        await fileWriter.WriteAsync($"<h2>{header}</h2>\n");
        
        if (entries.Count == 0)
        {
            return;
        }

        await fileWriter.WriteAsync("<table border=1 frame=void rules=rows,columns>\n");
        await fileWriter.WriteAsync(
            "<tr><th>Company name</th><th>ticker</th><th>#shares</th><th>weight(%)</th></tr>\n");
        foreach (var e in entries)
        {
            await fileWriter.WriteAsync($"<tr><td>{e.Company}</td><td>{e.Ticker}</td><td>{e.SharesChange}</td><td>{e.Weight}</td></tr>\n");
        }
        await fileWriter.WriteAsync("</table>\n");
    }
}