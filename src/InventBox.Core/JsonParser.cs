using System.Text.Json;

namespace InventBox.Core;

/// <summary>
/// Represents the json parser operation.
/// </summary>
/// <typeparam name="T">General object for json.</typeparam>
public class JsonParser<T>
{
    private FileLogger _logger;
    private string _path;
    /// <summary>
    /// Initialize a new instance of json parser.
    /// </summary>
    /// <param name="logger">File logger for logging purpose.</param>
    /// <param name="path">Path for logger.</param>
    public JsonParser(FileLogger logger, string path)
    {
        _logger = logger;
        _path = path;
    }
    /// <summary>
    /// Parse the object into a json string.
    /// </summary>
    /// <param name="value">The object data.</param>
    /// <returns>A json string with the object data.</returns>
    public string ParseJson(T value)
    {
        var json = JsonSerializer.Serialize(value, new JsonSerializerOptions
        {
            WriteIndented = true
        });
        return json;
    }
    /// <summary>
    /// Convert the json string into the object.
    /// </summary>
    /// <param name="json">the json string with object.</param>
    /// <returns>The object from the json.</returns>
    public T? DeParseJson(string json)
    {
        var value = JsonSerializer.Deserialize<T>(json);
        return value;
    }
}
