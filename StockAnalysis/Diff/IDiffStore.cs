namespace StockAnalysis.Diff;

public interface IDiffStore
{
    /// <summary>
    /// Stores computed diff to csv file.
    /// </summary>
    /// <param name="data">The list of data containing difference between 2 files containing stocks.</param>
    /// <param name="path">The path to which file is supposed to be saved.</param>
    /// <param name="name">The name of new file, without path and without the .csv extension.</param>
    /// <returns>Bool - true if data were successfully saved, otherwise false.</returns>
    /// <exception cref="ObjectDisposedException">The stream was closed prematurely.</exception>
    /// <exception cref="UnauthorizedAccessException">The file could not be created.</exception>
    /// <exception cref="ArgumentException">The path is an empty string (""). -or- path contains the name of a system device.</exception>
    /// <exception cref="PathTooLongException">The path is too long.</exception>
    /// <exception cref="DirectoryNotFoundException">The specified path is invalid</exception>
    public static abstract Task<bool> StoreDiff(List<DiffData> data, String path, String name);
}