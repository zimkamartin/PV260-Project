using System.Text.Json;
using StockAnalysis.Download;

namespace StockAnalysis.HoldingsConfig;

public class Configuration
{
    public string ConfigurationPath { get; }

    public Configuration(string configurationPath)
    {
        ConfigurationPath = configurationPath;
    }

    /// <summary>
    /// Loads configuration from preset file path.
    /// </summary>
    /// <returns>Information about desired ETF holdings in an array. Can be empty if an issue arose during loading.</returns>
    public async Task<HoldingInformation[]> LoadConfiguration()
    {
        try
        {
            await using var reader = File.OpenRead(ConfigurationPath);
            var holding = await JsonSerializer.DeserializeAsync<Holdings>(reader);
            return holding.HoldingInfo.ToArray();
        }
        //ToDo: Gotta catch 'em all!
        catch (Exception e) when (e is ArgumentNullException || e is FileNotFoundException )
        {
            return Array.Empty<HoldingInformation>();
        }
    }
}