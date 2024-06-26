namespace StockAnalysis.Utilities;

/// <summary>
/// Interface for providing the current date and time.
/// Mainly for testing purposes, to inject provider if the current time is needed.
/// </summary>
public interface IDateTimeProvider
{
    /// <summary>
    /// Returns the current date in UTC.
    /// </summary>
    public DateTime UtcNow();
}
