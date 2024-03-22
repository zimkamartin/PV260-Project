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
        return date.ToString(Constants.Constants.DateFolderNameFormat);
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
        var resultDate = periodType switch
        {
            PeriodType.Daily => date.AddDays(step),
            PeriodType.Monthly => date.AddMonths(step),
            PeriodType.Quarterly => date.AddMonths(step * 3),
            PeriodType.Weekly => date.AddDays(7 * step),
            _ => date,
        };
        
        return resultDate.ToString(Constants.Constants.DateFolderNameFormat);
    }
}