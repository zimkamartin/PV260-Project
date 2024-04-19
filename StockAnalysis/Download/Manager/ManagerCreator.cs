using StockAnalysis.Download.Getter;
using StockAnalysis.Download.Store;

namespace StockAnalysis.Download.Manager;

public static class ManagerCreator
{
    public static DownloadManager CreateManager( string downloadFolderPath, HttpClient client, string extension )
    {
        IGetter getter;
        IStore store;
        switch (extension.Replace(".", "").ToLower())
        {
            case "csv":
                getter = new CsvDownload();
                store = new CsvStorage();
                break;
            default:
                throw new NotImplementedException("Chosen extension is currently not supported.");
        }

        return new DownloadManager(downloadFolderPath, getter, store, client);
    }
}