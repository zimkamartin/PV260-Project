using StockAnalysis.Sending.Client;

namespace StockAnalysis.Sending.ClientGenerator;

/// <summary>
/// A generator that should provides a client for the sender.
/// </summary>
public interface ISmtpClientGenerator
{
    /// <summary>
    /// Generates a client that is used to send the notification.
    /// </summary>
    /// <returns>A client that is used to send the notification.</returns>
    IClient GenerateClient();
}