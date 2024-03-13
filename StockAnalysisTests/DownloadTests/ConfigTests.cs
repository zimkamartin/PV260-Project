using System.Text;
using StockAnalysis.HoldingsConfig;

namespace StockAnalysisTests.DownloadTests;

public class ConfigTests
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
}