﻿using System.Globalization;
using System.Text.RegularExpressions;
using StockAnalysis.Diff.Data;
using StockAnalysis.Diff.Load;

namespace StockAnalysis.Diff.Compute;

public class CsvDiffComputer : IDiffCompute
{
    private readonly IHoldingLoader _loader;

    public CsvDiffComputer(IHoldingLoader loader)
    {
        _loader = loader;
    }

    /// <summary>
    /// Creates a diff between two csv files and returns the changes.
    /// </summary>
    public IEnumerable<DiffData> CreateDiff(string newFile, string? oldFile)
    {
        var filename = Path.GetFileName(newFile);

        // If an older version does not exist, fall back to default.
        if (oldFile == null || !(Path.Exists(oldFile)))
        {
            oldFile = Path.GetDirectoryName(Path.GetDirectoryName(newFile)) + Path.DirectorySeparatorChar +
                      Constants.Constants.DefaultFolderName + Path.DirectorySeparatorChar + filename;
        }

        List<FundData> oldData = new();
        try
        {
            oldData = _loader.LoadData(oldFile).ToList();
        }
        catch (Exception)
        {
            // ignored
        }

        try
        {
            var newData = _loader.LoadData(newFile).ToList();
            return ComputeChanges(oldData, newData);
        }
        catch (Exception e)
        {
            throw new DiffComputeException(e.Message);
        }
    }

    /// <summary>
    /// Computes the changes between two sets of data.
    /// </summary>
    public static IEnumerable<DiffData> ComputeChanges(List<FundData> oldData,
                                                       List<FundData> newData)
    {
        return (from newDataEntry in newData
                let oldDataEntry = oldData.FirstOrDefault(x => x.Ticker == newDataEntry.Ticker)
                select oldDataEntry != null ? GetNewDiffData(newDataEntry, oldDataEntry) : GetNewDiffData(newDataEntry))
            .ToList();
    }

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

    private static DiffData GetNewDiffData(FundData newDataEntry, FundData oldDataEntry)
    {
        var sharesChange = ComputeChange(newDataEntry.Shares, oldDataEntry.Shares);
        var marketValueChange = ComputeChange(newDataEntry.MarketValue, oldDataEntry.MarketValue);
        var weightValueChange = ComputeChange(newDataEntry.Weight, oldDataEntry.Weight);

        return new DiffData
        {
            Company = newDataEntry.Company,
            Ticker = newDataEntry.Ticker,
            SharesChange = sharesChange,
            MarketValueChange = marketValueChange,
            Weight = weightValueChange,
            NewEntry = false
        };
    }

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