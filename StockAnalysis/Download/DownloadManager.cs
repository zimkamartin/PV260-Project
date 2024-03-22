using StockAnalysis.Download.Getter;
using StockAnalysis.Download.Store;
using StockAnalysis.HoldingsConfig;

namespace StockAnalysis.Download;

public class DownloadManager
{
    private readonly IGetter _getter;
    private readonly IStore _storage;
    public string StoragePath { get; set; }

    public DownloadManager(string storagePath, IGetter getter, IStore store)
    {
        StoragePath = storagePath;
        _getter = getter;
        _storage = store;
    }

    /// <summary>
    /// Orchestrates the download process for a group of holdings.
    /// It is virtual because it is meant to be overridden in tests.
    /// </summary>
    /// <param name="holdings">Information about the desired holdings - used for download and file storage names.</param>
    /// <param name="client">HttpClient used to make the download requests. This method does not modify the client. The client may need to have the default request header for "User-Agent" set to "Other" for the download to work!</param>
    /// <param name="storageDirectory">The name of the specific directory a given file is stored into. Will be created if it does not exist yet.</param>
    /// <returns>Boolean value determining whether the whole process succeeded. Note - it may happen that some files are successfully stored before a failure occurs. The method stops at the first failure.</returns>
    public virtual async Task<bool> GetHoldings(IEnumerable<HoldingInformation> holdings,
                                                HttpClient client,
                                                string storageDirectory)
    {
        try
        {
            foreach (var uri in holdings)
            {
                await using var stream = await _getter.Get(uri.Uri, client);
                if (!await _storage.Store(stream, StoragePath, storageDirectory, uri.Name))
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