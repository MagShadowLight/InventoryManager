using System;
using Eto.Forms;
using Eto.Drawing;
using InventBox.Desktop.Models;
using InventBox.Core.Models;

namespace InventBox.Desktop.Components.ItemsForm
{
	public class ItemViewModel
	{
		private bool _checked { get; set; }
	}
	public partial class CreateItemsDialog : Dialog
	{
		public CreateItemsDialog()
		{
			string[] conditionsItems = new []
			{
				Conditions.Excellent.ToString(),
				Conditions.Great.ToString(),
				Conditions.Good.ToString(),
				Conditions.Ok.ToString(),
				Conditions.Acceptable.ToString(),
				Conditions.Broken.ToString(),
				Conditions.Excellent.ToString()
			};
			
			Title = "Create item";
			Size = new Size(500,500);
			Items items = new Items();
			var idInput = new TextBox() { Width = 200 };
			var nameInput = new TextBox() { Width = 200 };
			var descriptionInput = new TextBox() { Width = 200 };
			var quantityInput = new TextBox() { Width = 200 };
			var serialNoInput = new TextBox() { Width = 200 };
			var modelNoInput = new TextBox() { Width = 200 };
			var ManufacturerInput = new TextBox() { Width = 200 };
			var InsuredInput = new CheckBox() {Checked = false};
			var noteInput = new TextBox() { Width = 200 };
			var conditionsInput = new DropDown() { 
				DataStore = conditionsItems,
				Width = 200 
			};

			idInput.BindDataContext(t => t.Text, (Items item) => item.Id.ToString());
			nameInput.BindDataContext(t => t.Text, (Items items) => items.Name);
			descriptionInput.BindDataContext(t => t.Text, (Items items) => items.Description);
			quantityInput.BindDataContext(t => t.Text, (Items items) => items.Quantity.ToString());
			serialNoInput.BindDataContext(t => t.Text, (Items items) => items.SerialNumber);
			modelNoInput.BindDataContext(t => t.Text, (Items items) => items.ModelNumber);
			ManufacturerInput.BindDataContext(t => t.Text, (Items items) => items.Manufacturer);
			InsuredInput.CheckedBinding.BindDataContext(Binding.Property((Items items) => items.Insured).ToBool(true, false));
			noteInput.BindDataContext(t => t.Text, (Items items) => items.Notes);
			conditionsInput.BindDataContext(dd => dd.SelectedIndex, (Items items) => Convert.ToInt32(items.Conditions));
			var SubmitButton = new Command();
			SubmitButton.Executed += (sender, e) =>
			{
				
			};


			var form = new DynamicLayout
			{
				Padding = 10,

			};
			form.BeginVertical();
			form.AddRow(
				"Id",
				idInput
			);
			form.AddRow(
				"Name",
				nameInput
			);
			form.AddRow(
				"Description",
				descriptionInput
			);
			form.AddRow(
				"Quantity",
				quantityInput
			);
			form.AddRow(
				"Serial Number",
				serialNoInput
			);
			form.AddRow(
				"Model Number",
				modelNoInput
			);
			form.AddRow(
				"Manufacturer",
				ManufacturerInput
			);
			form.AddRow(
				"Insured?",
				InsuredInput
			);
			form.AddRow(
				"Note",
				noteInput
			);
			form.AddRow(
				"Condition",
				conditionsInput
			);
			form.AddRow(
				new Button() {Command = SubmitButton}
			);
			form.EndVertical();



			Content = form;
		}
	}
}
