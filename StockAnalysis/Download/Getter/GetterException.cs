namespace StockAnalysis.Download.Getter;

public class GetterException : Exception
{
    public GetterException() {}
    
    public GetterException(string message) : base(message) {}
    
    public GetterException(string message, Exception inner) : base(message, inner ) {}
}