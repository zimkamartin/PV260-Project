﻿using System.Text;

namespace StockAnalysis.Diff;

public class DiffStore
{
    public static async Task<bool> StoreDiff(List<DiffData> data, String path, String name)
    {
        List<DiffData> newEntries = data.Where(a => a.NewEntry).ToList();
        List<DiffData> oldEntriesPositive = data.Where(a => !a.NewEntry && a.SharesChange >= 0).ToList();
        List<DiffData> oldEntriesNegative = data.Where(a => !a.NewEntry && a.SharesChange < 0).ToList();
        
        var finalPath = Path.Combine(path, name + ".csv");
        try
        {
            await using var fileStream = File.Create(finalPath);
            {
                WriteDiffPositions(fileStream, newEntries, "New", "");
                WriteDiffPositions(fileStream, oldEntriesPositive, "Increased", "(\u2191x%)");
                WriteDiffPositions(fileStream, oldEntriesNegative, "Reduced", "\u2193x%");
            }
            fileStream.Close();
            return true;
        }
        catch (Exception e) when (e is DirectoryNotFoundException
                                      or NotSupportedException
                                      or IOException)
        {
            return false;
        }
    }

    private static void WriteDiffPositions(FileStream fileStream, List<DiffData> entries, string which, string sharesFormat)
    {
        byte[] info = new UTF8Encoding(true).GetBytes(which + " positions:;;;\nCompany name;ticker;#shares" + sharesFormat + ";weight(%)\n");
        fileStream.Write(info, 0, info.Length);
        foreach (var entry in entries)
        {
            info = new UTF8Encoding(true).GetBytes(entry.Company + ";" + entry.Ticker + ";" + entry.SharesChange + ";" + entry.Weight + "\n");
            fileStream.Write(info, 0, info.Length);
        }
    }
}