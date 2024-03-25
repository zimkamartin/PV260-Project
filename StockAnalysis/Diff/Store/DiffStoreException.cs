namespace StockAnalysis.Diff.Store;

public class DiffStoreException : Exception
{
    public DiffStoreException()
    {
    }

    public DiffStoreException(string message) : base(message)
    {
    }

    public DiffStoreException(string message, Exception? inner) : base(message, inner)
    {
    }

}