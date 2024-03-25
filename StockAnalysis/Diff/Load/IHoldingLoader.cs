using StockAnalysis.Diff.Data;

namespace StockAnalysis.Diff.Load;

public interface IHoldingLoader
{
    /// <summary>
    /// Loads holding (fund) data from a resource.
    /// </summary>
    /// <param name="path">The path to the given resource.</param>
    /// <returns>A collection of holding (fund) data.</returns>
    /// <exception cref="HoldingLoaderException">Thrown if the loading process fails.</exception>
    IEnumerable<FundData> LoadData(string path);
}