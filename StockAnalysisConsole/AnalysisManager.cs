using System.Runtime.Serialization;
using StockAnalysis.Diff;
using StockAnalysis.Download;
using StockAnalysis.Download.Getter;
using StockAnalysis.Download.PeriodicalDownload;
using StockAnalysis.Download.Store;
using StockAnalysis.HoldingsConfig;
using StockAnalysis.Sending.Sender;
using StockAnalysis.Utilities;
using StockAnalysisConsole.Utils.Paths;

namespace StockAnalysisConsole;

public class AnalysisManager
{
    private readonly Configuration _config;
    private readonly DownloadManager _manager;
    private readonly ISender _sender;

    public AnalysisManager(IGetter dataGetter, IStore dataStore, ISender sender)
    {
        _config = new Configuration(Paths.GetConfigFilePath());
        _manager = new DownloadManager(Paths.GetDownloadFolderPath(), dataGetter, dataStore);
        _sender = sender;
    }

    private static async Task<List<string>> PerformDiff(IEnumerable<HoldingInformation> holdings, 
                                            string storageDirectory, string extension, Period? period)
    {
        var diffPaths = new List<string>();
        var diffFolder = Paths.GetDiffFolderPath();
        // Just quick and dirty way to to solve it
        var oldStorageDir =  period == null ? "." : DateManipulator.GetFolderName(DateOnly.FromDateTime(period.Start), -1, period);
        if (!Directory.Exists(diffFolder))
        {
            try
            {
                Directory.CreateDirectory(diffFolder);
            }
            catch (Exception)
            {
                Console.WriteLine($"Failed create {diffFolder}, exiting.");
                return new List<string>();
            }
        }
        foreach (var holding in holdings)
        {
            var holdingPath = Path.Combine(Paths.GetDownloadFolderPath(), storageDirectory,
                holding.Name + extension);
            var holdingPathOld = period != null && Directory.Exists(oldStorageDir)
                ? null
                : Path.Combine(Paths.GetDownloadFolderPath(), oldStorageDir,
                    holding.Name + extension);
            try
            {

                var data = DiffComputer.CreateDiff(holdingPath, holdingPathOld);
                await DiffStore.StoreDiff(data, diffFolder, holding.Name);
            }
            catch (Exception)
            {
                Console.WriteLine($"Failed to perform a diff on {holding.Name}, exiting.");
                return new List<string>();
            }

            diffPaths.Add(Path.Combine(diffFolder, holding.Name + extension));
        }

        return diffPaths;
    }
    private async Task<bool> GetNewData(IEnumerable<HoldingInformation> holdings, HttpClient client, string storageDirectory)
    {
        return await _manager.GetHoldings(holdings, client, storageDirectory);
    }
    
    public async Task PerformAnalysis(HttpClient client, string extension, string[] addresses, Period? period)
    {
        var holdings = await _config.LoadConfiguration();
        // When Period is set, "now" can be obtained from it. But not every time.
        var storageDirectory = DateManipulator.GetFolderName(DateOnly.FromDateTime(DateTime.UtcNow));
        if (!await GetNewData(holdings, client, storageDirectory))
        {
            Console.WriteLine("Failed to download the required holdings.");
            return;
        }
        var diffPaths = await PerformDiff(holdings, storageDirectory, extension, period);
        if (diffPaths.Count == 0)
        {
            Console.WriteLine("Failed to obtain any holding diffs.");
            return;
        }
        try
        {
            await _sender.SendNotification(addresses, diffPaths);
        }
        catch (Exception)
        {
            Console.WriteLine("Email sending failed.");
        }
    }
}