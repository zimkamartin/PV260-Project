using StockAnalysis.HoldingsConfig;
using StockAnalysis.Utilities;

namespace StockAnalysis.Download.PeriodicalDownload;

/// <summary>
/// Calls the Downloader to get the data periodically,
/// which is defined by the Period.
/// </summary>
public class PeriodicalDownloader
{
    private readonly DownloadManager _downloadManager;
    private Period _period;
    private readonly IDateTimeProvider _dateTimeProvider;
    private IEnumerable<HoldingInformation> _holdings;
    private HttpClient _client;

    /// <summary>
    /// Holding information needed for the download.
    /// </summary>
    public IEnumerable<HoldingInformation> Holdings
    {
        get => _holdings;
        set
        {
            _holdings = value;
            SchedulePeriodicDownload();
        }
    }

    /// <summary>
    /// Client used for the download.
    /// </summary>
    public HttpClient Client
    {
        get => _client;
        set
        {
            _client = value;
            SchedulePeriodicDownload();
        }
    }

    /// <summary>
    /// Period of the download action.
    /// </summary>
    public Period Period
    {
        get => _period;
        set
        {
            _period = value;
            SchedulePeriodicDownload();
        }
    }

    /// <summary>
    /// TODO: remove? It should be only virtual for tests.
    /// Downloader manager used for the download.
    /// </summary>
    public virtual DownloadManager DownloaderManager
    {
        get => _downloadManager;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PeriodicalDownloader"/> class.
    /// </summary>
    /// <param name="downloadManager">Used for the download action.</param>
    /// <param name="period">Specifies frequency of the download action.</param>
    /// <param name="dateTimeProvider">Used for determining current time.</param>
    /// <param name="holdings">Holding information required for the download action.</param>
    /// <param name="client">HTTP client required for the download.</param>
    public PeriodicalDownloader(DownloadManager downloadManager, Period period, IDateTimeProvider dateTimeProvider,
        IEnumerable<HoldingInformation> holdings, HttpClient client)
    {
        _downloadManager = downloadManager;
        _period = period;
        _dateTimeProvider = dateTimeProvider;
        _holdings = holdings;
        _client = client;
    }

    /// <summary>
    /// Sets the timer for the periodic download.
    /// </summary>
    /// <returns>Timer that calling the download periodically according to desired interval.</returns>
    public Timer SchedulePeriodicDownload()
    {
        var current = _dateTimeProvider.UtcNow();
        // TODO: Is async callback OK?
        // TODO: Configure storage directory based on download period. Use the new DateManipulator class.
        var timer = new Timer(x => _downloadManager.GetHoldings(_holdings, _client, "."), null,
            _period.TimeToGo(current), _period.Interval);
        // TODO: Is there need to dispose the timer later?
        return timer;
    }
}