using StockAnalysis.Diff.Data;
using StockAnalysis.Utilities;
using Const = StockAnalysis.Constants.Constants;
using System.Text;

namespace StockAnalysis.Diff.Store;

public class HtmlDiffStore : IDiffStore
{
    /// <summary>
    /// Stores the diff data in a html file.
    /// </summary>
    public async Task StoreDiff(IEnumerable<DiffData> data, string path, string name)
    {
        //divide data to new, oldPositive, oldNegative entries
        var (newEntries, oldEntriesPositive, oldEntriesNegative) = DataExtractor.ExtractEntries(data);
        //change shares to absolute number - would be negative - comment if not wanted
        oldEntriesNegative.ForEach(a => a.SharesChange = double.Abs(a.SharesChange));

        var finalPath = Path.Combine(path, name + Const.HtmlExtension);
        try
        {
            await using var fileWriter = new StreamWriter(finalPath);
            
            //firstly build the whole output, write it to the file only at the end
            var toWrite = new StringBuilder("<html>\n<body>\n");

            WriteDiffPositions(toWrite, newEntries, "New positions");
            WriteDiffPositions(toWrite, oldEntriesPositive, "Increased positions");
            WriteDiffPositions(toWrite, oldEntriesNegative, "Reduced positions");

            toWrite.Append("</body>\n</html>\n");
            
            await fileWriter.WriteAsync(toWrite);
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

    private static void WriteDiffPositions(StringBuilder output, List<DiffData> entries, string header)
    {
        output.Append($"<h2>{header}</h2>\n");
        
        if (entries.Count == 0)
        {
            return;
        }

        output.Append("<table border=1 frame=void rules=rows,columns>\n");
        output.Append("<tr><th>Company name</th><th>ticker</th><th>#shares</th><th>weight(%)</th></tr>\n");
        foreach (var e in entries)
        {
            output.Append($"<tr><td>{e.Company}</td><td>{e.Ticker}</td><td>{e.SharesChange}</td><td>{e.Weight}</td></tr>\n");
        }
        output.Append("</table>\n");
    }
}