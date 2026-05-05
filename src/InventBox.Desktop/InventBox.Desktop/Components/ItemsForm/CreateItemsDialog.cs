using System;
using Eto.Forms;
using Eto.Drawing;
using InventBox.Desktop.ModelView;
using InventBox.Core.Models;
using InventBox.Desktop.EventHandlers;
using System.Collections.Generic;
using InventBox.Core.Utils;
using System.Data;
using InventBox.Core;

namespace InventBox.Desktop.Components.ItemsForm
{
	public class ItemViewModel
	{
		private bool _checked { get; set; }
	}
	public partial class CreateItemsDialog : Dialog
	{
		private static FileLogger _logger;
		private static string _path;
		private ItemInputEventHandler _inputEventHandler = new ItemInputEventHandler();
		public CreateItemsDialog(ItemModelView modelView, string path, FileLogger logger)
		{
			_path = path;
			_logger = logger;
			List<Conditions> conditions = EnumUtils<Conditions>.GetEnumList<Conditions>();
			DataContext = modelView;
			List<string> conditionsItems = new List<string>();
			foreach (var condition in conditions)
			{
				conditionsItems.Add(condition.ToString());
			}
			Console.WriteLine(string.Join(",", conditionsItems));
			

			Title = "Create item";
			Size = new Size(500,500);

			var idInput = new TextBox() { Width = 200 };
			var nameInput = new TextBox() { Width = 200 };
			var descriptionInput = new TextBox() { Width = 200 };
			var quantityInput = new NumericStepper() { Width = 200 };
			var serialNoInput = new TextBox() { Width = 200 };
			var modelNoInput = new TextBox() { Width = 200 };
			var ManufacturerInput = new TextBox() { Width = 200 };
			var InsuredInput = new CheckBox()
			{
				
			};
			var noteInput = new TextBox() { Width = 200 };
			var conditionsInput = new DropDown() { 
				DataStore = conditionsItems,
				Width = 200 
			};
			

			idInput.TextBinding.BindDataContext((ItemModelView item) => item.Id.ToString());
			nameInput.BindDataContext(t => t.Text, (ItemModelView items) => items.Name);
			descriptionInput.BindDataContext(t => t.Text, (ItemModelView items) => items.Description);
			quantityInput.ValueBinding.BindDataContext((ItemModelView items) => items.Quantity);
			serialNoInput.BindDataContext(t => t.Text, (ItemModelView items) => items.SerialNumber);
			modelNoInput.BindDataContext(t => t.Text, (ItemModelView items) => items.ModelNumber);
			ManufacturerInput.BindDataContext(t => t.Text, (ItemModelView items) => items.Manufacturer);
			InsuredInput.CheckedBinding.BindDataContext(Binding.Property((ItemModelView items) => items.Insured).ToBool(true, false));
			noteInput.BindDataContext(t => t.Text, (ItemModelView items) => items.Notes);
			conditionsInput.BindDataContext(dd => dd.SelectedIndex, (ItemModelView items) => Convert.ToInt32(items.Conditions));
            // _inputEventHandler.ChangeIdValue(idInput, (ItemModelView)modelView);
            // _inputEventHandler.ChangeNameValue(nameInput, (ItemModelView)modelView);
            // _inputEventHandler.ChangeDescriptionValue(descriptionInput, (ItemModelView)modelView);
            // _inputEventHandler.ChangeQuantityValue(quantityInput, (ItemModelView)modelView);
            // _inputEventHandler.ChangeSNValue(serialNoInput, (ItemModelView)modelView);
            // _inputEventHandler.ChangeMNValue(modelNoInput, (ItemModelView)modelView);
            // _inputEventHandler.ChangeManufacturerValue(ManufacturerInput, (ItemModelView)modelView);
            // _inputEventHandler.ChangeInsuredValue(InsuredInput, (ItemModelView)modelView);
            // _inputEventHandler.ChangeNotesValue(noteInput, (ItemModelView)modelView);
            // _inputEventHandler.ChangeConditionsValue(conditionsInput, (ItemModelView)modelView);
			var SubmitButton = CreateSubmitButton(modelView);
			


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
			form.Add(
				new Button() {Command = SubmitButton, Size = new Size(100,50), Text = "Submit"}
			);
			form.EndVertical();
			// WriteLog(nameInput);
			Content = form;

		}

        private Items CreateItem(ItemModelView modelView)
        {
			var model = (ItemModelView)DataContext;
			var item = (Items)model;
			return item;
        }

        public void WriteLog(TextBox textBox)
		{
			textBox.TextChanging += (sender, e) => Console.WriteLine($"Id: {textBox.Text}");
			textBox.TextChanged += (sender, e) => Console.WriteLine($"Id: {textBox.Text}");
		}
		private void WriteItem(Items items)
		{
			Console.WriteLine(items.Id.ToString());
			Console.WriteLine(items.Name);
			Console.WriteLine(items.Description);
			Console.WriteLine(items.Quantity.ToString());
			Console.WriteLine(items.SerialNumber);
			Console.WriteLine(items.ModelNumber);
			Console.WriteLine(items.Manufacturer);
			Console.WriteLine(items.Insured.ToString());
			Console.WriteLine(items.Notes);
			Console.WriteLine(items.CreatedAt);
			Console.WriteLine(items.UpdatedAt);
			Console.WriteLine(items.Conditions);
		}
		private Command CreateSubmitButton(ItemModelView model)
		{
			var button = new Command();
			button.Executed += (sender, e) =>
			{
				
				Items item = CreateItem(model);
				// WriteItem(item);
				ModelsList.items.Add(item);
				for (int i = 0; i < ModelsList.items.Count; i++)
				{
					var x = ModelsList.items[i];
					var hash = System.Runtime.CompilerServices.RuntimeHelpers.GetHashCode(x);
					Console.WriteLine($"  [{i}] type={x.GetType().Name} hash={hash} name='{x.Name}'");
				}
				if (this != null)
					this.Close();
			};
			return button;
		}
	}
}
