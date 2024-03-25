using StockAnalysis.Constants;
using StockAnalysis.Diff.Compute;
using StockAnalysis.Diff.Load;
using StockAnalysis.Diff.Store;
using StockAnalysis.Download;
using StockAnalysis.Download.Getter;
using StockAnalysis.Download.PeriodicalDownload;
using StockAnalysis.Download.Store;
using StockAnalysis.Sending.ClientGenerator;
using StockAnalysis.Sending.Sender;
using StockAnalysis.Utilities.Email;
using StockAnalysisConsole.Utils.Paths;

namespace StockAnalysisConsole
{
    internal static class Program
    {
        private const string ClientHost = "smtp.gmail.com";
        private const int SmtpPort = 587;
        private const string SenderMail = "pv260.s24.goth.pinkteam@gmail.com";

        public static async Task Main()
        {
            Console.WriteLine("Welcome to StockAnalysis.");

            var addresses = await LoadAddresses();
            if (!addresses.Any())
            {
                Console.WriteLine("No addresses provided - e-mails will not be sent.");
                return;
            }

            var period = SetPeriod();

            Console.WriteLine("Starting analyzer. ");

            // Create sender and manager.
            var sender = new Sender(SenderMail,
                new SmtpClientGenerator(SmtpPort,
                    SenderMail,
                    true,
                    ClientHost));
            
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Add("User-Agent", "Other");

            var downloadManager =
                new DownloadManager(Paths.GetDownloadFolderPath(), new CsvDownload(), 
                                new CsvStorage(), client);
            
            var manager = new AnalysisManager(downloadManager, new CsvDiffComputer(new CsvHoldingLoader()), 
                                                new CsvDiffStore());

            List<string> diffPaths;
            try
            {
                diffPaths = await manager.PerformAnalysis(client, Constants.CsvExtension, period);

                if (diffPaths.Count == 0)
                {
                    Console.WriteLine("Failed to obtain any holding diffs.");
                    return;
                }
            }
            catch (Exception)
            {
                Console.WriteLine("Analysis failed.");
                return;
            }

            // Send e-mails.
            await SendEmails(sender, addresses, diffPaths);

            Console.WriteLine("Have a nice day!");
        }


        /// <summary>
        /// Load addresses from file (JSON) or CLI.
        /// </summary>
        /// <returns>List of e-mail addresses of recipients.</returns>
        private static async Task<string[]> LoadAddresses()
        {
            Console.WriteLine("Would you like to load emails from Emails.json? y/n");
            string[] addresses;
            var key = Console.ReadKey(true);
            if (key.KeyChar == 'y')
            {
                try
                {
                    addresses = await EmailReader.ReadFromJson(Paths.GetEmailFilePath());
                }
                catch (Exception)
                {
                    Console.WriteLine("Failed to read addresses from file.");
                    addresses = EmailReader.ReadFromCli();
                }
            }
            else
            {
                addresses = EmailReader.ReadFromCli();
            }

            return addresses;
        }

        /// <summary>
        /// Allows user to configure period for analysis.
        /// </summary>
        private static Period? SetPeriod()
        {
            // TODO: Add options for period setting.
            Console.WriteLine("Would you like to set monthly period event for analysis? y/n");

            Period? period = null;
            if (Console.ReadKey(true).KeyChar == 'y')
            {
                Console.WriteLine("Monthly period event set.");
                period = new Period(PeriodType.Monthly, DateTime.UtcNow);
            }

            return period;
        }

        /// <summary>
        /// Sends e-mails to recipients.
        /// </summary>
        private static async Task SendEmails(ISender sender, IEnumerable<string> addresses, IEnumerable<string> diffPaths)
        {
            try
            {
                Console.WriteLine("Analysis completed. Sending e-mails... ");
                await sender.SendNotification(addresses, diffPaths);
            }
            catch (SenderException)
            {
                Console.WriteLine("Email sending failed.");
                return;
            }

            Console.WriteLine("E-mails sent. Check your inbox.");
        }
    }
}