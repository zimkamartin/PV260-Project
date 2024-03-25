namespace StockAnalysis.Sending.Sender;

public class SenderException : Exception
{
    public SenderException()
    {
    }

    public SenderException(string message) : base(message)
    {
    }

    public SenderException(string message, Exception inner) : base(message, inner)
    {
    }
}