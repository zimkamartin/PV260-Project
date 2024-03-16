using System.Net;
using System.Net.Mail;

namespace SendEmailExample;

public class SendEmailConsoleApp : IConsoleApp
{
    public Task SendMail(List<string> mailAddresses, string attachmentPath)
    {
        string fromMail = "pv260.s24.goth.pinkteam@gmail.com";
        string fromPassword = "here WAS a password";
        
        MailMessage message = new MailMessage();
        message.From = new MailAddress(fromMail);
        message.Subject = $"Diff for month {DateTime.Now.ToString("MMMM")}";
        foreach (string ma in mailAddresses)
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
        
        SmtpClient client = new SmtpClient("smtp.gmail.com")
        {
            Port = 587,
            Credentials = new NetworkCredential(fromMail, fromPassword),
            EnableSsl = true,
        };
        return client.SendMailAsync(message);
    }
}