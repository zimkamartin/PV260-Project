namespace StockAnalysis.Download.Getter;

public interface IGetter
{
    /// <summary>
    /// Retrieves a resource from a given uri, returning it as a stream.
    /// </summary>
    /// <param name="uri">The uri for a resource request.</param>
    /// <param name="client">An HttpClient that makes the request. Must be provided and managed by the application.</param>
    /// <returns>A stream of the resource's contents.</returns>
    Task<Stream> Get(string uri, HttpClient client);
}