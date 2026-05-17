namespace InventBox.Core.Utils;

public class EnumUtils<T>
{
    public static List<T> GetEnumList<T>() {
        T[] arrayValues = (T[])Enum.GetValues(typeof(T));
        List<T> list = new List<T>(arrayValues);
        return list;
    }
}
