using Eto.Forms;

namespace InventBox.Desktop.Interfaces;

public interface IDialogs<T>
{
    public DynamicLayout CreateForm(T modelView);
    public Command CreateSubmitButton();
}
