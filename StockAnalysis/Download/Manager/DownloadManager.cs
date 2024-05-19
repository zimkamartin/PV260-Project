using StockAnalysis.Download.Getter;
using StockAnalysis.Download.Store;
using StockAnalysis.HoldingsConfig;

namespace StockAnalysis.Download.Manager;

public class DownloadManager
{
    private readonly IGetter _getter;
    private readonly IStore _storage;
    private readonly HttpClient _client;
    public string StoragePath { get; }

    public DownloadManager(string storagePath, IGetter getter, IStore store, HttpClient client)
    {
        StoragePath = storagePath;
        _getter = getter;
        _storage = store;
        _client = client;
    }

    /// <summary>
    /// Orchestrates the download process for a group of holdings.
    /// </summary>
    /// <param name="holdings">Information about the desired holdings - used for download and file storage names.</param>
    /// <param name="storageDirectory">The name of the specific directory a given file is stored into. Will be created if it does not exist yet.</param>
    /// <returns>Boolean value determining whether the whole process succeeded. Note - it may happen that some files are successfully stored before a failure occurs. The method stops at the first failure.</returns>
    public async Task<bool> GetHoldings(IEnumerable<HoldingInformation> holdings, string storageDirectory)
    {
        try
        {
            foreach (var uri in holdings)
            {
                await using var stream = await _getter.Get(uri.Uri, _client);
                
                if (stream is null 
                    || !await _storage.Store(stream, StoragePath, storageDirectory, uri.Name))
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