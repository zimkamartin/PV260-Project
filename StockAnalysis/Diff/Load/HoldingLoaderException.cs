namespace StockAnalysis.Diff.Load;

public class HoldingLoaderException : Exception
{
    public HoldingLoaderException()
    {
    }

    public HoldingLoaderException(string message) : base(message)
    {
    }

    public HoldingLoaderException(string message, Exception? inner) : base(message, inner)
    {
    }
}