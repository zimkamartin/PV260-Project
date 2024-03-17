namespace StockAnalysis.Download;

public static class Storage
{
    /// <summary>
    /// Stores data within a stream into the filesystem, combining path and name to form a file.
    /// </summary>
    /// <param name="stream">A stream that will be completely stored into a file. This function DOES NOT close the stream.</param>
    /// <param name="path">The path to where the stream contents should be stored.</param>
    /// <param name="name">The desired name of the file that will be created.</param>
    /// <returns>Value indicating whether the write succeeded.</returns>
    /// <exception cref="ObjectDisposedException">The stream was closed prematurely.</exception>
    /// <exception cref="PathTooLongException">The path is too long.</exception>
    /// <exception cref="UnauthorizedAccessException">The file could not be created.</exception>
    public static async Task<bool> WriteToFileSystem(Stream stream, string path, string name)
    {
        if (stream is { CanRead: false, CanSeek: false })
        {
            return false;
        }
        var finalPath = Path.Combine(path, name);
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
        long prev_line_len = 0;
        string? line = "";
        while (line != null)
        {
            offset += line.Length + 1;
            line = reader.ReadLine();
            if (line != null)
            {
                prev_line_len = line.Length + 1;
            }
        }

        if (offset - prev_line_len - 1 > 0)
        {
            stream.SetLength(offset - prev_line_len - 1);
        }
    }
}