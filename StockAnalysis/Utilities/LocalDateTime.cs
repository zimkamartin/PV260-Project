namespace StockAnalysis.Utilities;

/// <summary>
/// Provides the current date and time.
/// </summary>
public class SystemDateTime : IDateTimeProvider
{
    public DateTime UtcNow() => DateTime.UtcNow;
}