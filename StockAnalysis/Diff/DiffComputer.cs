using CsvHelper;
using System.Globalization;
using System.Text.RegularExpressions;

namespace StockAnalysis.Diff;

public static class DiffComputer
{
    /// <summary>
    /// Creates a diff between two csv files and returns the changes.
    /// </summary>
    public static List<DiffData> CreateDiff(string newFile, string? oldFile)
    {
        string filename = Path.GetFileName(newFile);

        // If path to old diff is null or file with it does not exists, diff is computed against default one
        if (oldFile == null || !(Path.Exists(oldFile)))
        {
            oldFile = Path.GetDirectoryName(Path.GetDirectoryName(newFile)) + Path.DirectorySeparatorChar +
                      "Default" + Path.DirectorySeparatorChar + filename;
        }

        if (filename != Path.GetFileName(oldFile))
        {
            throw new ArgumentException("Different files cannot be compared");
        }

        if (!Path.Exists(newFile))
        {
            throw new ArgumentException("File with new data does not exist");
        }


        // Try to load old/default csv, data empty if neither exists
        var oldData = new List<FundData>();
        try
        {
            oldData = LoadData(oldFile);
        }
        // Not sure about "FileNotFoundException" - something more general?
        // What if "Default" folder doesn't exist? 
        catch (Exception)
        {
            // Do nothing - empty list already initialized
        }

        try
        {
            // Load new data
            var newData = LoadData(newFile);
            // Compute changes
            return ComputeChanges(oldData, newData);
        }
        catch (Exception)
        {
            // TODO: Should it be ArgumentException?
            throw new ArgumentException("Creating diff failed.");
        }
    }

    /// <summary>
    /// Loads data from (now) a csv file.
    /// In the future, it could be extended to load data from other sources.
    /// </summary>
    private static List<FundData> LoadData(string filename)
    {
        using var reader = new StreamReader(filename);
        // TODO: We are passing .csv extension to PerformAnalysis, but here we are not checking it
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
        return csv.GetRecords<FundData>().ToList();
    }

    /// <summary>
    /// Computes the changes between two sets of data.
    /// </summary>
    public static List<DiffData> ComputeChanges(List<FundData> oldData, List<FundData> newData)
    {
        return (from newDataEntry in newData
                let oldDataEntry = oldData.FirstOrDefault(x => x.Ticker == newDataEntry.Ticker)
                select oldDataEntry != null ? GetNewDiffData(newDataEntry, oldDataEntry) : GetNewDiffData(newDataEntry))
            .ToList();
    }

    /// <summary>
    /// Overload for creating new diff data if there is no old data to compare with.
    /// </summary>
    private static DiffData GetNewDiffData(FundData dataEntry)
    {
        return new DiffData
        {
            Company = dataEntry.Company,
            Ticker = dataEntry.Ticker,
            SharesChange = StringToNumber(dataEntry.Shares),
            MarketValueChange = StringToNumber(dataEntry.MarketValue),
            Weight = StringToNumber(dataEntry.Weight),
            NewEntry = true
        };
    }

    /// <summary>
    /// Overload for creating new diff data if there is old data to compare with.
    /// </summary>
    private static DiffData GetNewDiffData(FundData newDataEntry, FundData oldDataEntry)
    {
        var sharesChange = ComputeChange(newDataEntry.Shares, oldDataEntry.Shares);
        var marketValueChange = ComputeChange(newDataEntry.MarketValue, oldDataEntry.MarketValue);

        return new DiffData
        {
            Company = newDataEntry.Company,
            Ticker = newDataEntry.Ticker,
            SharesChange = sharesChange,
            MarketValueChange = marketValueChange,
            Weight = StringToNumber(newDataEntry.Weight),
            NewEntry = false
        };
    }

    /// <summary>
    /// Computes the change between two string values from the csv.
    /// </summary>
    private static double ComputeChange(string newValue, string oldValue)
    {
        return StringToNumber(newValue) - StringToNumber(oldValue);
    }

    private static double StringToNumber(string data)
    {
        data = Regex.Replace(data, @"[,$%]", "");
        return double.Parse(data, CultureInfo.InvariantCulture);
    }
}