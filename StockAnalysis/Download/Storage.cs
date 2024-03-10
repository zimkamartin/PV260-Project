namespace StockAnalysis.Download;

public class Storage
{

    public static async Task WriteToFileSystem(Stream stream, string path, string name)
    {
        var finalPath = Path.Combine(path, name);
        try
        {
            await using var fileStream = File.Create(finalPath);
            await stream.CopyToAsync(fileStream);
        }
        catch (Exception)
        {
            throw;
        }
    }
}