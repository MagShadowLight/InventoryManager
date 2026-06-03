using System.Text.Json;
using InventBox.Core.Models;

namespace InventBox.Core;

public class JsonParser<T>
{
    public string ParseJson(T value)
    {
        var json = JsonSerializer.Serialize(value, new JsonSerializerOptions
        {
            WriteIndented = true
        });
        return json;
    }
    public T? DeParseJson(string json)
    {
        var value = JsonSerializer.Deserialize<T>(json);
        return value;
    }
}
