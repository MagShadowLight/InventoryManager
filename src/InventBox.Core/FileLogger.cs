using System.Text;
using InventBox.Core.Interfaces;
using InventBox.Core.Utils;

namespace InventBox.Core;

/// <summary>
/// Represents the logger for the file stream.
/// </summary>
public class FileLogger : ILogger
{
    private FileStream _stream;
    int fileLength;
    /// <summary>
    /// Print the log message into the file.
    /// </summary>
    /// <param name="message">The log message.</param>
    /// <param name="path">The path to be place for logging purpose.</param>
    public void Logs(string message, string path = "")
    {
        message = TextUtils.CreateNewLine(message);
        if (!File.Exists(path)) {
            _stream = File.Create(path);
            _stream.Close();
        }
        fileLength = File.ReadAllText(path).Length;
        _stream = File.OpenWrite(path);
        if (fileLength > 0)
            _stream.Position = fileLength + 1;
        WriteText(_stream, $"[LOG] {message}\n");
        _stream.Close();
    }
    /// <summary>
    /// Print the warn message into the file.
    /// </summary>
    /// <param name="message">The warn message.</param>
    /// <param name="path">The path of log for logging purpose.</param>
    public void Warn(string message, string path = "")
    {
        message = TextUtils.CreateNewLine(message);
        if (!File.Exists(path)) {
            _stream = File.Create(path);
            _stream.Close();
        }
        fileLength = File.ReadAllText(path).Length;
        _stream = File.OpenWrite(path);
        if (fileLength > 0)
            _stream.Position = fileLength + 1;
        WriteText(_stream, $"[WARN] {message}");
        _stream.Close();
    }
    /// <summary>
    /// Print the error message into the file.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <param name="path">The path for message place into the file.</param>
    public void Error(string message, string path = "")
    {
        message = TextUtils.CreateNewLine(message);
        if (!File.Exists(path)) {
            _stream = File.Create(path);
            _stream.Close();
        }
        fileLength = File.ReadAllText(path).Length;
        _stream = File.OpenWrite(path);
        if (fileLength > 0)
            _stream.Position = fileLength + 1;
        WriteText(_stream, $"[ERROR] {message}");
        _stream.Close();
    }
    /// <summary>
    /// Convert the message to byte array and write it to the file
    /// </summary>
    /// <param name="stream">The stream for the file.</param>
    /// <param name="message">The message to be converted into byte array.</param>
    private static void WriteText(FileStream stream, string message)
    {
        byte[] bytes = new UTF8Encoding(true).GetBytes(message);
        stream.Write(bytes, 0, bytes.Length);
    }
}
