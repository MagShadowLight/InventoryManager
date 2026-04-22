namespace InventBox.Core.Interfaces;

public interface IDataTransfer
{
    public void Import(string path);
    public void Export(string[] text, string path);
}
