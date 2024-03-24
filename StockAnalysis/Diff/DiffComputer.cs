using CsvHelper;
using System.Globalization;
using System.Text.RegularExpressions;

namespace StockAnalysis.Diff;

public static class DiffComputer
{
    public static List<DiffData> CreateDiff(string path)
    {
        
        // FIXME: constants "-old" and "-new"
        var oldFile = Path.GetDirectoryName(path) + Path.DirectorySeparatorChar +
                      Path.GetFileNameWithoutExtension(path) + "-old" + Path.GetExtension(path);
        var newFile = Path.GetDirectoryName(path) + Path.DirectorySeparatorChar +
                      Path.GetFileNameWithoutExtension(path) + "-new" + Path.GetExtension(path);
        // Load old.csv and new.csv
        // FIXME: Throws exception if file does not exist
        var oldData = LoadData(oldFile);
        var newData = LoadData(newFile);

        // Compute changes
        var changes = ComputeChanges(oldData, newData);
        
        return changes;
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

