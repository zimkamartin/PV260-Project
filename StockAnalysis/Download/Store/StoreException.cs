namespace StockAnalysis.Download.Store;

/// <summary>
/// An exception thrown by methods for the IStore interface.
/// </summary>
public class StoreException : Exception
{
    public StoreException() {}
    
    public StoreException(string message) : base(message){}
    
    public StoreException(string message, Exception inner) : base(message, inner){}
}