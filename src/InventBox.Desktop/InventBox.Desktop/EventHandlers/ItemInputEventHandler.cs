using System;
using Eto.Forms;
using InventBox.Core.Models;
using InventBox.Desktop.ModelView;

namespace InventBox.Desktop.EventHandlers;

public class ItemInputEventHandler
{
    public void ChangeIdValue(TextBox textBox, ItemModelView modelView)
    {
        textBox.TextChanged += (sender, e) => {
            if (int.TryParse(textBox.Text, out int id))
            modelView.Id = id;
        };
    }
    public void ChangeNameValue(TextBox textBox, ItemModelView modelView)
    {
        textBox.TextChanged += (sender, e) => {
            modelView.Name = textBox.Text;
        };
    }    public void ChangeDescriptionValue(TextBox textBox, ItemModelView modelView)
    {
        textBox.TextChanged += (sender, e) => {
            modelView.Description = textBox.Text;
        };
    }    public void ChangeQuantityValue(TextBox textBox, ItemModelView modelView)
    {
        textBox.TextChanged += (sender, e) => {
            if (int.TryParse(textBox.Text, out int quantity))
            modelView.Quantity = quantity;
        };
    }    public void ChangeSNValue(TextBox textBox, ItemModelView modelView)
    {
        textBox.TextChanged += (sender, e) => modelView.SerialNumber = textBox.Text;
    }    public void ChangeMNValue(TextBox textBox, ItemModelView modelView)
    {
        textBox.TextChanged += (sender, e) => modelView.ModelNumber = textBox.Text;
    }    public void ChangeManufacturerValue(TextBox textBox, ItemModelView modelView)
    {
        textBox.TextChanged += (sender, e) => modelView.Manufacturer = textBox.Text;
    }    public void ChangeNotesValue(TextBox textBox, ItemModelView modelView)
    {
        textBox.TextChanged += (sender, e) => modelView.Notes = textBox.Text;
    }

    public void ChangeInsuredValue(CheckBox checkBox, ItemModelView modelView)
    {
        checkBox.CheckedChanged += delegate
        {
            modelView.Insured = checkBox.Checked.Value;
        };
    }
    public void ChangeConditionsValue(DropDown dropDown, ItemModelView modelView)
    {
        dropDown.SelectedIndexChanged += (sender, e) =>
        {
            modelView.Conditions = Enum.Parse<Conditions>(dropDown.SelectedKey);
        };
    } 
}
