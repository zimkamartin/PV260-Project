using StockAnalysis.HoldingsConfig;
using StockAnalysis.Utilities;

namespace StockAnalysis.Download.PeriodicalDownload;

/// <summary>
/// TODO: Periodical Download will be deprecated after it will be uploaded on Azure
/// and exchanged with Azure Functions.
/// Calls the Downloader to get the data periodically,
/// which is defined by the Period.
/// </summary>
public class PeriodicalDownloader
{
    private Period _period;
    private readonly IDateTimeProvider _dateTimeProvider;
    private IEnumerable<HoldingInformation> _holdings;
    private HttpClient _client;
    private readonly Func<HttpClient, string, string, Period, Task> _periodicEvent;
    private string _outExtension;
    private string _inExtension;

    /// <summary>
    /// Initializes a new instance of the <see cref="PeriodicalDownloader"/> class.
    /// </summary>
    /// <param name="period">Specifies frequency of the download action.</param>
    /// <param name="dateTimeProvider">Used for determining current time.</param>
    /// <param name="holdings">Holding information required for the download action.</param>
    /// <param name="client">HTTP client required for the download.</param>
    /// <param name="outExtension"></param>
    /// <param name="inExtension"></param>
    /// <param name="periodicEvent"></param>
    public PeriodicalDownloader(Period period, IDateTimeProvider dateTimeProvider,
        IEnumerable<HoldingInformation> holdings, HttpClient client, 
        string outExtension, string inExtension,
        Func<HttpClient, string, string, Period, Task> periodicEvent)
    {
        _period = period;
        _dateTimeProvider = dateTimeProvider;
        _holdings = holdings;
        _client = client;
        _periodicEvent = periodicEvent;
        _outExtension = outExtension;
        _inExtension = inExtension;
    }

    /// <summary>
    /// Sets the timer for the periodic download.
    /// </summary>
    /// <returns>Timer that calling the download periodically according to desired interval.</returns>
    public Timer SchedulePeriodicDownload()
    {
        var current = _dateTimeProvider.UtcNow();
        // TODO: Is async callback OK?
        // TODO: Configure storage directory based on download period. Use the new DateManipulator class
        var timer = new Timer(x =>
                _periodicEvent(_client, _outExtension, _inExtension, _period)
            , null, _period.TimeToGo(current), _period.Interval);

        // TODO: Is there need to dispose the timer later?
        return timer;
    }
}