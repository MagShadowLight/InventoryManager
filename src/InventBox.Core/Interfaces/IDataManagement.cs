namespace InventBox.Core.Interfaces;

public interface IDataManagement<T>
{
    public T Load(string path);
    public void Save(T values, string path);
}
