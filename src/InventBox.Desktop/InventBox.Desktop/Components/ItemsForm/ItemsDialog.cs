using System;
using Eto.Forms;
using Eto.Drawing;
using InventBox.Desktop.ModelView;
using InventBox.Core.Models;
using System.Collections.Generic;
using InventBox.Core.Utils;
using System.Data;
using InventBox.Core;
using InventBox.Desktop.Interfaces;
using InventBox.Desktop.ModelViews;
using System.Linq;

namespace InventBox.Desktop.Components.ItemsForm
{
	public enum Mode
	{
		Create, 
		Edit
	}
	public partial class ItemsDialog : Dialog, IDialogs<ItemModelView>
	{
		private List<CategoryModelView> categories = new List<CategoryModelView>();
		private GridView _grid = new GridView();
		private static FileLogger _logger;
		private static string _path;
		private readonly Mode _mode;
		private readonly Action<Items> _onSubmit;
		private CategoryModelView _modelView;
		public ItemsDialog(ItemModelView modelView, Mode mode, Action<Items> onSubmitEvent, string path, FileLogger logger)
		{
			foreach (var category in ModelsList.categories)
			{
				var temp = CopyCategoryModelView(category);
				categories.Add(temp);
			}
			_grid = CreateGrid();
			_path = path;
			_logger = logger;
			_mode = mode;
			_onSubmit = onSubmitEvent;
			Resizable = true;
			DataContext = modelView;
			Title = _mode == Mode.Create ? "Create item" : "Edit item" ;
			Size = new Size(500,750);
			var form = CreateForm(modelView);
			Content = form;
		}

		public DynamicLayout CreateForm(ItemModelView modelView)
		{
			// Create dropdown list
			
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
			var categoryInput = CreateCategoryGrid();

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
			var selectedValue = _grid.SelectedItem;
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
			form.AddRow(
				"Category",
				categoryInput
			);
			form.EndVertical();
			form.BeginHorizontal();
			form.AddSeparateRow(
				null,
				new Button() {Command = SubmitButton, Width = 100, Height = 10, Text = "Submit"},
				null
			);
			form.EndHorizontal();
			form.AddRow("");
			return form;
		}
		private CategoryModelView CopyCategoryModelView(Category category)
		{
			return new CategoryModelView
			{
				Id = category.Id,
				Name = category.Name,
				Description = category.Description	
			};
		}

        private Panel CreateCategoryGrid()
        {
			return new Panel() {
				Content = _grid
			};
        }
		// private StackLayout CreateLayout()
		// {
		// 	var layout = new StackLayout
		// 	{
				
		// 	};
		// 	layout.DataContext = Binding.Property<Category, string>(c => c.Name);
		// 	layout.Content = CreateRadio();
		// 	return layout;
		// }
		// private RadioButtonList CreateRadio() {
		// 	var list = new RadioButtonList
		// 	{
		// 		Height = 100,
		// 		Width = 500,
		// 		DataContext = Binding.Property<Category, string>(c => c.Name),

		// 	};
		// 	list.Content = new RadioButton
		// 	{
		// 		Text = list.DataContext.ToString()
		// 	};
		// 	return list;
		// }

		private GridView CreateGrid()
		{
			_grid = new GridView()
			{
				GridLines = GridLines.Both,
				Size = new Size(150, 350),
				AllowMultipleSelection = false,
				Columns =
				{
					new GridColumn
					{
						HeaderText = "Category Name",
						Editable = false,
						DataCell = new TextBoxCell
						{			
							Binding = Binding.Delegate<CategoryModelView, string>(c => c.Name)
							// VerticalAlignment = VerticalAlignment.Top,
						},
					}
				},
				DataStore = categories,
			};
			_grid.RowHeight = 10;
			return _grid;
		}

        public Command CreateSubmitButton()
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
