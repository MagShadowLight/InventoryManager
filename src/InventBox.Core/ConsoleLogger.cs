using InventBox.Core.Interfaces;

namespace InventBox.Core;

/// <summary>
/// Represents the logger for the console.
/// </summary>
public class ConsoleLogger : ILogger
{
    /// <summary>
    /// Print the log message to the console.
    /// </summary>
    /// <param name="message">The message for the logging.</param>
    /// <param name="path">The path for logging purpose.</param>
    public void Logs(string message, string path = "")
    {
        Console.WriteLine($"[LOG] {message}");
    }
    /// <summary>
    /// Print the warning message to the console
    /// </summary>
    /// <param name="message">The message for the warning.</param>
    /// <param name="path">The path for logging purpose.</param>
    public void Warn(string message, string path = "")
    {
        Console.WriteLine($"[WARN] {message}");
    }
    /// <summary>
    /// Print the error message to the console.
    /// </summary>
    /// <param name="message">The message for the error.</param>
    /// <param name="path">The path for logging purpose.</param>
    public void Error(string message, string path = "")
    {
        Console.WriteLine($"[ERROR] {message}");
    }
}
