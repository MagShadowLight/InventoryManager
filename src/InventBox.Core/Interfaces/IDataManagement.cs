namespace InventBox.Core.Interfaces;

public interface IDataManagement<T>
{
    public T Load(string path, bool isItem = false);
    public void Save(T values, string path);
}
