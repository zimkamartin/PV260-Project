using StockAnalysis.Download.PeriodicalDownload;

namespace StockAnalysis.Utilities;

public static class DateManipulator
{
    /// <summary>
    /// Creates a folder name from a given date.
    /// </summary>
    /// <param name="date">The date to be transformed into a folder name.</param>
    /// <returns>A string representing the folder's name.</returns>
    public static string GetFolderName(DateOnly date)
    {
        return date.ToString();
    }

    /// <summary>
    /// Creates a folder name from the given data.
    /// </summary>
    /// <param name="date">The relevant date.</param>
    /// <param name="step">A step in a given period. -1 means one period back, 1 means one period forward. </param>
    /// <param name="periodType">The type of period used for the service.</param>
    /// <returns>A folder name corresponding to the date shifted by step * period.</returns>
    public static string GetFolderName(DateOnly date, int step, PeriodType periodType)
    {
        return periodType switch
        {
            PeriodType.Daily => date.AddDays(step).ToString(),
            PeriodType.Monthly => date.AddMonths(step).ToString(),
            PeriodType.Quarterly => date.AddMonths(step * 4).ToString(),
            PeriodType.Weekly => date.AddDays(7 * step).ToString(),
            _ => date.ToString()
        };
    }
}