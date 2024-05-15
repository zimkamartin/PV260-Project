namespace StockAnalysisTests.DiffTests;

public static class PathResolver
{
    public static string GetTestDataPath()
    {
        var current = Environment.CurrentDirectory;
        var projectDirectory = Directory.GetParent(current);
        var testDataPath = current;
        if (projectDirectory is not null)
        {
            testDataPath = Path.Combine(projectDirectory.Parent!.Parent!.FullName, "TestData");
        }

        return testDataPath;
    }
}