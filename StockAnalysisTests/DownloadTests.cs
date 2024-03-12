using System.Text;
using StockAnalysis.Download;
using StockAnalysis.HoldingsConfig;

namespace StockAnalysisTests;

public class DownloadTests
{
    private string? _projectRoot;
    [SetUp]
    public void Setup()
    {  
        var current = Environment.CurrentDirectory;
        var projectDirectory = Directory.GetParent(current);
        _projectRoot = current;
        if (projectDirectory is not null)
        {
            _projectRoot = projectDirectory.Parent!.Parent!.FullName;
        }
    }

    [Test]
    public async Task ReadConfigEmpty()
    {
        var config = new Configuration(_projectRoot + "/Mocks/empty_config.json");
        var holdings = await config.LoadConfiguration();
        
        Assert.That(holdings, Is.Empty);
    }

    [Test]
    public async Task ReadConfigSingle()
    {
        var config = new Configuration(_projectRoot + "/Mocks/single_config.json");

        var holdings = await config.LoadConfiguration();
        
        Assert.That(holdings, Has.Length.EqualTo(1));
        Assert.Multiple(() =>
        {
            Assert.That(holdings[0].Name, Is.EqualTo("ARKK-Holdings"));
            Assert.That(holdings[0].Uri, Is.EqualTo("https://ark-funds.com/wp-content/uploads/funds-etf-csv/ARK_INNOVATION_ETF_ARKK_HOLDINGS.csv"));
        });
    }

    [Test]
    public async Task ReadConfigMultiple()
    {
        var config = new Configuration(_projectRoot + "/Mocks/multi_config.json");

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
        var config = new Configuration(_projectRoot + "/Mocks/xxxNotExistsxxx.json");

        var holdings = await config.LoadConfiguration();
        
        Assert.That(holdings, Is.Empty);
    }
    
    [Test]
    public async Task SimpleStorageTest()
    {
        UnicodeEncoding encoding = new();
        const string text = "This is a sample text.";
        var bytes = encoding.GetBytes(text);

        using var memoryStream = new MemoryStream(bytes);
        await Storage.WriteToFileSystem(memoryStream, _projectRoot!, "test.txt");
        var totalPath = Path.Join(_projectRoot, "test.txt");
        Assert.That(File.Exists(totalPath), Is.True);
        var actualBytes = await File.ReadAllBytesAsync(totalPath);
        Assert.That(actualBytes, Is.EqualTo(bytes));
        
        // Cleanup.
        File.Delete(totalPath);
        Assert.That(File.Exists(totalPath), Is.False);
    }

    [Test]
    public async Task DownloadSingleCsv()
    {
        HoldingInformation[] holdings =
        {
            new("ARKK-Holdings",
                "https://ark-funds.com/wp-content/uploads/funds-etf-csv/ARK_INNOVATION_ETF_ARKK_HOLDINGS.csv")
        };
        var manager = new DownloadManager(".");
        using var client = new HttpClient();
        
        // This is necessary, otherwise the website will reject our request.
        client.DefaultRequestHeaders.Add("User-Agent", "Other");
        
        Assert.That(await manager.DownloadHoldingsCsv(holdings, client), Is.True);
        Assert.That(File.Exists("./ARKK-Holdings.csv"), Is.True);
        
        // Cleanup.
        File.Delete("./ARKK-Holdings.csv");
        Assert.That(File.Exists("./ARKK-Holdings.csv"), Is.False);
    }

    [Test]
    public async Task DownloadMultipleCsv()
    {
        HoldingInformation[] holdings =
        {
            new("ARKK-Holdings",
                "https://ark-funds.com/wp-content/uploads/funds-etf-csv/ARK_INNOVATION_ETF_ARKK_HOLDINGS.csv"),
            new("ARKG-Holdings",
                "https://ark-funds.com/wp-content/uploads/funds-etf-csv/ARK_GENOMIC_REVOLUTION_ETF_ARKG_HOLDINGS.csv")
        };
        var manager = new DownloadManager(".");
        using var client = new HttpClient();
        
        // This is necessary, otherwise the website will reject our request.
        client.DefaultRequestHeaders.Add("User-Agent", "Other");
        
        Assert.That(await manager.DownloadHoldingsCsv(holdings, client), Is.True);
        Assert.That(File.Exists("./ARKK-Holdings.csv"), Is.True);
        Assert.That(File.Exists("./ARKG-Holdings.csv"), Is.True);
        
        // Cleanup.
        File.Delete("./ARKK-Holdings.csv");
        Assert.That(File.Exists("./ARKK-Holdings.csv"), Is.False);
        File.Delete("./ARKG-Holdings.csv");
        Assert.That(File.Exists("./ARKG-Holdings.csv"), Is.False);
        
    }
}