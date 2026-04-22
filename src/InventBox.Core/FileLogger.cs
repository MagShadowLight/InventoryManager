using System.Text;
using InventBox.Core.Interfaces;
using InventBox.Core.Utils;

namespace InventBox.Core;

public class FileLogger : ILogger
{
    private FileStream _stream;
    int fileLength;

    // Print the log message to the file.
    public void Logs(string message, string path = "")
    {
        message = TextUtils.NewLine(message);
        if (!File.Exists(path)) {
            _stream = File.Create(path);
            _stream.Close();
        }
        fileLength = File.ReadAllText(path).Length;
        _stream = File.OpenWrite(path);
        if (fileLength > 0)
            _stream.Position = fileLength;
        WriteText(_stream, $"[LOG] {message}");
        _stream.Close();
    }
    // Print the warning message to the file.
    public void Warn(string message, string path = "")
    {
        message = TextUtils.NewLine(message);
        if (!File.Exists(path)) {
            _stream = File.Create(path);
            _stream.Close();
        }
        fileLength = File.ReadAllText(path).Length;
        _stream = File.OpenWrite(path);
        if (fileLength > 0)
            _stream.Position = fileLength;
        WriteText(_stream, $"[WARN] {message}");
        _stream.Close();
    }
    // Print the error message to the file.
    public void Error(string message, string path = "")
    {
        message = TextUtils.NewLine(message);
        if (!File.Exists(path)) {
            _stream = File.Create(path);
            _stream.Close();
        }
        fileLength = File.ReadAllText(path).Length;
        _stream = File.OpenWrite(path);
        if (fileLength > 0)
            _stream.Position = fileLength;
        WriteText(_stream, $"[ERROR] {message}");
        _stream.Close();
    }
    // Convert the message to byte array and write it to the file using those byte array.
    private static void WriteText(FileStream stream, string message)
    {
        byte[] bytes = new UTF8Encoding(true).GetBytes(message);
        stream.Write(bytes, 0, bytes.Length);
    }
}
