using InventBox.Core;
using Xunit.Abstractions;
using Moq;

namespace InventBox.Test.InventBox.Core;

public class ConsoleLoggerTest 
{
    private Mock<TextWriter> consoleOutput;
    private readonly ITestOutputHelper output;
    private ConsoleLogger _logger;
    private string message;

    public ConsoleLoggerTest(ITestOutputHelper output)
    {
        this.output = output;
        consoleOutput = new Mock<TextWriter>();

        consoleOutput.Setup(c => c.WriteLine(It.IsAny<string>()))
            .Callback<string>(s => output.WriteLine(s));

        Console.SetOut(consoleOutput.Object);
    }

    [Fact]
    public void WhenUserIsDoingSomethingThenConsoleShouldPrintLogsMessages()
    {
        // Arrange
        _logger = new ConsoleLogger();
        message = "This is a Log message";
        // Act
        _logger.Logs(message);
        // Assert
        consoleOutput.Verify(c => c.WriteLine($"[LOG] {message}"), Times.Once);
        output.WriteLine("message should print in logs");
    }
    [Fact]
    public void WhenUserIsDoingSomethingThenConsoleShouldPrintWarningMessage()
    {
        // Arrange
        _logger = new ConsoleLogger();
        message = "This is a warning message"; 
        // Act
        _logger.Warn(message);
        // Assert
        consoleOutput.Verify(c => c.WriteLine($"[WARN] {message}"), Times.Once);
        output.WriteLine("message should print in warning");
    }
    [Fact]
    public void WhenUserIsDoingSomethingThenConsoleShouldPrintErrorMessage()
    {
        // Arrange
        _logger = new ConsoleLogger();
        message = "This is a error message";
        // Act
        _logger.Error(message);
        // Assert
        consoleOutput.Verify(c => c.WriteLine($"[ERROR] {message}"), Times.Once);
        output.WriteLine("message should print in error");
    }
}
