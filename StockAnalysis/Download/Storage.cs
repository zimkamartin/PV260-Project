namespace StockAnalysis.Download;

public class Storage
{
    /// <summary>
    /// Stores data within a stream into the filesystem, combining path and name to form a file.
    /// </summary>
    /// <param name="stream">A stream that will be completely stored into a file. This function DOES NOT close the stream.</param>
    /// <param name="path">The path to where the stream contents should be stored.</param>
    /// <param name="name">The desired name of the file that will be created.</param>
    public static async Task WriteToFileSystem(Stream stream, string path, string name)
    {
        var finalPath = Path.Combine(path, name);
        try
        {
            stream.Seek(0, SeekOrigin.Begin);
            await using var fileStream = File.Create(finalPath);
            await stream.CopyToAsync(fileStream);
        }
        catch (Exception)
        {
            throw;
        }
    }
}