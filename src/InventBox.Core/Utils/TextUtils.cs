namespace InventBox.Core.Utils;

/// <summary>
/// Represents a utility for text.
/// </summary>
public class TextUtils
{
    /// <summary>
    /// Create a new line of the message.
    /// </summary>
    /// <param name="message">The text of the message.</param>
    /// <returns>The message with a new line.</returns>
    public static string CreateNewLine(string message)
    {
        return message + "\n";
    }
}
