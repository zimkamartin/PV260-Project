using StockAnalysis.Constants;
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
            Console.WriteLine("Would you like to load emails from Emails.json? y/n");
            var now = DateTime.UtcNow;
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

            if (!addresses.Any())
            {
                Console.WriteLine("No addresses provided - emails will not be sent.");
            }
            // TODO: Add options for period 
            Console.WriteLine("Would you like to set monthly period event for analysis? y/n");

            Period? period = null;
            if (Console.ReadKey(true).KeyChar == 'y')
            {
                Console.WriteLine("Monthly period event set.");
                period = new Period(PeriodType.Monthly, now);
            }

            Console.WriteLine("Starting analyzer. ");

            var sender = new Sender(SenderMail, 
                                    new SmtpClientGenerator(SmtpPort, 
                                                    SenderMail, 
                                                         true, 
                                                        ClientHost));
            
            var manager = new AnalysisManager(new CsvDownload(), 
                                                new CsvStorage(),
                                                sender);
            
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Add("User-Agent", "Other");
            
            await manager.PerformAnalysis(client, Constants.CsvExtension, addresses, period);
        }
    }
}