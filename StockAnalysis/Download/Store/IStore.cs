namespace StockAnalysis.Download.Store;

public interface IStore
{
    /// <summary>
    /// Writes stream data into a filesystem. This combines the path and the fileName to form a file.
    /// </summary>
    /// <param name="stream">A stream that will be written into the filesystem.</param>
    /// <param name="path">The path to where in the filesystem the stream should be stored.</param>
    /// <param name="fileName">The name of the file under which the stream will be stored, must not include a file extension.</param>
    /// <returns>A boolean value that determines if the write succeeded or failed.</returns>
    Task<bool> Store(Stream stream, string path, string fileName);
}