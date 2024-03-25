using System.Net.Mail;
using StockAnalysis.Sending.ClientGenerator;

namespace StockAnalysis.Sending.Sender;

public class Sender : ISender
{
    private readonly ISmtpClientGenerator _generator;
    private readonly MailAddress _senderAddress;

    public Sender(string senderMail, ISmtpClientGenerator generator)
    {
        _senderAddress = new MailAddress(senderMail);
        _generator = generator;
    }

    private MailMessage ComposeMessage(IEnumerable<string> recipients, IEnumerable<string> attachmentPaths)
    {
        var message = new MailMessage();
        message.From = _senderAddress;
        message.Subject = $"Diff for month {DateTime.Now:MMMM}";
        foreach (var recipient in recipients)
        {
            message.Bcc.Add(new MailAddress(recipient));
        }

        message.Body = "Good morning Madam/Sir,\nHere is your monthly diff.\n\nHave a nice day!\nSincerely Pink team";
        try
        {
            foreach (var ap in attachmentPaths)
            {
                message.Attachments.Add(new Attachment(ap));
            }
        }
        catch (Exception e)
        {
            throw new SenderException(e.Message);
        }

        return message;
    }

    public async Task SendNotification(IEnumerable<string> recipients, IEnumerable<string> attachmentPaths)
    {
        MailMessage message;
        try
        {
            message = ComposeMessage(recipients, attachmentPaths);
        }
        catch (Exception e)
        {
            throw new SenderException(e.Message);
        }

        var client = _generator.GenerateClient();
        try
        {
            await client.SendMailAsync(message);
        }
        catch (SmtpException e)
        {
            throw new SenderException(e.Message);
        }
    }
}