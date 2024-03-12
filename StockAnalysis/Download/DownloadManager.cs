using System.IO;

namespace StockAnalysis.Download;

public class DownloadManager
{
    public string StoragePath { get; set; }
    private const string CsvExtension = ".csv";

    public DownloadManager(string storagePath)
    {
        StoragePath = storagePath;
    }

    public async Task<bool> DownloadHoldingsCsv(IEnumerable<HoldingInformation> uris, HttpClient client)
    {
        try
        {
            foreach (var uri in uris)
            {
                await using var stream = await Download.GetCsv(uri.Uri, client);
                return await Storage.WriteToFileSystem(stream, StoragePath, uri.Name + CsvExtension);
            }
        }
        catch (Exception)
        {
            // ToDo: Handle exceptions here.
            throw;
        }

        return true;
    }
}