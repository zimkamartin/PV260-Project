using System.Net.Mail;
using StockAnalysis.Sending.Client;
using StockAnalysis.Sending.ClientGenerator;
using StockAnalysis.Sending.Sender;

namespace StockAnalysisTests.SendEmailTests;

public class MessageRepository
{
    public MailMessage Message { get; set; } = null!;
}

public class MockClient : IClient
{
    private readonly MessageRepository _messageRepository;

    public MockClient(MessageRepository messageRepository)
    {
        _messageRepository = messageRepository;
    }

    public Task SendMailAsync(MailMessage mail)
    {
        _messageRepository.Message = mail;
        return Task.CompletedTask;
    }
}

public class MockSenderGenerator : ISmtpClientGenerator
{
    private readonly MessageRepository _messageRepository;

    public MockSenderGenerator(MessageRepository messageRepository)
    {
        _messageRepository = messageRepository;
    }

    public IClient GenerateClient()
    {
        return new MockClient(_messageRepository);
    }
}

public class SenderTests
{
    private const string Address = "test@test.com";

    [Test]
    public async Task Sender_SendNotification_ValidBody()
    {
        // Arrange
        var repository = new MessageRepository();
        var generator = new MockSenderGenerator(repository);
        var sender = new Sender(Address, generator);

        // Act
        await sender.SendNotification(new[] { Address }, new List<string>());

        // Assert
        Assert.That(repository.Message.Body,
            Is.EqualTo("Good morning Madam/Sir,\nHere is your monthly diff.\n\nHave a nice day!\nSincerely Pink team"));
    }

    [Test]
    public async Task Sender_SendNotification_ValidFrom()
    {
        // Arrange
        var repository = new MessageRepository();
        var testMail = new MailAddress(Address);
        var generator = new MockSenderGenerator(repository);
        var sender = new Sender(Address, generator);

        // Act
        await sender.SendNotification(new[] { Address }, new List<string>());

        // Assert
        Assert.That(repository.Message.From, Is.EqualTo(testMail));
    }

    [Test]
    public async Task Sender_SendNotification_ValidReceiver()
    {
        // Arrange
        var repository = new MessageRepository();
        var testMail = new MailAddress(Address);
        var generator = new MockSenderGenerator(repository);
        var sender = new Sender(Address, generator);

        // Act
        await sender.SendNotification(new[] { Address }, new List<string>());

        // Assert
        Assert.That(repository.Message.Bcc, Contains.Item(testMail));
    }
}