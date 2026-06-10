using System;
using Eto.Forms;

namespace InventBox.Desktop.Interfaces;
/// <summary>
/// Represents a interface for the control.
/// </summary>
/// <typeparam name="T">First general variable for the data.</typeparam>
/// <typeparam name="K">Second general variable for the data.</typeparam>
public interface IControls<T,K>
{
    public Button AddButton(string text, int width, int height, Action eventHandler);
    public GridView CreateGrid();
    public GridColumn GetColumn(string header, Func<T, string> data);
    public TextBoxCell GetData(Func<T, string> data);
    public DynamicLayout CreateDynamicLayout();
    public TextBox CreateSearchBar();
    public K ModelViewCopy(T value);
    public ContextMenu CreateContextMenu();
    public ButtonMenuItem CreateMenuItem(string text, Action clickHandler, Keys keys = Keys.None);
}
