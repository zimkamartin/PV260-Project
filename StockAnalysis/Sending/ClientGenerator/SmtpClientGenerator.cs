using System.Net;
using StockAnalysis.Sending.Client;

namespace StockAnalysis.Sending.ClientGenerator;

public class SmtpClientGenerator : ISmtpClientGenerator
{
    private readonly int _port;
    private readonly string _senderAddress;
    private readonly bool _ssl;
    private readonly string _clientHost;

    /// <summary>
    /// Initializes a new instance of the <see cref="SmtpClientGenerator"/> class.
    /// </summary>
    public SmtpClientGenerator(int port, string senderAddress, bool allowSsl, string clientHost)
    {
        _port = port;
        _senderAddress = senderAddress;
        _ssl = allowSsl;
        _clientHost = clientHost;
    }

    /// <summary>
    /// Generates a client that is used to send the notification.
    /// </summary>
    public IClient GenerateClient()
    {
        var client = new SmtpClientWrapper();
        client.Host = _clientHost;
        client.Port = _port;
        var applicationPassword = Environment.GetEnvironmentVariable("PV260_EMAIL_PASSWORD") ?? "password";
        client.Credentials = new NetworkCredential(_senderAddress, applicationPassword);
        client.EnableSsl = _ssl;
        return client;
    }
}