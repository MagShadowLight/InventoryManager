using InventBox.Core;
using Xunit.Abstractions;
using Moq;

namespace InventBox.Test.InventBox.Core;

public class FileLoggerTest
{
    private string message = string.Empty;
    private string _path = "Test.log";
    private FileLogger? _logger;

    [Fact]
    public void WhenUserIsDoingSomethingThenLogMessageShouldBePrintedToFile()
    {
        // Arrange
        _logger = new FileLogger();
        message = "This is a log message";
        // Act
        _logger.Logs(message, _path);
        // Assert
        string result = File.ReadAllText(_path);
        Assert.Contains($"[LOG] {message}", result);
        // Clean up
        DeleteTestFile(_path);
    }
    [Fact]
    public void WhenUserIsDoingSomethingThenWarnMessageShouldBePrintedToFile()
    {
        // Arrange
        _logger = new FileLogger();
        message = "This is a warn message.";
        // Act
        _logger.Warn(message, _path);
        // Assert
        string result = File.ReadAllText(_path);
        Assert.Contains($"[WARN] {message}", result);
        // Clean up
        DeleteTestFile(_path);
    }
    [Fact]
    public void WhenUserIsDoingSomethingThenErrorMessageShouldBePrintedToFile()
    {
        // Arrange
        _logger = new FileLogger();
        message = "This is a error message";
        // Act
        _logger.Error(message, _path);
        // Assert
        string result = File.ReadAllText(_path);
        Assert.Contains($"[ERROR] {message}", result);
        // Clean up
        DeleteTestFile(_path);
    }
    [Fact]
    public void WhenUserIsDoingSomethingThenFileShouldBeThereForAdditionalDebug()
    {
        // Arrange
        _logger = new FileLogger();
        string message1 = "This is a message";
        string message2 = "This is another message";
        string message3 = "This is third message";
        string message4 = "This is fourth message";
        // Act
        _logger.Logs(message1, _path);
        _logger.Logs(message2, _path);
        _logger.Warn(message3, _path);
        _logger.Error(message4, _path);
        // Assert
        string result = File.ReadAllText(_path);
        Assert.Contains(message1, result);
        result = File.ReadAllText(_path);
        Assert.Contains(message2, result);
        result = File.ReadAllText(_path);
        Assert.Contains(message3, result);
        result = File.ReadAllText(_path);
        Assert.Contains(message4, result);
        // Clean up
        DeleteTestFile(_path);
    }

    private void DeleteTestFile(string path)
    {
        File.Delete(path);
    }
}
