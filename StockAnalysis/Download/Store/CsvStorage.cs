namespace StockAnalysis.Download.Store;

public class CsvStorage : IStore
{
    /// <summary>
    /// Stores data within a stream into the filesystem, combining path and name to form a file.
    /// </summary>
    /// <param name="stream">A stream that will be written into the filesystem.</param>
    /// <param name="path">The path to where in the filesystem the stream should be stored.</param>
    /// <param name="fileName">The name of the file under which the stream will be stored, must not include a file extension.</param>
    /// <returns>Value indicating whether the write succeeded.</returns>
    /// <exception cref="ObjectDisposedException">The stream was closed prematurely.</exception>
    /// <exception cref="PathTooLongException">The path is too long.</exception>
    /// <exception cref="UnauthorizedAccessException">The file could not be created.</exception>
    public async Task<bool> Store(Stream stream, string path, string fileName)
    {
        if (stream is { CanRead: false, CanSeek: false })
        {
            return false;
        }
        
        // Idea for storage:
        // root/default/default.csv -> default file, empty.
        // root/07022022/holding.csv -> older file
        // root/14022022/holding.csv -> current file, we check if there is an older file by checking date - period
        var finalPath = Path.Combine(path, 
                                          fileName + Constants.Constants.NewSuffix 
                                               + Constants.Constants.CsvExtension);
        try
        {
            stream.Seek(0, SeekOrigin.Begin);
            await using var fileStream = File.Create(finalPath);
            await stream.CopyToAsync(fileStream);
            RemoveLastLine(fileStream);
            return true;
        }
        catch (Exception e) when (e is DirectoryNotFoundException 
                                      or NotSupportedException 
                                      or IOException)
        {
            return false;
        }
        // Rethrow.
        catch (Exception)
        {
            throw;
        }
    }

    // The last line in the ARK Holdings csv files is filled with plaintext that makes the diff tool crash. :(
    private static void RemoveLastLine(Stream stream)
    {
        stream.Seek(0, SeekOrigin.Begin);
        using var reader = new StreamReader(stream);
        long offset = 0;
        long prevLineLen = 0;
        var line = "";
        while (line != null)
        {
            offset += line.Length + 1;
            line = reader.ReadLine();
            if (line != null)
            {
                prevLineLen = line.Length + 1;
            }
        }

        if (offset - prevLineLen - 1 > 0)
        {
            stream.SetLength(offset - prevLineLen - 1);
        }
    }
}