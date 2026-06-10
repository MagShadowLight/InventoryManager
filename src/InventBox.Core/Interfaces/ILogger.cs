namespace InventBox.Core.Interfaces;

/// <summary>
/// Represents the interface for logger.
/// </summary>
public interface ILogger
{
    public void Logs(string message, string path = "");
    public void Warn(string message, string path = "");
    public void Error(string message, string path = "");
}
