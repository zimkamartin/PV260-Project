using StockAnalysis.Diff.Load;

namespace StockAnalysis.Diff.Compute;

public static class DiffComputerCreator
{
    public static IDiffCompute CreateComputer(string extension)
    {
        switch (extension.Replace(".", "").ToLower())
        {
            case "csv":
                return new CsvDiffComputer(new CsvHoldingLoader());
            default:
                throw new NotImplementedException("Chosen extension is currently not supported.");
        }
    }
}