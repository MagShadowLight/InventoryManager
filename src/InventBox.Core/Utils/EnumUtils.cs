namespace InventBox.Core.Utils;

/// <summary>
/// Represents the utility for enum.
/// </summary>
/// <typeparam name="T">The general variable for enum.</typeparam>
public class EnumUtils<T>
{
    /// <summary>
    /// Get a value from the enum and add it to the list.
    /// </summary>
    /// <typeparam name="T">The variable for enum.</typeparam>
    /// <returns>The list from the enum values.</returns>
    public static List<T> GetEnumList<T>() {
        T[] arrayValues = (T[])Enum.GetValues(typeof(T));
        List<T> list = new List<T>(arrayValues);
        return list;
    }
}
