namespace StockAnalysisTests.Utility;

public static class PathResolver
{
    public static string GetRoot()
    {
        var current = Environment.CurrentDirectory;
        return Directory.GetParent(current)!.Parent!.Parent!.FullName;
    }
    
    public static string GetTestDataPath()
    {
        return Path.Combine(GetRoot(), "TestData");;
    }
}