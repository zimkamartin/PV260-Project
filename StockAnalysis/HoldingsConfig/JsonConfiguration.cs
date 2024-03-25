using System.Text.Json;
using StockAnalysis.Download;

namespace StockAnalysis.HoldingsConfig;

public class JsonConfiguration : IConfiguration
{
    private string ConfigurationPath { get; }

    public JsonConfiguration(string configurationPath)
    {
        ConfigurationPath = configurationPath;
    }

    /// <summary>
    /// Loads configuration from preset file path. The configuration must be in a JSON format
    /// that corresponds with the 
    /// </summary>
    /// <returns>Information about desired ETF holdings in an array. Can be empty if an issue arose during loading.</returns>
    /// <exception cref="ConfigurationException">Configuration load failed.</exception>
    public async Task<IEnumerable<HoldingInformation>> LoadConfiguration()
    {
        try
        {
            await using var reader = File.OpenRead(ConfigurationPath);
            var holding = await JsonSerializer.DeserializeAsync<Holdings>(reader);
            return holding.HoldingInfo.ToArray();
        }
        catch (Exception e) when (e is ArgumentNullException
                                      or JsonException)
        {
            return Array.Empty<HoldingInformation>();
        }
        catch (Exception e)
        {
            throw new ConfigurationException(e.Message);
        }
    }
}