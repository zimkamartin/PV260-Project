namespace StockAnalysis.Download.Getter;

public class CsvDownload : IGetter
{
    /// <summary>
    /// Downloads a csv file from the provided uri.
    /// </summary>
    /// <param name="uri">The uri from which the file will be downloaded.</param>
    /// <param name="client">The client used for the request. This method does not modify the client.</param>
    /// <returns>A stream of the downloaded contents.</returns>
    /// <exception cref="GetterException">The process of retrieving data failed.</exception>
    public async Task<Stream> Get(string uri, HttpClient client)
    {
        try
        {
            var message = await client.SendAsync(new HttpRequestMessage(HttpMethod.Get, new Uri(uri)));
            if (message.IsSuccessStatusCode)
            {
                return await message.Content.ReadAsStreamAsync();
            }

            throw new GetterException("Download failed: ["
                                      + message.StatusCode
                                      + "] "
                                      + message.ReasonPhrase);
        }
        catch (Exception e)
        {
            throw new GetterException(e.Message);
        }
    }
}