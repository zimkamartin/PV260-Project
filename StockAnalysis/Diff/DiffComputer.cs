using CsvHelper;
using System.Globalization;
using System.Text.RegularExpressions;

namespace StockAnalysis.Diff;

public static class DiffComputer
{
    public static List<DiffData> CreateDiff(string newFile, string? oldFile)
    {
        string filename = Path.GetFileName(newFile);
        
        //if path to old diff is null or file with it does not exists, diff is computed against default one
        if (oldFile == null || !(Path.Exists(oldFile)))
        {
            oldFile = Path.GetDirectoryName(Path.GetDirectoryName(newFile)) + Path.DirectorySeparatorChar +
                                            "Default" + Path.DirectorySeparatorChar + filename;
        }
        
        if (filename != Path.GetFileName(oldFile))
        {
            throw new ArgumentException("different files cannot be compared");
        }

        //try to load old/default csv, data empty if neither exists
        var oldData = new List<FundData>();
        try
        {
            oldData = LoadData(oldFile);
        }
        catch (FileNotFoundException)
        { //do nothing - empty list already initialized
        }

        try
        {
            // Load new.csv
            var newData = LoadData(newFile);
            // Compute changes
            var changes = ComputeChanges(oldData, newData);
            return changes;
        }
        catch (FileNotFoundException e)
        {
            throw new ArgumentException("file with new data does not exist");
        }
    }

    private static List<FundData> LoadData(string filename)
    {
        using var reader = new StreamReader(filename);
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
        return csv.GetRecords<FundData>().ToList();
    }

    public static List<DiffData> ComputeChanges(List<FundData> oldData, List<FundData> newData)
    {
        var changes = new List<DiffData>();

        foreach (var newDataEntry in newData)
        {
            var oldDataEntry = oldData.FirstOrDefault(x => x.Ticker == newDataEntry.Ticker);

            if (oldDataEntry != null)
            {
                var sharesChange = StringToNumber(newDataEntry.Shares) - StringToNumber(oldDataEntry.Shares);
                var marketValueChange =
                    StringToNumber(newDataEntry.MarketValue) - StringToNumber(oldDataEntry.MarketValue);

                changes.Add(new DiffData
                {
                    Company = newDataEntry.Company,
                    Ticker = newDataEntry.Ticker,
                    SharesChange = sharesChange,
                    MarketValueChange = marketValueChange,
                    Weight = StringToNumber(newDataEntry.Weight),
                    NewEntry = false
                });
            }
            else
            {
                changes.Add(new DiffData
                {
                    Company = newDataEntry.Company,
                    Ticker = newDataEntry.Ticker,
                    SharesChange = StringToNumber(newDataEntry.Shares),
                    MarketValueChange = StringToNumber(newDataEntry.MarketValue),
                    Weight = StringToNumber(newDataEntry.Weight),
                    NewEntry = true
                });
            }
        }

        return changes;
    }

    private static double StringToNumber(string data)
    {
        data = Regex.Replace(data, @"[,$%]", "");
        return double.Parse(data, CultureInfo.InvariantCulture);
    }
}

