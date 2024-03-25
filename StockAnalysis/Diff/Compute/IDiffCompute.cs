using StockAnalysis.Diff.Data;

namespace StockAnalysis.Diff.Compute;

public interface IDiffCompute
{
    /// <summary>
    /// Calculates a diff between a new and old holding entry.
    /// </summary>
    /// <param name="newPath">The path to the new holding entry.</param>
    /// <param name="oldPath">The path to the old holding entry.</param>
    /// <returns>A collection that represents the output of the diff analysis.</returns>
    /// <exception cref="DiffComputeException">On irrecoverable diff analysis failure.</exception>
    public IEnumerable<DiffData> CreateDiff(string newPath, string? oldPath);
}