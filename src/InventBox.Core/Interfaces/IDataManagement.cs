namespace InventBox.Core.Interfaces;

/// <summary>
/// Represents the interface for Data management.
/// </summary>
/// <typeparam name="T"></typeparam>
public interface IDataManagement<T>
{
    public T Load(string path, bool isItem = false);
    public void Save(T values, string path);
}
