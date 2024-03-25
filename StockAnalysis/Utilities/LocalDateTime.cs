namespace StockAnalysis.Utilities;

/// <summary>
/// Provides the current date and time.
/// </summary>
public class LocalDateTime : IDateTimeProvider
{
    public DateTime UtcNow() => DateTime.UtcNow;
}