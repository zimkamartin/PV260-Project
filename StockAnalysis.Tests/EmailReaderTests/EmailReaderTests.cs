using StockAnalysis.Utilities.Email;
using StockAnalysisTests.Utility;

namespace StockAnalysisTests.EmailReaderTests;

public class EmailReaderTests
{
    private const string StoreDirectory = "Mocks";
    
    [Test]
    public async Task ReadFromJson_EmptyFile_ReturnsEmpty()
    {
        // Arrange
        const string file = "empty_email.json";

        // Act
        var result = await EmailReader.ReadFromJson(Path.Combine(PathResolver.GetRoot(), StoreDirectory, file));

        // Assert
        Assert.That(result, Is.Empty);
    }

    [Test]
    public async Task ReadFromJson_ValidFile_ReturnParsedContents()
    {
        // Arrange
        const string file = "email.json";
        var expected = new[] { "test@test.com" };

        // Act
        var result = await EmailReader.ReadFromJson(Path.Combine(PathResolver.GetRoot(), StoreDirectory, file));

        // Assert
        Assert.That(result, Is.EqualTo(expected));
    }

    [Test]
    public async Task ReadFromJson_InvalidFile_ReturnsEmpty()
    {
        // Arrange
        const string file = "email_bad.json";

        // Act
        var result = await EmailReader.ReadFromJson(Path.Combine(PathResolver.GetRoot(), StoreDirectory, file));

        // Assert
        Assert.That(result, Is.Empty);
    }

    [Test]
    public async Task ReadFromJson_MissingFile_ThrowsException()
    {
        // Arrange
        const string file = "email_not_exists.json";

        // Act
        try
        {
            await EmailReader.ReadFromJson(Path.Combine(PathResolver.GetRoot(), StoreDirectory, file));
            Assert.Fail("EmailReader did not throw an exception.");
        }
        catch (Exception)
        {
            Assert.Pass();
        }
    }

    [Test]
    public void ReadFromCli_ValidEmail_ReturnsEmail()
    {
        // Arrange
        const string email = "test@test.com";
        var expected = new[] { email };
        var actual = Array.Empty<string>();

        // Act
        SubstituteStdIn(email, () => { actual = EmailReader.ReadFromCli(); });

        // Assert
        Assert.That(expected, Is.EqualTo(actual));
    }

    [Test]
    public void ReadFromCli_InvalidEmail_ReturnsEmpty()
    {
        // Arrange
        const string email = "test.com";
        var expected = Array.Empty<string>();
        var actual = Array.Empty<string>();

        // Act
        SubstituteStdIn(email, () => { actual = EmailReader.ReadFromCli(); });

        // Assert
        Assert.That(expected, Is.EqualTo(actual));
    }

    [Test]
    public void ReadFromCli_OneValidOneInvalidEmail_ReturnsValid()
    {
        // Arrange
        const string input = "test.com\ntest@test.com";
        var expected = new[] { "test@test.com" };
        var actual = Array.Empty<string>();

        // Act
        SubstituteStdIn(input, () => { actual = EmailReader.ReadFromCli(); });

        // Assert
        Assert.That(expected, Is.EqualTo(actual));
    }

    // Taken from https://stackoverflow.com/a/75457111
    private static void SubstituteStdIn(string content, Action callback)
    {
        var originalStdIn = Console.In;

        using var newStdIn = new StringReader(content);
        Console.SetIn(newStdIn);

        callback.Invoke();

        Console.SetIn(originalStdIn);
    }
}