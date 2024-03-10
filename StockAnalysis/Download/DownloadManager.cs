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

    public async Task<bool> DownloadHoldingsCsv(HoldingInformation[] uris, HttpClient client)
    {
        try
        {
            foreach (var uri in uris)
            {
                var stream = await Download.GetCsv(uri.Uri, client);
                await Storage.WriteToFileSystem(stream, StoragePath, uri.Name + CsvExtension);
                stream.Close();
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