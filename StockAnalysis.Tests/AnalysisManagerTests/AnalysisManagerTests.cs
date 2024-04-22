using StockAnalysis.Diff.Compute;
using StockAnalysis.Diff.Store;
using StockAnalysis.Download.Manager;
using StockAnalysis.Utilities;
using StockAnalysisConsole;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;

namespace StockAnalysisTests.AnalysisManagerTests;

public class AnalysisManagerTests
{
    private string? _projectRoot;
    private WireMockServer _server;

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
        
        _server = WireMockServer.Start(9876);
        // Most of the body is simply arbitrary data and has no effect on tests.
        _server.Given(Request.Create().WithPath("/ARK_INNOVATION_ETF_ARKK_HOLDINGS.csv").UsingGet()
        ).RespondWith(
            Response
                .Create()
                .WithStatusCode(200)
                .WithHeader("Content-Type", "text/csv")
                .WithBody("date,fund,company,ticker,cusip,shares,\"market value ($)\",\"weight (%)\"\n" +
                          "04/16/2024,ARKK,\"TESLA INC\",TSLA,88160R101,\"4,028,071\",\"$650,452,905.08\",9.83%\n" +
                          "04/16/2024,ARKK,\"COINBASE GLOBAL INC -CLASS A\",COIN,19260Q107,\"2,630,233\",\"$587,620,354.53\",8.88%\n" +
                          "04/16/2024,ARKK,\"ROKU INC\",ROKU,77543R102,\"8,941,303\",\"$527,000,398.82\",7.96%\n" +
                          "04/16/2024,ARKK,\"BLOCK INC\",SQ,852234103,\"6,171,325\",\"$453,592,387.50\",6.85%\n")
        );
        _server.Given(Request.Create().WithPath("/ARK_GENOMIC_REVOLUTION_ETF_ARKG_HOLDINGS.csv").UsingGet()
        ).RespondWith(
            Response
                .Create()
                .WithStatusCode(200)
                .WithHeader("Content-Type", "text/csv")
                .WithBody("date,fund,company,ticker,cusip,shares,\"market value ($)\",\"weight (%)\"\n" +
                          "04/16/2024,ARKG,\"TESLA INC\",TSLA,88160R101,\"4,028,071\",\"$650,452,905.08\",9.83%\n" +
                          "04/16/2024,ARKG,\"COINBASE GLOBAL INC -CLASS A\",COIN,19260Q107,\"2,630,233\",\"$587,620,354.53\",8.88%\n" +
                          "04/16/2024,ARKG,\"ROKU INC\",ROKU,77543R102,\"8,941,303\",\"$527,000,398.82\",7.96%\n" +
                          "04/16/2024,ARKG,\"BLOCK INC\",SQ,852234103,\"6,171,325\",\"$453,592,387.50\",6.85%\n")
        );
    }
    
    [TearDown]
    public void Teardown()
    {
        _server.Stop();
    }
    
    [Test]
    public async Task PerformAnalysis_Csv_Success()
    {
        // Arrange
        const string directory = "AnalysisManagerTests";
        const string fileName = "analysis";
        const string configFile = "analysis-config.json";
        
        using var client = new HttpClient();

        // This is necessary, otherwise the website will reject our request.
        client.DefaultRequestHeaders.Add("User-Agent", "Other");
        
        var dirPath = Path.Join(_projectRoot, directory);
        var configPath = Path.Join(dirPath, configFile);
        var download = ManagerCreator.CreateManager(Path.Join(dirPath, "csv"), client, ".csv");
        var manager = new AnalysisManager(
            download,
            DiffComputerCreator.CreateComputer(".csv"),
            DiffStoreCreator.CreateStore(".csv"),
            configPath);
        
        // Act
        var result = await manager.PerformAnalysis(client, ".csv", ".csv", null);
        
        // Assert
        Assert.That( result[0], Does.EndWith(".csv"));
        
        // Cleanup
        var folder = DateManipulator.GetFolderName(DateOnly.FromDateTime(DateTime.UtcNow));
        var totalPath = Path.Join(dirPath, "csv", folder);
        File.Delete(Path.Join(totalPath, fileName + ".csv"));
        Directory.Delete(totalPath);
    }
    
    [Test]
    public async Task PerformAnalysis_Html_Success()
    {
        // Arrange
        const string directory = "AnalysisManagerTests";
        const string fileName = "analysis";
        const string configFile = "analysis-config.json";
        
        using var client = new HttpClient();

        // This is necessary, otherwise the website will reject our request.
        client.DefaultRequestHeaders.Add("User-Agent", "Other");
        
        var dirPath = Path.Join(_projectRoot, directory);
        var configPath = Path.Join(dirPath, configFile);
        var download = ManagerCreator.CreateManager(Path.Join(dirPath, "html"), client, ".csv");
        var manager = new AnalysisManager(
            download,
            DiffComputerCreator.CreateComputer(".csv"),
            DiffStoreCreator.CreateStore(".html"),
            configPath);
        
        // Act
        var result = await manager.PerformAnalysis(client, ".html", ".csv", null);
        
        // Assert
        Assert.That( result[0], Does.EndWith(".html"));
        
        // Cleanup
        var folder = DateManipulator.GetFolderName(DateOnly.FromDateTime(DateTime.UtcNow));
        var totalPath = Path.Join(dirPath, "html", folder);
        File.Delete(Path.Join(totalPath, fileName + ".csv"));
        File.Delete(Path.Join(totalPath, fileName + ".html"));
        Directory.Delete(totalPath);
    }
}