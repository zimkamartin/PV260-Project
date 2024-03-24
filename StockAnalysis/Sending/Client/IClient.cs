using System.Net.Mail;

namespace StockAnalysis.Sending.Client;

public interface IClient
{
    Task SendMailAsync(MailMessage mail);
}