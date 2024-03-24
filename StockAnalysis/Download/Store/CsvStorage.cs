namespace StockAnalysis.Download.Store;

public class CsvStorage : IStore
{
    /// <summary>
    /// Stores data within a stream into the filesystem, combining path and name to form a file.
    /// </summary>
    /// <param name="stream">A stream that will be written into the filesystem.</param>
    /// <param name="storagePath">The path to where in the filesystem the new files are saved.</param>
    /// <param name="storageDirectory">The name of the specific directory a given file is stored.</param>
    /// <param name="fileName">The name of the file under which the stream will be stored, must not include a file extension.</param>
    /// <returns>Value indicating whether the write succeeded.</returns>
    /// <exception cref="ObjectDisposedException">The stream was closed prematurely.</exception>
    /// <exception cref="PathTooLongException">The path is too long.</exception>
    /// <exception cref="UnauthorizedAccessException">The file could not be created.</exception>
    public async Task<bool> Store(Stream stream, string storagePath, string storageDirectory, string fileName)
    {
        if (stream is { CanRead: false, CanSeek: false })
        {
            return false;
        }

        var fullStoragePath = Path.Combine(storagePath, storageDirectory);
        if (!Directory.Exists(fullStoragePath))
        {
            Directory.CreateDirectory(fullStoragePath);
        }

        var finalPath = Path.Combine(fullStoragePath,
            fileName
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

    /// <summary>
    /// Removes the last line from a stream.
    /// Reason: The last line in the ARK Holdings csv files is filled with plaintext that makes the diff tool crash. :(
    /// </summary>
    private static void RemoveLastLine(Stream stream)
    {
        stream.Seek(0, SeekOrigin.Begin);
        using var reader = new StreamReader(stream);
        long offset = 0;
        long prevLineLen = 0;
        while (reader.ReadLine() is { } line)
        {
            offset += prevLineLen;
            prevLineLen = line.Length + 1;
        }

        if (offset > 0)
        {
            stream.SetLength(offset);
        }
    }
}