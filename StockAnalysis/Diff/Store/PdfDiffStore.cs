using System.Globalization;
using Aspose.Pdf;
using Aspose.Pdf.Text;
using StockAnalysis.Diff.Data;
using StockAnalysis.Utilities;

namespace StockAnalysis.Diff.Store;

public class PdfDiffStore : IDiffStore
{
    private static Table WriteDiffPositions(IEnumerable<DiffData> data)
    {
        var table = new Table
        {
            Border = new BorderInfo(BorderSide.All, .5f, Color.Black)
        };

        MarginInfo margin = new()
        {
            Top = 3f,
            Bottom = 3f,
            Left = 1f,
            Right = 1f
        };

        table.DefaultCellPadding = margin;
        
        var headerRow = table.Rows.Add();
        headerRow.Cells.Add("Company");
        headerRow.Cells.Add("Ticker");
        headerRow.Cells.Add("#Shares");
        headerRow.Cells.Add("Weight (%)");
        foreach (var item in data)
        {
            var row = table.Rows.Add();
            row.Cells.Add(item.Company);
            row.Cells.Add(item.Ticker);
            row.Cells.Add(item.SharesChange.ToString(CultureInfo.CurrentCulture));
            row.Cells.Add(item.Weight.ToString(CultureInfo.CurrentCulture));
        }

        return table;
    }

    private static void AddPositionsPage(IEnumerable<DiffData> positions, string title, Document document)
    {
        var positionsTable = WriteDiffPositions(positions);
        var positionsPage = document.Pages.Add();
        
        positionsPage.Paragraphs.Add(new TextFragment(title));
        positionsPage.Paragraphs.Add(positionsTable);
    }
    public Task StoreDiff(IEnumerable<DiffData> data, string path, string name)
    {
        //divide data to new, oldPositive, oldNegative entries
        var (newEntries, oldEntriesPositive, oldEntriesNegative) = DataExtractor.ExtractEntries(data);
        //change shares to absolute number - would be negative - comment if not wanted
        oldEntriesNegative.ForEach(a => a.SharesChange = double.Abs(a.SharesChange));
        
        Document document = new();
        AddPositionsPage(newEntries, "New Positions", document);
        AddPositionsPage(oldEntriesPositive, "Increased Positions", document);
        AddPositionsPage(oldEntriesNegative, "Reduced Positions", document);

        try
        {
            document.Save(Path.Join(path, name + ".pdf"));
        }
        catch (Exception e)
        {
            throw new DiffStoreException("Failed to save diff as PDF.", e);
        }

        return Task.CompletedTask;
    }
}