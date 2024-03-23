namespace StockAnalysis.Sending.Sender;

public interface ISender
{
    /// <summary>
    /// Sends a notification 
    /// </summary>
    /// <param name="recipients">A collection of recipients.</param>
    /// <param name="attachmentPaths">A collection of necessary attachments (diff files).</param>
    /// <returns></returns>
    Task SendNotification(IEnumerable<string> recipients, IEnumerable<string> attachmentPaths);
}