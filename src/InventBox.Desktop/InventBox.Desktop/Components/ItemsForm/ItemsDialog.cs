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
		private List<Category> categories = new List<Category>();
		private List<Locations> locations = new List<Locations>();
		private static FileLogger _logger;
		private static string _path;
		private readonly Mode _mode;
		private Category category;
		private Locations location;
		private Warrantly warrantly;
		private Insurance _insurance;
		private readonly Action<Items> _onSubmit;
		private ItemModelView _itemModel;
		public ItemsDialog(ItemModelView modelView, Mode mode, Action<Items> onSubmitEvent, string path, FileLogger logger)
		{
			_itemModel = modelView;
			foreach (var category in ModelsList.categories)
			{
				var temp = CopyCategoryModelView(category);
				categories.Add(temp);
			}
			foreach (var location in ModelsList.locations)
			{
				var temp = CopyLocationModelView(location);
				locations.Add(temp);
			}
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
			var noteInput = new TextBox() { Width = 200 };
			var conditionsInput = new EnumDropDown<Conditions>() { 
				Width = 200 
			};		
			var categoryInput = CreateCategory();
			var locationInput = CreateLocationListBox();

			// Bind those input to data
			nameInput.BindDataContext(t => t.Text, (ItemModelView items) => items.Name);
			descriptionInput.BindDataContext(t => t.Text, (ItemModelView items) => items.Description);
			quantityInput.ValueBinding.BindDataContext((ItemModelView items) => items.Quantity);
			serialNoInput.BindDataContext(t => t.Text, (ItemModelView items) => items.SerialNumber);
			modelNoInput.BindDataContext(t => t.Text, (ItemModelView items) => items.ModelNumber);
			ManufacturerInput.BindDataContext(t => t.Text, (ItemModelView items) => items.Manufacturer);
			noteInput.BindDataContext(t => t.Text, (ItemModelView items) => items.Notes);
			conditionsInput.SelectedValueBinding.BindDataContext(Binding.Property((ItemModelView items) => items.Conditions));
			categoryInput.ItemTextBinding = Binding.Delegate<Category, string>(c => c.Name);
			categoryInput.SelectedValueChanged += (sender, e) => category = (Category)categoryInput.SelectedValue;
			locationInput.ItemTextBinding = Binding.Delegate<Locations, string>(c => c.Floor + " " + c.Room);
			locationInput.SelectedValueChanged += (sender, e) => location = (Locations)locationInput.SelectedValue;
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
				"Note",
				noteInput
			);
			form.AddRow(
				"Condition",
				conditionsInput
			);
			form.AddRow(
				"Category"
			);
			form.EndVertical();
			form.Add(categoryInput, true, false);
			form.BeginVertical();
			form.AddRow("Location");
			form.EndVertical();
			form.Add(locationInput);
			form.BeginVertical();
			form.AddRow(
				"Warrant",
				(warrantly == null) ? AddButton("Create Warrant", 100, 10, OnWarrantCreate) : AddButton("Remove Warrant", 100, 10, OnWarrantDelete)
			);
			form.Add((warrantly != null) ? "Warrantly added" : "");
			form.AddRow(
				"Insurance",
				(_insurance == null) ? AddButton("Create Insurance", 100, 10, OnInsuranceCreate) : AddButton("Remove Insurance", 100, 10, OnInsuranceDelete)
			);
			form.Add((_insurance != null) ? "Insurance added" : "");
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

        private void OnInsuranceDelete()
        {
			_insurance = null;
			Content = CreateForm(_itemModel);
        }

        private void OnWarrantDelete()
        {
			warrantly = null;
			Content = CreateForm(_itemModel);
        }

        private void OnInsuranceCreate()
        {
			InsuranceModelView insurance = new InsuranceModelView() {Id = ModelsList.items.Count + 1};
			var InsuranceDialog = new InsuranceDialog(insurance, _path, _logger, _mode, new Size(500,250), insure => insurance = insuranceModelCopy(insure));
			InsuranceDialog.Closed += (sender, e) => {
				_insurance = insurance;
				Content = CreateForm(_itemModel);
			};
			InsuranceDialog.ShowModal();
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
		private LocationsModelView CopyLocationModelView(Locations location)
		{
			return new LocationsModelView
			{
				Id = location.Id,
				Floor = location.Floor,
				Room = location.Room,
				Container = location.Container,
				X = location.X,
				Y = location.Y
			};
		}

		private ListBox CreateCategory()
		{
			return new ListBox
			{
				Height = 100,
				DataStore = categories
			};
		}
		private ListBox CreateLocationListBox()
		{
			return new ListBox
			{
				Height = 100,
				DataStore = locations
			};
		}

        public Command CreateSubmitButton()
		{
			var createCommand = new Command();
			createCommand.Executed += (sender, e) =>
			{
				var model = (ItemModelView)DataContext;
				model.Category = SubmitCategory(category);
				model.Locations = SubmitLocation(location);
				model.UpdatedAt = DateTime.Now;
				model.Warrantly = warrantly;
				model.Insurance = _insurance;
				_onSubmit?.Invoke(model);
				Close();
			};
			return createCommand;
		}

        private Locations SubmitLocation(Locations location)
        {
            return new Locations
			{
				Id = location.Id,
				Floor = location.Floor,
				Room = location.Room,
				Container = location.Container,
				X = location.X,
				Y = location.Y
			};
        }

        private Category SubmitCategory(Category category)
		{
			return new Category
			{
				Id = category.Id,
				Name = category.Name,
				Description = category.Description
			};
		}
		private Button AddButton(string text, int width, int height, Action eventHandler)
		{
			var command = new Command();
			command.Executed += (sender, eventArgs) => eventHandler();
			return new Button { Text = text, Width = width, Height = height, Command = command};
		}
		private void OnWarrantCreate()
		{
			WarrantlyModelView warrant = new WarrantlyModelView() {Id = ModelsList.items.Count + 1};
			var warrantDialog = new WarrantlyDialog(warrant, _path, _logger, Mode.Create, new Size(500, 250), warrantly => warrant = warrantlyModelCopy(warrantly));
			warrantDialog.Closed += (sender, e) => {
				warrantly = warrant;
				Content = CreateForm(_itemModel);
			};
			warrantDialog.ShowModal();
		}
		private WarrantlyModelView warrantlyModelCopy(Warrantly warrant)
		{
			return new WarrantlyModelView
			{
				Id = warrant.Id,
				StartDate = warrant.StartDate,
				EndDate = warrant.EndDate,
				Status = warrant.Status,
				Provider = warrant.Provider,
				ContactNumber = warrant.ContactNumber	
			};
		} 
		
		private InsuranceModelView insuranceModelCopy(Insurance warrant)
		{
			return new InsuranceModelView
			{
				Id = warrant.Id,
				StartDate = warrant.StartDate,
				EndDate = warrant.EndDate,
				Insured = warrant.Insured,
				Provider = warrant.Provider,
				ContactNumber = warrant.ContactNumber	
			};
		} 
	}
}
