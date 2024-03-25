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
    public async Task Configuration_LoadConfiguration_EmptyFile()
    {
        var config = new JsonConfiguration(_projectRoot + "/Mocks/empty_config.json");
        var holdings = await config.LoadConfiguration();

        Assert.That(holdings, Is.Empty);
    }

    [Test]
    public async Task Configuration_LoadConfiguration_SingleEntry()
    {
        // Arrange
        var config = new JsonConfiguration(_projectRoot + "/Mocks/single_config.json");

        // Act
        var holdings = await config.LoadConfiguration();
        var holdingInformation = holdings.ToList();
        
        // Assert
        Assert.That(holdingInformation, Has.Count.EqualTo(1));
        Assert.Multiple(() =>
        {
            Assert.That(holdingInformation[0].Name, Is.EqualTo("ARKK-Holdings"));
            Assert.That(holdingInformation[0].Uri,
                Is.EqualTo(
                    "https://ark-funds.com/wp-content/uploads/funds-etf-csv/ARK_INNOVATION_ETF_ARKK_HOLDINGS.csv"));
        });
    }

    [Test]
    public async Task Configuration_LoadConfiguration_MultipleEntries()
    {
        var config = new JsonConfiguration(_projectRoot + "/Mocks/multi_config.json");

        var holdings = await config.LoadConfiguration();
        var holdingInformation = holdings.ToList();
        
        Assert.That(holdingInformation, Has.Count.EqualTo(3));
        Assert.Multiple(() =>
        {
            Assert.That(holdingInformation[0].Name, Is.EqualTo("Holding1"));
            Assert.That(holdingInformation[0].Uri, Is.EqualTo("Uri1"));
        });

        Assert.Multiple(() =>
        {
            Assert.That(holdingInformation[1].Name, Is.EqualTo("Holding2"));
            Assert.That(holdingInformation[1].Uri, Is.EqualTo(""));
        });

        Assert.Multiple(() =>
        {
            Assert.That(holdingInformation[2].Name, Is.EqualTo("Holding3"));
            Assert.That(holdingInformation[2].Uri, Is.EqualTo(".Uri3"));
        });
    }

    [Test]
    public async Task Configuration_LoadConfiguration_NonExistentFile()
    {
        var config = new JsonConfiguration(_projectRoot + "/Mocks/xxxNotExistsxxx.json");

        try
        {
            _ = await config.LoadConfiguration();
        }
        catch (ConfigurationException)
        {
            Assert.Pass();
        }

        Assert.Fail("The method was expected to throw an exception.");
    }
}