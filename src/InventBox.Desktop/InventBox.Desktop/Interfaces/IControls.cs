using System;
using Eto.Forms;

namespace InventBox.Desktop.Interfaces;

public interface IControls<T>
{
    public Button AddButton(string text, int width, int height, Action eventHandler);
    public GridColumn GetColumn(string header, Func<T, string> data);
    public TextBoxCell GetData(Func<T, string> data);
}
