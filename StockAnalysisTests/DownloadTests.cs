using NUnit.Framework.Interfaces;
using StockAnalysis.Download;
using StockAnalysis.HoldingsConfig;

namespace StockAnalysisTests;

public class DownloadTests
{
    private string projectRoot;
    [SetUp]
    public void Setup()
    {  
        var current = Environment.CurrentDirectory;
        var projectDirectory = Directory.GetParent(current);
        projectRoot = current;
        if (projectDirectory is not null)
        {
            projectRoot = projectDirectory!.Parent!.Parent!.FullName;
        }
    }

    [Test]
    public async Task ReadConfigEmpty()
    {
        var config = new Configuration(projectRoot + "/Mocks/empty_config.json");
        var holdings = await config.LoadConfiguration();
        
        Assert.That(holdings, Is.Empty);
    }

    [Test]
    public async Task ReadConfigSingle()
    {
        var config = new Configuration(projectRoot + "/Mocks/single_config.json");

        var holdings = await config.LoadConfiguration();
        
        Assert.That(holdings, Has.Length.EqualTo(1));
        Assert.That(holdings[0].Name, Is.EqualTo("ARKK-Holdings"));
        Assert.That(holdings[0].Uri, Is.EqualTo("https://ark-funds.com/wp-content/uploads/funds-etf-csv/ARK_INNOVATION_ETF_ARKK_HOLDINGS.csv"));
    }

    [Test]
    public async Task ReadConfigMultiple()
    {
        var config = new Configuration(projectRoot + "/Mocks/multi_config.json");

        var holdings = await config.LoadConfiguration();
        
        Assert.That(holdings, Has.Length.EqualTo(3));
        Assert.Multiple(() =>
        {
            Assert.That(holdings[0].Name, Is.EqualTo("Holding1"));
            Assert.That(holdings[0].Uri, Is.EqualTo("Uri1"));
        });
        
        Assert.Multiple(() =>
        {
            Assert.That(holdings[1].Name, Is.EqualTo("Holding2"));
            Assert.That(holdings[1].Uri, Is.EqualTo(""));
        });
        
        Assert.Multiple(() =>
        {
            Assert.That(holdings[2].Name, Is.EqualTo("Holding3"));
            Assert.That(holdings[2].Uri, Is.EqualTo(".Uri3"));
        });
    }

    [Test]
    public async Task ReadConfigFileNotExists()
    {
        var config = new Configuration(projectRoot + "/Mocks/xxxNotExistsxxx.json");

        var holdings = await config.LoadConfiguration();
        
        Assert.That(holdings, Is.Empty);
    }

    [Test]
    public async Task DownloadSingleCsv()
    {
        HoldingInformation[] holdings =
        {
            new HoldingInformation("ARKK-Holdings",
                "https://ark-funds.com/wp-content/uploads/funds-etf-csv/ARK_INNOVATION_ETF_ARKK_HOLDINGS.csv")
        };
        var manager = new DownloadManager(".");
        var client = new HttpClient();
        client.DefaultRequestHeaders.Add("User-Agent", "Other");
        Assert.That(await manager.DownloadHoldingsCsv(holdings, client), Is.True);
        Assert.That(File.Exists("./ARKK-Holdings.csv"), Is.True);
    }

    // [Test]
    // public async Task SimpleStorageTest()
    // {
    //     var streamWriter = new StreamWriter(stream);
    //     Storage.WriteToFileSystem()
    // }
}