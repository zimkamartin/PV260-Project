using CsvHelper;
using CsvHelper.Configuration.Attributes;
using System.Globalization;

namespace StockAnalysis
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
        public string Shares { get; set; }

        [Name("market value ($)")]
        public string MarketValue { get; set; }

        [Name("weight (%)")]
        public string Weight { get; set; }
    }

    class Program
    {
        public static void Main(string[] args)
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
                    var sharesChange = StringToNumber(newDataEntry.Shares) - StringToNumber(oldDataEntry.Shares);
                    var marketValueChange = StringToNumber(newDataEntry.MarketValue) - StringToNumber(oldDataEntry.MarketValue);
                    var weightChange = StringToNumber(newDataEntry.Weight) - StringToNumber(oldDataEntry.Weight);

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

        static double StringToNumber(String data)
        {
            data = data.Replace(",", "");
            data = data.Replace("$","");
            data = data.Replace("%", "");
            Console.WriteLine(data+"DATAAAAAAAAAAAAAAAAAA");
            return double.Parse(data, CultureInfo.InvariantCulture);
        }
    }

    public class ChangeData
    {
        public string Company { get; set; }
        public string Ticker { get; set; }
        public double SharesChange { get; set; }
        public double MarketValueChange { get; set; }
        public double WeightChange { get; set; }
    }
}