namespace StockAnalysis.Download;

public static class Download
{
    public static async Task<Stream> GetCsv(string uri, HttpClient client)
    {
        try
        {
            var message = await client.SendAsync(new HttpRequestMessage( HttpMethod.Get, new Uri( uri )));
            if (message.IsSuccessStatusCode)
            {
                return await message.Content.ReadAsStreamAsync();
            }

            throw new HttpRequestException("Download failed.");
        }
        catch (Exception)
        {
            throw;
        }
    }
}