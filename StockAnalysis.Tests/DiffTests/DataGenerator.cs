using System.Text;
using Faker;
using StockAnalysis.Diff.Data;

namespace StockAnalysisTests.DiffTests;

public class DataGenerator
{
    public static string GenerateRandomTicker()
    {
        Random rnd = new Random();
        // Create a StringBuilder to construct the string
        var result = new StringBuilder(3);
        
        // Loop to append three random capital letters
        for (int i = 0; i < 3; i++)
        {
            // Generate a random number between 0 and 25 and add 65 to get a capital letter (A-Z in ASCII)
            char letter = (char)('A' + rnd.Next(26));
            result.Append(letter);
        }
        
        return result.ToString();
    }
    public static List<FundData> GenerateData(int numberOfTickers)
    {
        var newData = new List<FundData>();
        Random rnd = new Random();
        for (int i = 1; i <= numberOfTickers; i++)
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
    
    public static List<FundData> GenerateData(List<FundData> data, int newTickets = 0)
    {
        Random rnd = new Random();
        List<FundData> newData  = new List<FundData>();
        for (int i = 0; i < data.Count; i++)
        {
            newData.Add(new FundData
            {
                Company = data[i].Company,
                Ticker = data[i].Ticker,
                Shares = (100 + rnd.Next(-100, 100)).ToString(),
                MarketValue = (1000 + rnd.Next(-1000, 1000)).ToString(),
                Weight = (10 + rnd.Next(-10, 10)).ToString(),
            });
        }
        newData.AddRange(GenerateData(newTickets));
        return newData;
    }

    public static List<DiffData> DiffCorrectCalculation(List<FundData> oldData,
        List<FundData> newData)
    {
        List<DiffData> diffData = new List<DiffData>();
        for (int i = 0; i < newData.Count; i++)
        {
            var ticker = newData[i].Ticker;
            var company = newData[i].Company;
            var sharesChange = Int32.Parse(newData[i].Shares);
            var marketValueChange = Int32.Parse(newData[i].MarketValue);
            var weight = Int32.Parse(newData[i].Weight);
            if (oldData.Any(stock => stock.Ticker == ticker))
            {
                sharesChange -= Int32.Parse(oldData[i].Shares);
                marketValueChange -= Int32.Parse(oldData[i].MarketValue);
                weight -= Int32.Parse(oldData[i].Weight);
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
    
    public static bool CompareData(List<DiffData> data1, List<DiffData> data2)
    {
        if (data1.Count != data2.Count)
        {
            return false;
        }

        for (int i = 0; i < data1.Count; i++)
        {
            if (data1[i].Ticker != data2[i].Ticker)
            {
                return false;
            }

            if (data1[i].SharesChange != data2[i].SharesChange)
            {
                return false;
            }

            if (data1[i].MarketValueChange != data2[i].MarketValueChange)
            {
                return false;
            }

            if (data1[i].Weight != data2[i].Weight)
            {
                return false;
            }
        }

        return true;
    }

    public static List<DiffData> GenerateDiffData()
    {
        var data1 = DataGenerator.GenerateData(2);
        var data2= DataGenerator.GenerateData(data1);
        var data = DataGenerator.DiffCorrectCalculation(data1, data2);
        return data;
    }
}