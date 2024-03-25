using System.Text.Json;
using StockAnalysis.Download;

namespace StockAnalysis.HoldingsConfig;

public class Configuration
{
    private string ConfigurationPath { get; set; }

    public Configuration(string configurationPath)
    {
        ConfigurationPath = configurationPath;
    }

    /// <summary>
    /// Loads configuration from preset file path. The configuration must be in a JSON format
    /// that corresponds with the 
    /// </summary>
    /// <returns>Information about desired ETF holdings in an array. Can be empty if an issue arose during loading.</returns>
    /// <exception cref="DirectoryNotFoundException">The directory (and therefore the file) of the configuration file does not exist.</exception>
    /// <exception cref="PathTooLongException">The path to the configuration file is too long.</exception>
    /// <exception cref="UnauthorizedAccessException">The program can't access the configuration file.</exception>
    /// <exception cref="NotSupportedException">The configuration could not be serialized.</exception>
    /// <exception cref="IOException">An I/O error occured while opening the configuration file.</exception>
    /// <exception cref="FileNotFoundException">The supplied configuration file does not exist.</exception>
    public async Task<HoldingInformation[]> LoadConfiguration()
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
        catch (Exception)
        {
            throw;
        }
    }
}