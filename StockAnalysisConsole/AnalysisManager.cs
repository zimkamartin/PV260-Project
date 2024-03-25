using StockAnalysis.Diff;
using StockAnalysis.Download;
using StockAnalysis.Download.Getter;
using StockAnalysis.Download.PeriodicalDownload;
using StockAnalysis.Download.Store;
using StockAnalysis.HoldingsConfig;
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

    /// <summary>
    /// Gets the path of a holding file.
    /// </summary>
    private static string GetPath(HoldingInformation holding, string dir, string extension)
    {
        return Path.Combine(Paths.GetDownloadFolderPath(), dir, holding.Name + extension);
    }

    /// <summary>
    /// It ensures that the diff folder exists and returns its path.
    /// </summary>
    private static string EnsureDiffFolder()
    {
        var diffFolder = Paths.GetDiffFolderPath();
        if (Directory.Exists(diffFolder)) return diffFolder;
        try
        {
            Directory.CreateDirectory(diffFolder);
        }
        catch (Exception)
        {
            Console.WriteLine($"Failed to create {diffFolder}, exiting.");
        }

        return diffFolder;
    }

    /// <summary>
    /// Performs a diff on a single holding.
    /// </summary>
    private static async Task PerformDiff(HoldingInformation holding, string holdingPathNew, string? holdingPathOld)
    {
        try
        {
            var data = DiffComputer.CreateDiff(holdingPathNew, holdingPathOld);
            await DiffStore.StoreDiff(data, Paths.GetDiffFolderPath(), holding.Name);
        }
        catch (Exception)
        {
            Console.WriteLine($"Failed to perform a diff on {holding.Name}, exiting.");
            throw;
        }
    }

    /// <summary>
    /// Performs diffs for all holdings.
    /// </summary>
    private static async Task<List<string>> PerformDiffs(IEnumerable<HoldingInformation> holdings,
        string storageDir, string extension, Period? period)
    {
        var diffPaths = new List<string>();

        var diffFolder = EnsureDiffFolder();

        // Just quick and dirty way to to solve it
        var oldStorageDir = period == null
            ? "."
            : DateManipulator.GetFolderName(DateOnly.FromDateTime(period.Start), -1, period);

        foreach (var holding in holdings)
        {
            var holdingPath = GetPath(holding, storageDir, extension);
            var holdingPathOld = period != null && Directory.Exists(oldStorageDir)
                ? null
                : GetPath(holding, oldStorageDir, extension);

            await PerformDiff(holding, holdingPath, holdingPathOld);
            diffPaths.Add(Path.Combine(diffFolder, holding.Name + extension));
        }

        return diffPaths;
    }

    /// <summary>
    /// Gets new data for all holdings.
    /// </summary>
    private async Task<bool> GetNewData(IEnumerable<HoldingInformation> holdings, HttpClient client,
        string storageDirectory)
    {
        return await _manager.GetHoldings(holdings, client, storageDirectory);
    }

    /// <summary>
    /// Handles the whole analysis process.
    /// It is virtual because it is meant to be overridden in tests.
    /// </summary>
    /// <returns>Paths of diffs.</returns>
    public virtual async Task<List<string>> PerformAnalysis(HttpClient client, string extension, Period? period)
    {
        var holdings = await _config.LoadConfiguration();
        // When Period is set, "now" can be obtained from it. But not every time.
        var storageDir = DateManipulator.GetFolderName(DateOnly.FromDateTime(DateTime.UtcNow));
        if (!await GetNewData(holdings, client, storageDir))
        {
            return new List<string>();
        }

        return await PerformDiffs(holdings, storageDir, extension, period);
    }
    
    public async Task PerformAnalysisPeriodically(HttpClient client, string extension, Period period)
    {
        var holdings = await _config.LoadConfiguration();
        var downloader = new PeriodicalDownloader(period, new SystemDateTime(), holdings, client, extension, PerformAnalysis);
        downloader.SchedulePeriodicDownload();
    }
}