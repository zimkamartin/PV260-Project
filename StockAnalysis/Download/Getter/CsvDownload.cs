namespace StockAnalysis.Download.Getter;

public class CsvDownload : IGetter
{
    /// <summary>
    /// Downloads a csv file from the provided uri.
    /// </summary>
    /// <param name="uri">The uri from which the file will be downloaded.</param>
    /// <param name="client">The client used for the request. This method does not modify the client.</param>
    /// <returns>A stream of the downloaded contents.</returns>
    /// <exception cref="ArgumentNullException">The formed request is null.</exception>
    /// <exception cref="InvalidOperationException">An error occured with the sending of the request.</exception>
    /// <exception cref="HttpRequestException">The request failed due to an underlying issue such as network connectivity, DNS failure, server certificate validation or timeout.</exception>
    /// <exception cref="TaskCanceledException">Failure due to timeout.</exception>
    public async Task<Stream> Get(string uri, HttpClient client)
    {
        try
        {
            var message = await client.SendAsync(new HttpRequestMessage(HttpMethod.Get, new Uri(uri)));
            if (message.IsSuccessStatusCode)
            {
                return await message.Content.ReadAsStreamAsync();
            }

            throw new HttpRequestException("Download failed: ["
                                           + message.StatusCode
                                           + "] "
                                           + message.ReasonPhrase);
        }
        catch (Exception)
        {
            throw;
        }
    }
}