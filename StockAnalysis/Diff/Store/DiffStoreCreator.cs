namespace StockAnalysis.Diff.Store;

public static class DiffStoreCreator
{
    public static IDiffStore CreateStore(string outputExtension)
    {
        return outputExtension.Replace(".", "").ToLower() switch
        {
            "csv" => new CsvDiffStore(),
            "html" => new HtmlDiffStore(),
            "pdf" => new PdfDiffStore(),
            _ => throw new NotImplementedException("Chosen output extension is currently not supported.")
        };
    }
}