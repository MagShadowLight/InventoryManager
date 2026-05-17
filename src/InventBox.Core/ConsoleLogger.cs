using InventBox.Core.Interfaces;

namespace InventBox.Core;

public class ConsoleLogger : ILogger
{
    // Print the log message to the console
    public void Logs(string message, string path = "")
    {
        Console.WriteLine($"[LOG] {message}");
    }
    // Print the warning message to the console
    public void Warn(string message, string path = "")
    {
        Console.WriteLine($"[WARN] {message}");
    }
    // Print the error message to the console
    public void Error(string message, string path = "")
    {
        Console.WriteLine($"[ERROR] {message}");
    }
}
