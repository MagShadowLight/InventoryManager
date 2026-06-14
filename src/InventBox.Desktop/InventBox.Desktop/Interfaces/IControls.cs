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
    public GridView CreateGrid();
    public DynamicLayout CreateDynamicLayout();
    public TextBox CreateSearchBar();
    public K ModelViewCopy(T value);
    public ContextMenu CreateContextMenu();
}
