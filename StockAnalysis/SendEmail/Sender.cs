using System.Net;
using System.Net.Mail;
namespace StockAnalysis.SendEmail;

public static class Sender
{
    public static async Task SendMail(List<string> mailAddresses, string attachmentPath)
    {
        const string fromMail = "pv260.s24.goth.pinkteam@gmail.com";
        // App password.
        var fromPassword = Environment.GetEnvironmentVariable("PV260-email-password") ?? "password";  // TODO: do it using .env file
        
        var message = new MailMessage();
        message.From = new MailAddress(fromMail);
        message.Subject = $"Diff for month {DateTime.Now:MMMM}";
        foreach (var ma in mailAddresses)
        {
            message.Bcc.Add(new MailAddress(ma));
        }
        message.Body = "Good morning Madam/Sir,\nHere is your monthly diff.\n\nHave a nice day!\nSincerely Pink team";
        try
        {
            message.Attachments.Add(new Attachment(attachmentPath));
        }
        catch (Exception e)
        {
            switch (e)
            {
                case DirectoryNotFoundException or FileNotFoundException:
                    Console.WriteLine("Directory or file does not exist");
                    throw;
                case UnauthorizedAccessException:
                    Console.WriteLine("Does not have access to the folder or file.");
                    throw;
                default:
                    throw;
            }
        }
        
        try
        {
            using var client = new SmtpClient("smtp.gmail.com");
            client.Port = 587;
            client.Credentials = new NetworkCredential(fromMail, fromPassword);
            client.EnableSsl = true;

            await client.SendMailAsync(message);
        }
        catch (SmtpException)
        {
            Console.WriteLine("There was an Smtp exception, most probably the password was set wrongly.");
            throw;
        }
    }
}