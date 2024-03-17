using StockAnalysis.Diff;
using StockAnalysis.Download;
using StockAnalysis.Download.PeriodicalDownload;
using StockAnalysis.HoldingsConfig;
using StockAnalysis.SendEmail;
using StockAnalysis.Utilities;

namespace StockAnalysisConsole
{
    internal static class Program
    {
        public static async Task Main()
        {
            var addresses = new List<string>(){ "514369@mail.muni.cz" };
            Console.WriteLine("Starting analyzer. ");
            var current = Environment.CurrentDirectory;
            var projectDirectory = Directory.GetParent(current);
            var projectRoot = current;
            if (projectDirectory is not null)
            {
                projectRoot = projectDirectory.Parent!.Parent!.FullName;
            }

            var config = new Configuration(Path.Combine(projectRoot, "Config", "Configuration.json"));
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Add("User-Agent", "Other");
            
            DownloadManager manager = new(Path.Combine(projectRoot, "Downloads"));
            var holdings = await config.LoadConfiguration();
            
            Console.WriteLine("Would you like to set up the periodic downloader for 1 month? y/n");
            var res = Console.ReadKey(true);
            if (res.KeyChar != 'y')
            {
                Console.WriteLine("Performing a single aperiodic run of analysis.");
                if (!await manager.DownloadHoldingsCsv(holdings, client))
                {
                    Console.WriteLine("Failed to download the required files.");
                    return;
                }
                Console.WriteLine("ETF Holdings downloaded.");

                var attachmentsPaths = new List<string>();
                foreach ( var holding in holdings )
                {
                    var data = DiffComputer.CreateDiff(Path.Combine(projectRoot, "Downloads", holding.Name + ".csv"));
                    var storePath = Path.Combine(projectRoot, "Diff");
                    await DiffStore.StoreDiff(data, storePath, holding.Name);
                    var filePath = Path.Combine(storePath, holding.Name + ".csv");
                    Console.WriteLine("ETF diff stored at: " + filePath);
                    attachmentsPaths.Add(filePath);
                }
                Console.WriteLine("Sending emails.");
                await Sender.SendMail(addresses, attachmentsPaths);
                Console.WriteLine("Emails sent.");
                
                Console.WriteLine("Analysis finished.");
                return;
            }
            
            Console.WriteLine("Setting up a periodic analysis.");
            // Note: This will just end the program right now. Setting this up to run forever and hosting it on some server is necessary.
            var period = new Period(PeriodType.Monthly, DateTime.Today);
            var downloader = new PeriodicalDownloader(manager, period, new SystemDateTime(), holdings, client);
            downloader.SchedulePeriodicDownload();
            Console.WriteLine("Download scheduled.");

        }
    }
}