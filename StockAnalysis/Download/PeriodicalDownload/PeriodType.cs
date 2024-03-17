namespace StockAnalysis.Download.PeriodicalDownload;

/// <summary>
/// Period type specifies in what intervals the data should be downloaded.
/// </summary>
public enum PeriodType
{
    Daily,
    Weekly,
    Monthly,
    Quarterly,
    Custom
}