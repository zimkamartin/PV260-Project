// See https://aka.ms/new-console-template for more information
using CsvHelper;
using CsvHelper.Configuration.Attributes;
using System.Globalization;

namespace SemestralProject
{
    public class FundData
    {
        [Name("date")]
        public DateTime Date { get; set; }

        [Name("fund")]
        public string Fund { get; set; }

        [Name("company")]
        public string Company { get; set; }

        [Name("ticker")]
        public string Ticker { get; set; }

        [Name("cusip")]
        public string Cusip { get; set; }

        [Name("shares")]
        public long Shares { get; set; }

        [Name("market value ($)")]
        public decimal MarketValue { get; set; }

        [Name("weight (%)")]
        public decimal Weight { get; set; }
    }

    class Program
    {
        static void Main(string[] args)
        {
            // Load old.csv and new.csv
            List<FundData> oldData = LoadData("old.csv");
            List<FundData> newData = LoadData("new.csv");

            // Compute changes
            var changes = ComputeChanges(oldData, newData);

            // Output changes
            Console.WriteLine("Changes:");
            foreach (var change in changes)
            {
                Console.WriteLine($"Company: {change.Company}, Ticker: {change.Ticker}, Shares Change: {change.SharesChange}, Market Value Change: {change.MarketValueChange}, Weight Change: {change.WeightChange}");
            }
        }

        static List<FundData> LoadData(string filename)
        {
            using (var reader = new StreamReader(filename))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                return csv.GetRecords<FundData>().ToList();
            }
        }

        static List<ChangeData> ComputeChanges(List<FundData> oldData, List<FundData> newData)
        {
            var changes = new List<ChangeData>();

            foreach (var newDataEntry in newData)
            {
                var oldDataEntry = oldData.FirstOrDefault(x => x.Ticker == newDataEntry.Ticker);

                if (oldDataEntry != null)
                {
                    var sharesChange = newDataEntry.Shares - oldDataEntry.Shares;
                    var marketValueChange = newDataEntry.MarketValue - oldDataEntry.MarketValue;
                    var weightChange = newDataEntry.Weight - oldDataEntry.Weight;

                    changes.Add(new ChangeData
                    {
                        Company = newDataEntry.Company,
                        Ticker = newDataEntry.Ticker,
                        SharesChange = sharesChange,
                        MarketValueChange = marketValueChange,
                        WeightChange = weightChange
                    });
                }
            }

            return changes;
        }
    }

    public class ChangeData
    {
        public string Company { get; set; }
        public string Ticker { get; set; }
        public long SharesChange { get; set; }
        public decimal MarketValueChange { get; set; }
        public decimal WeightChange { get; set; }
    }
}