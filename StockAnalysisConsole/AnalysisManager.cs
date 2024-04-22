using StockAnalysis.Diff.Compute;
using StockAnalysis.Diff.Store;
using StockAnalysis.Download.Manager;
using StockAnalysis.Download.PeriodicalDownload;
using StockAnalysis.HoldingsConfig;
using StockAnalysis.Utilities;
using StockAnalysisConsole.Utils.Paths;

namespace StockAnalysisConsole;

public class AnalysisManager
{
    private readonly IConfiguration _config;
    private readonly DownloadManager _manager;
    private readonly IDiffCompute _diffComputer;
    private readonly IDiffStore _diffStore;

    public AnalysisManager(DownloadManager manager,
                           IDiffCompute diffComputer,
                           IDiffStore diffStore,
                           string configPath)
    {
        _config = new JsonConfiguration(configPath);
        _manager = manager;
        _diffComputer = diffComputer;
        _diffStore = diffStore;

    }

    /// <summary>
    /// Gets the path of a holding file.
    /// </summary>
    private string GetPath(HoldingInformation holding, string dir, string extension)
    {
        return Path.Combine(_manager.StoragePath, dir, holding.Name + extension);
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
    private async Task PerformDiff(HoldingInformation holding, string holdingPathNew, string? holdingPathOld)
    {
        try
        {
            var data = _diffComputer.CreateDiff(holdingPathNew, holdingPathOld);
            await _diffStore.StoreDiff(data, Paths.GetDiffFolderPath(), holding.Name);
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
    private async Task<List<string>> PerformDiffs(IEnumerable<HoldingInformation> holdings,
        string storageDir, string inExtension, string outExtension, Period? period)
    {
        var diffPaths = new List<string>();

        var diffFolder = EnsureDiffFolder();

        // Just quick and dirty way to to solve it
        var oldStorageDir = period == null
            ? "."
            : DateManipulator.GetFolderName(DateOnly.FromDateTime(period.Start), -1, period);

        foreach (var holding in holdings)
        {
            var holdingPath = GetPath(holding, storageDir, inExtension);
            var holdingPathOld = period != null && Directory.Exists(oldStorageDir)
                ? null
                : GetPath(holding, oldStorageDir, inExtension);

            await PerformDiff(holding, holdingPath, holdingPathOld);
            diffPaths.Add(Path.Combine(diffFolder, holding.Name + outExtension));
        }

        return diffPaths;
    }

    /// <summary>
    /// Handles the whole analysis process.
    /// It is virtual because it is meant to be overridden in tests.
    /// The client here is received only because of PeriodicalDownloader.
    /// TODO: Discuss removing client here, this is redundant.
    /// </summary>
    /// <returns>Paths of diffs.</returns>
    public virtual async Task<List<string>> PerformAnalysis(HttpClient client, string outExtension, string inExtension, Period? period)
    {
        var holdings = (await _config.LoadConfiguration()).ToList();
        // When Period is set, "now" can be obtained from it. But not every time.
        var storageDir = DateManipulator.GetFolderName(DateOnly.FromDateTime(DateTime.UtcNow));
        if (!await _manager.GetHoldings(holdings, storageDir))
        {
            return new List<string>();
        }

        return await PerformDiffs(holdings, storageDir, inExtension, outExtension, period);
    }

    /// <summary>
    /// Handles the whole analysis process periodically.
    /// </summary>
    public void PerformAnalysisPeriodically(HttpClient client, string outExtension, string inExtension, Period period)
    {
        var downloader = new PeriodicalDownloader(period, new LocalDateTime(), client, outExtension, inExtension, PerformAnalysis);
        downloader.SchedulePeriodicDownload();
    }
}