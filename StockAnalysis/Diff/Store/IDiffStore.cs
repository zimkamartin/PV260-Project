using StockAnalysis.Diff.Data;

namespace StockAnalysis.Diff.Store;

public interface IDiffStore
{
    /// <summary>
    /// Stores computed diff to csv file.
    /// </summary>
    /// <param name="data">The list of data containing difference between 2 files containing stocks.</param>
    /// <param name="path">The path to which file is supposed to be saved.</param>
    /// <param name="name">The name of new file, without path and without the .csv extension.</param>
    /// <returns>Bool - true if data were successfully saved, otherwise false.</returns>
    /// <exception cref="DiffStoreException">The store operation failed.</exception>
    public Task StoreDiff(IEnumerable<DiffData> data, string path, string name);
}