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
	public enum Mode
	{
		Create, 
		Edit
	}
	public partial class ItemsDialog : Dialog
	{
		private static FileLogger _logger;
		private static string _path;
		private readonly Mode _mode;
		private readonly Action<Items> _onSubmit;
		public ItemsDialog(ItemModelView modelView, Mode mode, Action<Items> onSubmitEvent, string path, FileLogger logger)
		{
			_path = path;
			_logger = logger;
			_mode = mode;
			_onSubmit = onSubmitEvent;
			DataContext = modelView;
			Title = _mode == Mode.Create ? "Create item" : "Edit item" ;
			Size = new Size(500,350);
			var form = CreateForm(modelView);
			Content = form;
		}

		DynamicLayout CreateForm(ItemModelView modelView)
		{
			// Create Inputs
			var nameInput = new TextBox() { Width = 200 };
			var descriptionInput = new TextBox() { Width = 200 };
			var quantityInput = new NumericStepper() { Width = 200 };
			var serialNoInput = new TextBox() { Width = 200 };
			var modelNoInput = new TextBox() { Width = 200 };
			var ManufacturerInput = new TextBox() { Width = 200 };
			var InsuredInput = new CheckBox();
			var noteInput = new TextBox() { Width = 200 };
			var conditionsInput = new EnumDropDown<Conditions>() { 
				Width = 200 
			};			
			// Bind those input to data
			nameInput.BindDataContext(t => t.Text, (ItemModelView items) => items.Name);
			descriptionInput.BindDataContext(t => t.Text, (ItemModelView items) => items.Description);
			quantityInput.ValueBinding.BindDataContext((ItemModelView items) => items.Quantity);
			serialNoInput.BindDataContext(t => t.Text, (ItemModelView items) => items.SerialNumber);
			modelNoInput.BindDataContext(t => t.Text, (ItemModelView items) => items.ModelNumber);
			ManufacturerInput.BindDataContext(t => t.Text, (ItemModelView items) => items.Manufacturer);
			InsuredInput.CheckedBinding.BindDataContext(Binding.Property((ItemModelView items) => items.Insured).ToBool(true, false));
			noteInput.BindDataContext(t => t.Text, (ItemModelView items) => items.Notes);
			conditionsInput.SelectedValueBinding.BindDataContext(Binding.Property((ItemModelView items) => items.Conditions));
			var SubmitButton = CreateSubmitButton();

			// Create form
			var form = new DynamicLayout
			{
				Padding = 10,

			};
			form.BeginVertical();
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
			form.EndVertical();
			form.BeginHorizontal();
			form.AddSeparateRow(
				null,
				new Button() {Command = SubmitButton, Width = 100, Height = 10, Text = "Submit"},
				null
			);
			form.EndHorizontal();
			return form;
		}
		private Command CreateSubmitButton()
		{
			var createCommand = new Command();
			createCommand.Executed += (sender, e) =>
			{
				var model = (ItemModelView)DataContext;
				model.UpdatedAt = DateTime.Now;
				_onSubmit?.Invoke(model);
				Close();
			};
			return createCommand;
		}
	}
}
