using Eto.Forms;

namespace InventBox.Desktop.Interfaces;
/// <summary>
/// Represents the interface for dialog.
/// </summary>
/// <typeparam name="T">A general variable for data.</typeparam>
public interface IDialogs<T>
{
    public DynamicLayout CreateForm(T modelView);
    public Command CreateSubmitButton();
}
