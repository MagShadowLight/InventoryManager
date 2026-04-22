namespace InventBox.Core.Interfaces;

public interface ILogger
{
    public void Logs(string message, string path = "");
    public void Warn(string message, string path = "");
    public void Error(string message, string path = "");
}
