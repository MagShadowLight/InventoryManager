using System;
using Eto.Forms;
using Eto.Drawing;
using InventBox.Desktop.ModelView;
using InventBox.Core.Models;
using System.Collections.Generic;
using InventBox.Core;
using InventBox.Desktop.Interfaces;
using InventBox.Desktop.ModelViews;

namespace InventBox.Desktop.Components.ItemsForm
{
	public enum Mode
	{
		Create, 
		Edit
	}
	/// <summary>
	/// Represents the dialog for creating and editing items.
	/// </summary>
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
		/// <summary>
		/// Initialize a new instance for items dialog.
		/// </summary>
		/// <param name="modelView">Model view for the items.</param>
		/// <param name="mode">The mode for switching between creating and editing items</param>
		/// <param name="onSubmitEvent">Event handler for submit.</param>
		/// <param name="path">The path for logging purpose.</param>
		/// <param name="logger">The instance for logger.</param>
		public ItemsDialog(ItemModelView modelView, Mode mode, Action<Items> onSubmitEvent, string path, FileLogger logger)
		{
			_path = path;
			_logger = logger;
			_mode = mode;
			_logger.Logs($"Opening {(_mode == Mode.Create ? "Create item dialog" : "Edit item dialog")}", _path);
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
			_onSubmit = onSubmitEvent;
			Resizable = true;
			DataContext = modelView;
			Title = _mode == Mode.Create ? "Create item" : "Edit item" ;
			Size = new Size(500,750);
			var form = CreateForm(modelView);
			Content = form;
		}
		/// <summary>
		/// Create the dynamic layout for creating or editing items.
		/// </summary>
		/// <param name="modelView">Model view for the item.</param>
		/// <returns>Layout for the dialog.</returns>
		public DynamicLayout CreateForm(ItemModelView modelView)
		{
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
			var categoryInput = CreateCategoryListBox();
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
				"Warrantly",
				(warrantly == null) ? AddButton("Create Warrantly", 100, 40, OnWarrantlyCreate) : AddButton("Remove Warrant", 100, 40, OnWarrantlyDelete)
			);
			form.Add((warrantly != null) ? "Warrantly added" : "");
			form.AddRow(
				"Insurance",
				(_insurance == null) ? AddButton("Create Insurance", 100, 40, OnInsuranceCreate) : AddButton("Remove Insurance", 100, 40, OnInsuranceDelete)
			);
			form.Add((_insurance != null) ? "Insurance added" : "");
			form.EndVertical();
			form.BeginHorizontal();
			form.AddSeparateRow(
				null,
				new Button() {Command = SubmitButton, Width = 100, Height = 40, Text = "Submit"},
				null
			);
			form.EndHorizontal();
			form.AddRow("");
			return form;
		}
		/// <summary>
		/// Delete the insurance from the items.
		/// </summary>
        private void OnInsuranceDelete()
        {
			_logger.Logs("Removing insurance from the item.", _path);
			_insurance = null;
			_logger.Logs("Insurance removed.", _path);
			Content = CreateForm(_itemModel);
        }
		/// <summary>
		/// Delete the warrantly from the items.
		/// </summary>
        private void OnWarrantlyDelete()
        {
			_logger.Logs("Removing warrantly from the item.", _path);
			warrantly = null;
			_logger.Logs("warrantly removed.", _path);
			Content = CreateForm(_itemModel);
        }
		/// <summary>
		/// Create the insurance with user input data.
		/// </summary>
        private void OnInsuranceCreate()
        {
			_logger.Logs("Creating insurance into the item.", _path);
			InsuranceModelView insurance = new InsuranceModelView() {Id = ModelsList.items.Count + 1};
			var InsuranceDialog = new InsuranceDialog(insurance, _path, _logger, _mode, new Size(500,250), insure => insurance = insuranceModelCopy(insure));
			InsuranceDialog.Closed += (sender, e) => {
				if (insurance.StartDate <= DateTime.MinValue.AddDays(30) || insurance.EndDate <= DateTime.MinValue.AddDays(30))
					return;
				_insurance = insurance;
				_logger.Logs("insurance created.", _path);
				Content = CreateForm(_itemModel);
			};
			InsuranceDialog.ShowModal();
        }
		/// <summary>
		/// Copy the category data into the model view.
		/// </summary>
		/// <param name="category">The data of the category.</param>
		/// <returns>The model view for the category.</returns>
        private CategoryModelView CopyCategoryModelView(Category category)
		{
			return new CategoryModelView
			{
				Id = category.Id,
				Name = category.Name,
				Description = category.Description	
			};
		}
		/// <summary>
		/// Copy the location data into the model view.
		/// </summary>
		/// <param name="location">The data of the location.</param>
		/// <returns>The model view for the location.</returns>
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
		/// <summary>
		/// Create the list box for displaying list of categories.
		/// </summary>
		/// <returns>List box for the dialog.</returns>
		private ListBox CreateCategoryListBox()
		{
			return new ListBox
			{
				Height = 100,
				DataStore = categories
			};
		}
		/// <summary>
		/// Create the list box for displaying list of locations.
		/// </summary>
		/// <returns>List box for the dialog.</returns>
		private ListBox CreateLocationListBox()
		{
			return new ListBox
			{
				Height = 100,
				DataStore = locations
			};
		}
		/// <summary>
		/// Create the command for submitting the data.
		/// </summary>
		/// <returns>The command for submit.</returns>
        public Command CreateSubmitButton()
		{
			_logger.Logs("Submitting data", _path);
			var createCommand = new Command();
			createCommand.Executed += (sender, e) =>
			{
				var model = (ItemModelView)DataContext;
				model.Category = CheckCategory(category);
				model.Locations = CheckLocation(location);
				model.UpdatedAt = DateTime.Now;
				model.Warrantly = warrantly;
				model.Insurance = _insurance;
				if (model.Name == null)
					MessageBox.Show("Please add the name to the items", "Empty Name Detected", MessageBoxButtons.OK, MessageBoxType.Information);
				else if (model.Quantity <= 0)
					MessageBox.Show("Quantity must be greater than zero.", "Quantity less than or equal to zero detected", MessageBoxButtons.OK, MessageBoxType.Information);
				else
				{
					_onSubmit?.Invoke(model);
					Close();
				}
			};
			return createCommand;
		}
		/// <summary>
		/// Check if the location is null or not.
		/// </summary>
		/// <param name="location">The data for the locations.</param>
		/// <returns>It returns the locations or null.</returns>

        private Locations CheckLocation(Locations location)
        {
			if (location != null)
				return new Locations
				{
					Id = location.Id,
					Floor = location.Floor,
					Room = location.Room,
					Container = location.Container,
					X = location.X,
					Y = location.Y
				};
			return null;
        }
		/// <summary>
		/// Check if the category is null or not.
		/// </summary>
		/// <param name="category">The data for the category.</param>
		/// <returns>It returens the category data or null.</returns>
        private Category CheckCategory(Category category)
		{
			if (category != null)
				return new Category
				{
					Id = category.Id,
					Name = category.Name,
					Description = category.Description
				};
			return null;
		}
		/// <summary>
		/// Create the button with the command attached.
		/// </summary>
		/// <param name="text">The text for the button.</param>
		/// <param name="width">The width for the button.</param>
		/// <param name="height">The height for the button.</param>
		/// <param name="eventHandler">The handler for clicking the button.</param>
		/// <returns></returns>
		private Button AddButton(string text, int width, int height, Action eventHandler)
		{
			var command = new Command();
			command.Executed += (sender, eventArgs) => eventHandler();
			return new Button { Text = text, Width = width, Height = height, Command = command};
		}
		/// <summary>
		/// Create the warrantly with the user input data.
		/// </summary>
		private void OnWarrantlyCreate()
		{
			_logger.Logs("Creating warrantly into item.", _path);
			WarrantlyModelView warrantly = new WarrantlyModelView() {Id = ModelsList.items.Count + 1};
			var warrantDialog = new WarrantlyDialog(warrantly, _path, _logger, Mode.Create, new Size(500, 250), warrantly => warrantly = warrantlyModelCopy(warrantly));
			warrantDialog.Closed += (sender, e) => {
				if (warrantly.StartDate <= DateTime.MinValue.AddDays(30) || warrantly.EndDate <= DateTime.MinValue.AddDays(30))
					return;
				this.warrantly = warrantly;
				_logger.Logs("Warrantly created.", _path);
				Content = CreateForm(_itemModel);
			};
			warrantDialog.ShowModal();
		}
		/// <summary>
		/// Copy the warrantly into the model view.
		/// </summary>
		/// <param name="warrantly">The warrantly data.</param>
		/// <returns>The model view for warrantly.</returns>
		private WarrantlyModelView warrantlyModelCopy(Warrantly warrantly)
		{
			return new WarrantlyModelView
			{
				Id = warrantly.Id,
				StartDate = warrantly.StartDate,
				EndDate = warrantly.EndDate,
				Status = warrantly.Status,
				Provider = warrantly.Provider,
				ContactNumber = warrantly.ContactNumber	
			};
		} 
		/// <summary>
		/// Copy the insurance into the model view.
		/// </summary>
		/// <param name="insurance">The data for insurance.</param>
		/// <returns>The model view for insurance.</returns>
		private InsuranceModelView insuranceModelCopy(Insurance insurance)
		{
			return new InsuranceModelView
			{
				Id = insurance.Id,
				StartDate = insurance.StartDate,
				EndDate = insurance.EndDate,
				Insured = insurance.Insured,
				Provider = insurance.Provider,
				ContactNumber = insurance.ContactNumber	
			};
		} 
	}
}
