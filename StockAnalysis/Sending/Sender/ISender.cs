namespace StockAnalysis.Sending.Sender;

public interface ISender
{
    /// <summary>
    /// Sends a notification 
    /// </summary>
    /// <param name="recipients">A collection of recipients.</param>
    /// <param name="attachmentPaths">A collection of necessary attachments (diff files).</param>
    /// <returns></returns>
    /// <exception cref="SenderException">Thrown when the notification sending process fails.</exception>
    Task SendNotification(IEnumerable<string> recipients, IEnumerable<string> attachmentPaths);
}