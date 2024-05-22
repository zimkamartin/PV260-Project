using System.Net.Mail;
using System.Text.Json;

namespace StockAnalysis.Utilities.Email;

public static class EmailReader
{
    public static string[] ReadFromCli()
    {
        HashSet<string> addresses = new();
        while (true)
        {
            // TODO: Or press enter if you are done
            Console.WriteLine("Please input a recipient's email address or type 'done' if you are done.");
            var line = Console.ReadLine();
            if (line is "done" or null)
            {
                break;
            }

            try
            {
                // This is not very nice. TODO: Regexp instead?
                var address = new MailAddress(line);
                Console.WriteLine(!addresses.Add(address.Address)
                                    ? "Address already added. Skipping."
                                    : "Address successfully added.");
            }
            catch (Exception)
            {
                Console.WriteLine("Incorrect address format. Try again.");
            }
        }
        return addresses.ToArray();
    }

    public static async Task<string[]> ReadFromJson(string path)
    {
        try
        {
            await using var reader = File.OpenRead(path);
            var emails = await JsonSerializer.DeserializeAsync<EmailInfo>(reader);
            return emails.Emails.ToArray();
        }
        catch (Exception e) when (e is ArgumentNullException
                                      or JsonException)
        {
            return Array.Empty<string>();
        }
    }
}