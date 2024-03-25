namespace StockAnalysis.Diff.Compute;

public class DiffComputeException : Exception
{
    public DiffComputeException()
    {
    }

    public DiffComputeException(string message) : base(message)
    {
    }

    public DiffComputeException(string message, Exception? inner) : base(message, inner)
    {
    }
    
}