namespace SendEmailExample;

public interface IConsoleApp
{
    Task SendMail(List<string> mailAddresses, string attachmentPath);
}