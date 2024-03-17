using StockAnalysis.HoldingsConfig;

namespace StockAnalysis.Download;

public class DownloadManager
{
    public string StoragePath { get; set; }
    private const string CsvExtension = ".csv";

    public DownloadManager(string storagePath)
    {
        StoragePath = storagePath;
    }

    /// <summary>
    /// Orchestrates the download process for a group of holdings. The format of storage is a .csv.
    /// </summary>
    /// <param name="holdings">Information about the desired holdings - used for download and file storage names.</param>
    /// <param name="client">HttpClient used to make the download requests. This method does not modify the client. The client may need to have the default request header for "User-Agent" set to "Other" for the download to work!</param>
    /// <returns>Boolean value determining whether the whole process succeeded. Note - it may happen that some files are successfully stored before a failure occurs. The method stops at the first failure.</returns>
    public async Task<bool> DownloadHoldingsCsv(IEnumerable<HoldingInformation> holdings, HttpClient client)
    {
        try
        {
            foreach (var uri in holdings)
            {
                await using var stream = await Download.GetCsv(uri.Uri, client);
                if (!await Storage.WriteToFileSystem(stream, StoragePath, uri.Name + CsvExtension))
                {
                    return false;
                }
            }
        }
        catch (Exception e) when (e is HttpRequestException 
                                      or ArgumentNullException 
                                      or InvalidOperationException 
                                      or TaskCanceledException)
        {
            return false;
        }
        // Rethrow the more serious exceptions on our end - Unauthorized Access, Path Too Long, etc.
        catch (Exception)
        {
            throw;
        }

        return true;
    }
}