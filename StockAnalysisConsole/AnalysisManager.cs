using StockAnalysis.Diff;
using StockAnalysis.Download;
using StockAnalysis.Download.Getter;
using StockAnalysis.Download.Store;
using StockAnalysis.HoldingsConfig;
using StockAnalysis.SendEmail;
using StockAnalysis.Utilities;
using StockAnalysisConsole.Utils.Paths;

namespace StockAnalysisConsole;

public class AnalysisManager
{
    private readonly Configuration _config;
    private readonly DownloadManager _manager;

    public AnalysisManager(IGetter dataGetter, IStore dataStore)
    {
        _config = new Configuration(Paths.GetConfigFilePath());
        _manager = new DownloadManager(Paths.GetDownloadFolderPath(), dataGetter, dataStore);
    }

    private static async Task<List<string>> PerformDiff(IEnumerable<HoldingInformation> holdings, 
                                            string storageDirectory, string extension)
    {
        // TODO: Check for exceptions
        var diffPaths = new List<string>();
        var diffFolder = Paths.GetDiffFolderPath();
        if (!Directory.Exists(diffFolder))
        {
            Directory.CreateDirectory(diffFolder);
        }
        foreach (var holding in holdings)
        {
            var holdingPath = Path.Combine(Paths.GetDownloadFolderPath(), storageDirectory,
                holding.Name + extension);
            var data = DiffComputer.CreateDiff(holdingPath);
            await DiffStore.StoreDiff(data, diffFolder, holding.Name);
            diffPaths.Add(Path.Combine(diffFolder, holding.Name + extension));
        }

        return diffPaths;
    }
    private async Task<bool> GetNewData(IEnumerable<HoldingInformation> holdings, HttpClient client, string storageDirectory)
    {
        return await _manager.GetHoldings(holdings, client, storageDirectory);
    }
    
    public async Task PerformAnalysis(HttpClient client, string extension, List<string> addresses)
    {
        var holdings = await _config.LoadConfiguration();
        var storageDirectory = DateManipulator.GetFolderName(DateOnly.FromDateTime(DateTime.UtcNow));
        if (!await GetNewData(holdings, client, storageDirectory))
        {
            Console.WriteLine("Failed to download the required holdings.");
            return;
        }
        var diffPaths = await PerformDiff(holdings, storageDirectory, extension);
        if (diffPaths.Count == 0)
        {
            Console.WriteLine("Failed to obtain any holding diffs.");
            return;
        }
        
        try
        {
            await Sender.SendMail(addresses, diffPaths);
        }
        catch (Exception)
        {
            Console.WriteLine("Email sending failed.");
        }
    }
}