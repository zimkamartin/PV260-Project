using System.Text;
using StockAnalysis.Diff.Data;

namespace StockAnalysisTests.DiffTests.DiffComputerTests;

public static class DataGenerator
{
    private static string GenerateRandomTicker()
    {
        var rnd = new Random();
        // Create a StringBuilder to construct the string
        var result = new StringBuilder(3);
        
        // Loop to append three random capital letters
        for (var i = 0; i < 3; i++)
        {
            // Generate a random number between 0 and 25 and add 65 to get a capital letter (A-Z in ASCII)
            var letter = (char)('A' + rnd.Next(26));
            result.Append(letter);
        }
        
        return result.ToString();
    }
    public static List<FundData> GenerateData(int numberOfTickers)
    {
        var newData = new List<FundData>();
        var rnd = new Random();
        for (var i = 1; i <= numberOfTickers; i++)
        {
            var name = Faker.Company.Name();
            var ticker = GenerateRandomTicker();
            var shares = 100 + rnd.Next(-100,100);
            var marketValue = 1000 + rnd.Next(-1000,1000);
            var weight = 10 + rnd.Next(-10,10);

            newData.Add(new FundData
            {
                Company = name,
                Ticker = ticker,
                Shares = shares.ToString(),
                MarketValue = marketValue.ToString(),
                Weight = weight.ToString(),
            });
        }
        return newData;
    }
    
    public static List<FundData> GenerateData(IEnumerable<FundData> data, int newTickets = 0)
    {
        var rnd = new Random();
        var newData  = data.Select(t => new FundData
            {
                Company = t.Company,
                Ticker = t.Ticker,
                Shares = (100 + rnd.Next(-100, 100)).ToString(),
                MarketValue = (1000 + rnd.Next(-1000, 1000)).ToString(),
                Weight = (10 + rnd.Next(-10, 10)).ToString(),
            })
            .ToList();
        newData.AddRange(GenerateData(newTickets));
        return newData;
    }

    public static List<DiffData> GenerateExpectedResult(List<FundData> oldData,
        List<FundData> newData)
    {
        var diffData = new List<DiffData>();
        for (var i = 0; i < newData.Count; i++)
        {
            var ticker = newData[i].Ticker;
            var company = newData[i].Company;
            var sharesChange = int.Parse(newData[i].Shares);
            var marketValueChange = int.Parse(newData[i].MarketValue);
            var weight = int.Parse(newData[i].Weight);
            if (oldData.Any(stock => stock.Ticker == ticker))
            {
                sharesChange -= int.Parse(oldData[i].Shares);
                marketValueChange -= int.Parse(oldData[i].MarketValue);
                weight -= int.Parse(oldData[i].Weight);
            }
            
            diffData.Add(new DiffData()
            {
                Company = company,
                Ticker = ticker,
                SharesChange = sharesChange,
                MarketValueChange = marketValueChange,
                Weight = weight
            });
        }
        return diffData;
    }
}