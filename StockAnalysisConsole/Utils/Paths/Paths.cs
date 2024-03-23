namespace StockAnalysisConsole.Utils.Paths;

public static class Paths
{
    private const string ConfigFile = "Configuration.json";
    private const string ConfigFolder = "Config";
    private const string DownloadFolder = "Downloads";
    private const string DiffFolder = "Diff";
    private const string EmailsFile = "Emails.json";
    public static string GetProjectRoot()
    {
        var current = Environment.CurrentDirectory;
        var projectDirectory = Directory.GetParent(current);
        return projectDirectory is not null ? projectDirectory.Parent!.Parent!.FullName : current;
    }

    public static string GetConfigFilePath()
    {
        return Path.Combine(GetProjectRoot(), ConfigFolder, ConfigFile);
    }

    public static string GetDownloadFolderPath()
    {
        return Path.Combine(GetProjectRoot(), DownloadFolder);
    }

    public static string GetDiffFolderPath()
    {
        return Path.Combine(GetProjectRoot(), DiffFolder);
    }

    public static string GetEmailFilePath()
    {
        return Path.Combine(GetProjectRoot(), ConfigFolder, EmailsFile);
    }
}