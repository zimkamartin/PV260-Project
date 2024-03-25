namespace StockAnalysis.HoldingsConfig;

public interface IConfiguration
{
    /// <summary>
    /// Loads a configuration of the different holdings that must be retrieved and analyzed.
    /// </summary>
    /// <returns>Information about the required holdings.</returns>
    /// <exception cref="ConfigurationException">Configuration loading failed.</exception>
    Task<IEnumerable<HoldingInformation>> LoadConfiguration();
}