namespace InventBox.Core.Interfaces;

public interface IDataTransfer<T>
{
    public T Import(string path);
    public void Export(T values, string path);
}
