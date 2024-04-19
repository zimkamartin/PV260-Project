namespace StockAnalysis.Diff.Store;

public static class DiffStoreCreator
{
    public static IDiffStore CreateStore(string outputExtension)
    {
        switch (outputExtension.Replace(".", "").ToLower())
        {
            case "csv":
                return new CsvDiffStore();
            case "html":
                return new HtmlDiffStore();
            default:
                throw new NotImplementedException("Chosen output extension is currently not supported.");
        }
    }
}