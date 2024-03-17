namespace StockAnalysis.Download.PeriodicalDownload;

/// <summary>
/// It encapsulates the logic of periodicity, which some action may require,
/// by managing the interval and determining the time of the next start of the action.
/// </summary>
public class Period
{
    private readonly PeriodType _type;
    private readonly DateTime _start;
    public TimeSpan Interval { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Period"/> class.
    /// </summary>
    /// <param name="type">Specifies the duration of the interval.</param>
    /// <param name="start">Specifies start of the repetitive action.</param>
    public Period(PeriodType type, DateTime start)
    {
        _type = type;
        _start = start;
        Interval = SetIntervalFromType();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Period"/> class.
    /// This constructor is used for custom period setting, where the interval is
    /// not predefined.
    /// </summary>
    /// <param name="start">Specifies start of the repetitive action.</param>
    /// <param name="interval">Specifies the duration between repetitive action.</param>
    public Period(DateTime start, TimeSpan interval)
    {
        _type = PeriodType.Custom;
        _start = start;
        Interval = interval;
    }

    /// <summary>
    /// Sets the interval from the type of the period.
    /// </summary>
    /// <returns>Time interval.</returns>
    /// <exception cref="ArgumentOutOfRangeException">When type of period is custom,
    /// it is expected that this method is useless to call,
    /// therefore it throws an exception for undefined behavior. </exception>
    public TimeSpan SetIntervalFromType()
    {
        return TimeSpan.FromDays(
            _type switch
            {
                PeriodType.Daily => 1,
                PeriodType.Weekly => 7,
                PeriodType.Monthly => 30,
                PeriodType.Quarterly => 90,
                _ => throw new ArgumentOutOfRangeException("Invalid period type")
            });
    }

    /// <summary>
    /// According to the current time, it calculates the time to the next start of the action.
    /// </summary>
    /// <param name="current">Current time.</param>
    /// <returns>Time interval, which specifies when to start periodic action.</returns>
    public TimeSpan TimeToGo(DateTime current)
    {
        // Normalizing for the cases where the "Start" has already passed
        var futureStart = _start + Math.Ceiling((current - _start).Divide(Interval)) * Interval;
        return futureStart - current;
    }
}