using StockAnalysis.Constants;
using StockAnalysis.Download.Getter;
using StockAnalysis.Download.Store;

namespace StockAnalysisConsole
{
    internal static class Program
    {
        public static async Task Main()
        {
            // TODO: Option to set the email addresses.
            var addresses = new List<string>(){ "514182@mail.muni.cz" };
            
            Console.WriteLine("Starting analyzer. ");
            
            var manager = new AnalysisManager(new CsvDownload(), new CsvStorage());
            
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Add("User-Agent", "Other");
            
            // TODO: Option to choose the period.
            Console.WriteLine("Would you like to set up the periodic downloader for 1 month? y/n");
            var res = Console.ReadKey(true);
            if (res.KeyChar != 'y')
            {
                await manager.PerformAnalysis(client, Constants.CsvExtension, addresses);
                return;
            }
            
            Console.WriteLine("Setting up a periodic analysis.");
            // Note: This will just end the program right now. Setting this up to run forever and hosting it on some server is necessary.
            // var period = new Period(PeriodType.Monthly, DateTime.Today);
            // var downloader = new PeriodicalDownloader(manager, period, new SystemDateTime(), holdings, client);
            // downloader.SchedulePeriodicDownload();
            Console.WriteLine("Download scheduled.");
        }
    }
}