namespace StockAnalysis.HoldingsConfig;

public class ConfigurationException : Exception
{
    public ConfigurationException()
    {
    }

    public ConfigurationException(string message) : base(message)
    {
    }

    public ConfigurationException(string message, Exception other) : base(message, other)
    {
    }
}