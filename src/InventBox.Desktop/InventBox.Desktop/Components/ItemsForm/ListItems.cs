using Eto.Forms;
using Eto.Drawing;
using InventBox.Desktop.ModelView;
using InventBox.Core.Models;
using InventBox.Core;
using System.Linq;
using System;
using InventBox.Desktop.Interfaces;
using System.Threading.Tasks;
using System.IO;
using System.Collections.Generic;
using InventBox.Desktop.Enum;
using InventBox.Desktop.Component.BarCodeComponents;
using InventBox.Desktop.Utils;

namespace InventBox.Desktop.Components.ItemsForm
{
	/// <summary>
	/// Represents the panel for list of items.
	/// </summary>
	public partial class ListItems : Panel, IEventHandler, IControls<Items, ItemModelView>
	{
		private static string TmpDir = Path.Combine(Path.GetTempPath(), "InventBox", "Data", "Items");
		private string TmpPath = Path.Combine(TmpDir, "Data-Item-tmp.csv");
		private static string TmpDir2 = Path.Combine(Path.GetTempPath(), "InventBox", "Data");
		private string TmpCategoryPath = Path.Combine(TmpDir2, "Category", "Data-Category-tmp.csv");
		private string TmpLocationPath = Path.Combine(TmpDir2, "Locations", "Data-Location-tmp.csv");
		private JsonParser<Items> jsonParser;
		private SearchOptions search = SearchOptions.Name;
		private TextBox searchBar;
		private List<Items> _items = new List<Items>();
		private static string _loggerpath;
		private ImageCapture _capture;
		private BarCodeScanner _scanner;
		private static FileLogger _logger;
		private DataManagement<Items> _dataManagement;
		private DataManagement<Category> _categoryManagement;
		private DataManagement<Locations> _locationManagement;
		private GridView _grid;
		private string searchtext = "";
		private AppUtils<Items> _utils = new AppUtils<Items>(_logger, _loggerpath);
		private Tutorial itemTutorial = new Tutorial(new Size(500,550),
		"ItemTutorialDone.md",
		"Inventory section is where you can create, manage, and delete items into or from the list.",
		"It include the item id, name, description, quantity, serial and model number, manufacturer name, notes, category, where the items at, warrantly, and insurance information.",
		"To create, click the 'Create new item' button. To edit or delete, you must select item from the list first then click either 'Edit selected items' or 'Delete selected item'. You can also save and load items from the inventory into a file. Click 'Save Data' button to save and 'Load Data' button to load.",
		"You can search through the items. Click the dropdown at the top left that say 'Name' and select either item name, category name, and the floor or room the item is at and then type in the search bar to filter and click the 'search' button.",
		"You can also scan the bar code to filter the item name from the list. Click the 'Scan item name' button and scan the bar code by holding it on camera or bar code scanner."
		);
		/// <summary>
		/// Initialize a new instance for the panel.
		/// </summary>
		/// <param name="path">The path for logger.</param>
		/// <param name="logger">The file logger for logging purposes.</param>
		public ListItems(string path, FileLogger logger)
		{
			_items = ModelsList.items;
			_loggerpath = path;
			_logger = logger;
			jsonParser = new JsonParser<Items>(_logger, _loggerpath);
			_scanner = new BarCodeScanner(_loggerpath);
			_dataManagement = new DataManagement<Items>(_loggerpath);
			_capture = new ImageCapture(_loggerpath);
			_grid = CreateGrid();
			_utils = new AppUtils<Items>(_logger, _loggerpath, _grid, jsonParser);
			RefreshData();
			Visible = false;
			Content = CreateDynamicLayout();

			if (!File.Exists("ItemTutorialDone.md"))
				ShowTutorial();
		}
		/// <summary>
		/// Copy the items into the clipboard.
		/// </summary>
		public void OnCopy()
		{
			_logger.Logs("Copying data to clipboard.", _loggerpath);
			if (_grid.SelectedItem == null)
				return;
			Items SelectedValues = (Items)_grid.SelectedItem;
			var jsonItem = jsonParser.ParseJson(SelectedValues);
			Clipboard.Instance.Clear();		
			Clipboard.Instance.Text = jsonItem;
			_logger.Logs("Data copied successfully", _loggerpath);
		}
		/// <summary>
		/// Show the tutorial dialog (Wall of text)
		/// </summary>
		async void ShowTutorial() {
			_logger.Logs("Showing item tutorial for first time users.", _loggerpath);
			await itemTutorial.ShowModalAsync();
		}
		/// <summary>
		/// Creating the context menu for the grid.
		/// </summary>
		/// <returns>Context menu for grid with options.</returns>
		public ContextMenu CreateContextMenu()
		{
			_logger.Logs("Creating context menu", _loggerpath);
			var CopyItemCommand = _utils.CreateMenuItem("Copy Item", OnCopy);
			var CreateItemCommand = _utils.CreateMenuItem("Create new item", OnCreate);
			var UpdateItemCommand = _utils.CreateMenuItem("Edit item", OnEdit);			
			var DeleteItemCommand = _utils.CreateMenuItem("Delete item", OnDelete);			
			var SaveItemCommand = _utils.CreateMenuItem("Save items", OnSave);			
			var LoadItemCommand = _utils.CreateMenuItem("Load items", OnLoad);
			var EditMenu = new ButtonMenuItem
			{
				Text = "Edit",
				Items =
				{
					CreateItemCommand,
					UpdateItemCommand,
					DeleteItemCommand,
					SaveItemCommand,
					LoadItemCommand
				}
			};
			return new ContextMenu
			{
				Items =
				{
					EditMenu,
					CopyItemCommand
				},
			};
		}
		/// <summary>
		/// Refresh the data into the grid.
		/// </summary>
		public void RefreshData()
		{
			_grid.DataStore = _items.ToArray<Items>();
		}
		/// <summary>
		/// Create the grid for the panel.
		/// </summary>
		/// <returns>The view for the grid.</returns>
        public GridView CreateGrid()
        {
			_logger.Logs("Creating the item grid.", _loggerpath);
			var grid = new GridView()
			{
				GridLines = GridLines.Both,
				AllowMultipleSelection = false,
				Columns =
				{
					_utils.GetColumn("Id", i => i.Id.ToString()),
					_utils.GetColumn("Name", i => i.Name),
					_utils.GetColumn("Description", i => i.Description),
					_utils.GetColumn("Quantity", i => i.Quantity.ToString()),
					_utils.GetColumn("Serial Number", i => i.SerialNumber),
					_utils.GetColumn("Model Number", i => i.ModelNumber),
					_utils.GetColumn("Manufacturer", i => i.Manufacturer),
					_utils.GetColumn("Notes", i => i.Notes),
					_utils.GetColumn("Conditions", i => i.Conditions.ToString()),
					_utils.GetColumn("Category", i => (i.Category != null) ? i.Category.Name : ""),
					_utils.GetColumn("Floor", i => (i.Locations != null) ? i.Locations.Floor : ""),
					_utils.GetColumn("Room", i => (i.Locations != null) ? i.Locations.Room : ""),
					_utils.GetColumn("Container", i => (i.Locations != null) ? i.Locations.Container : ""),
					_utils.GetColumn("Warrantly Status", i => (i.Warrantly != null && i.Warrantly.Status != 0) ? i.Warrantly.Status.ToString() : "Not Warranted"),
					_utils.GetColumn("Warrant Provider", i => (i.Warrantly != null) ? i.Warrantly.Provider : ""),
					_utils.GetColumn("Warrant Contact #", i => (i.Warrantly != null) ? i.Warrantly.ContactNumber : "" ),
					_utils.GetColumn("Insurance Status", i => (i.Insurance != null && i.Insurance.Insured != 0) ? i.Insurance.Insured.ToString() : "Not Insured"),
					_utils.GetColumn("Insurance Provider", i => (i.Insurance != null) ? i.Insurance.Provider : ""),
					_utils.GetColumn("Insurance Contact #", i => (i.Insurance != null) ? i.Insurance.ContactNumber : "")
				},
				ContextMenu = CreateContextMenu()
			};
			return grid;
        }
		/// <summary>
		/// Create the layout for the panel.
		/// </summary>
		/// <returns>The layout for display.</returns>
		public DynamicLayout CreateDynamicLayout()
		{
			_logger.Logs("Creating the Inventory layout", _loggerpath);
			EnumDropDown<SearchOptions> searchDropDown = CreateSearchDropDown();
			searchBar = CreateSearchBar();
			DynamicLayout layout = new DynamicLayout();
			layout.BeginVertical(null, null, true, true);
			layout.BeginVertical();
			layout.BeginHorizontal();
			layout.Add(searchDropDown, false);
			layout.Add(searchBar, true);
			layout.Add(_utils.AddButton("Search", 100, 50, () => Search()), false);
			layout.Add(_utils.AddButton("Scan item name", 100, 50, async () => await OnScanBarCode()), false);
			layout.EndHorizontal();
			layout.EndVertical();
			layout.BeginVertical();
			layout.Add((ModelsList.items.Count > 0 
				? _grid 
				: new Label{
					Text = "The list is empty, Create new item or load from file to add it to the list.", 
					TextAlignment = TextAlignment.Center, 
					VerticalAlignment = VerticalAlignment.Center, 
					Font = new Font(
						FontFamilies.Serif, 
						12.0f, 
						FontStyle.Bold, 
						FontDecoration.None)
					}), true, true);
			layout.Add(null, true, false);
			layout.AddSeparateRow(4, null, true, false,
				new [] { 
					_utils.AddButton("Create new item", 100, 50, OnCreate),
					_utils.AddButton("Edit selected item", 100, 50, OnEdit),
					_utils.AddButton("Delete selected item", 100, 50, OnDelete),
					null,
					_utils.AddButton("Save Data", 100, 50, OnSave),
					_utils.AddButton("Load Data", 100, 50, OnLoad)
				}
			);
			layout.EndVertical();
			layout.EndVertical();
			return layout;
		}
		/// <summary>
		/// Create the enum drop down for switching search mode.
		/// </summary>
		/// <returns>Drop down for the enum.</returns>
		private EnumDropDown<SearchOptions> CreateSearchDropDown()
		{
			_logger.Logs("Creating the search option drop down.", _loggerpath);
			var dropdown = new EnumDropDown<SearchOptions>() {Cursor = Cursors.Pointer, Width = 100 };
			dropdown.SelectedValue = search;
			dropdown.SelectedValueChanged += (sender, e) =>
			{
				search = dropdown.SelectedValue;
				Content = CreateDynamicLayout();
			};
			return dropdown;
		}
		/// <summary>
		/// Create the text box for searching.
		/// </summary>
		/// <returns>Text box to display.</returns>
		public TextBox CreateSearchBar()
		{
			_logger.Logs("Creating the search bar.", _loggerpath);

			TextBox textBox = new TextBox() {Text = "", PlaceholderText = $"Search {search.ToString().ToLower()}"};
			textBox.TextChanged += (sender, e) =>
			{
				searchtext = textBox.Text;
			};
			textBox.KeyDown += (sender, e) =>
			{
				if (e.Key == Keys.Enter)
				{
					Search();
				}
			};
			return textBox;
		}
		/// <summary>
		/// Searching either the item name, category name, floor, or room for items.
		/// </summary>
		public void Search()
		{
			_logger.Logs($"Searching the item by {searchBar.Text}", _loggerpath);
			if (ModelsList.items.Count <= 0) {
				MessageBox.Show("The item list is empty. Please add one to search.", "Search", MessageBoxButtons.OK, MessageBoxType.Information);
				return;
			}
			if (string.IsNullOrEmpty(searchtext)) 
			{
				if (_items.Count == ModelsList.items.Count)
					MessageBox.Show("Search bar is empty. Please type in the search bar", "Search", MessageBoxButtons.OK, MessageBoxType.Information);
				_items = ModelsList.items;
			}
			else {
				if (search == SearchOptions.Name)
					_items = ModelsList.items.Where((item) => item.Name.ToLower().Contains(searchtext.ToLower())).ToList();
				if (search == SearchOptions.Category)
					_items = ModelsList.items.Where(item => (item.Category != null) ? item.Category.Name.ToLower().Contains(searchtext.ToLower()) : item.Name.Contains(string.Empty)).ToList();
				if (search == SearchOptions.Floor)
					_items = ModelsList.items.Where(item => (item.Locations != null) ? item.Locations.Floor.ToLower().Contains(searchtext.ToLower()) : item.Name.Contains(string.Empty)).ToList();
				if (search == SearchOptions.Room)
					_items = ModelsList.items.Where(item => (item.Locations != null) ? item.Locations.Room.ToLower().Contains(searchtext.ToLower()) : item.Name.Contains(string.Empty)).ToList();
			}
			RefreshData();
		} 
		/// <summary>
		/// Scan the bar code for filtering the item name.
		/// </summary>
		/// <returns></returns>
		private async Task OnScanBarCode()
		{
			await _logger.LogsAsync("Scanning item name by bar code.", _loggerpath);
			if (_items.Count <= 0)
			{
				MessageBox.Show("The item list is empty. Please add one to scan.");
				return;
			}
			Items items = new Items();
			await _capture.OpenCapture(new System.Threading.CancellationTokenSource());
			if (!_capture.IsCaptureOpen)
				return;
			var dialog = new BarCodeScannerDialog(_capture, _loggerpath, _logger);
		    await dialog.ShowModalAsync();
			dialog.Closing += async (sender, e) =>
			{
				if (!dialog.IsSuspended)
				{
					await _capture.CloseCapture();
					return;
				}
			};
			string name = _scanner.DecodeBarCode(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),".tmp", "InventBox", "Images", "Barcode.png"));
			if (string.IsNullOrEmpty(name)) {
				await _logger.LogsAsync("Failed to scan barcode.", _loggerpath);
				var faileddialog = MessageBox.Show("Failed to scan barcode", MessageBoxButtons.OK, MessageBoxType.Error);
                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();
                dialog.Dispose();
                return;
			}
			_items = ModelsList.items.Where(item => item.Name == name).ToList();
			RefreshData();
			await _logger.LogsAsync("Bar code scanned", _loggerpath);
			dialog.Dispose();
		}
		/// <summary>
		/// Create the items from the user input.
		/// </summary>
		public void OnCreate()
		{
			_logger.Logs("Creating a new item.", _loggerpath);
			ItemModelView modelView = new ItemModelView(){Id = ModelsList.items.Count + 1, Conditions = Conditions.NA};
			var createItemDialog = new ItemsDialog(modelView, Mode.Create, item => ModelsList.items.Add(item), _loggerpath, _logger);
			createItemDialog.Closed += (sender, e) => RefreshData();
			createItemDialog.ShowModal();
			_utils.CreateDirectory(TmpDir);
			_dataManagement.Save(ModelsList.items, TmpPath);
			_items = ModelsList.items;
			_logger.Logs("Item created successfully.", _loggerpath);
			Content = CreateDynamicLayout();
			RefreshData();
		}
		/// <summary>
		/// Saving the data from the list into the file.
		/// </summary>
		public void OnSave()
		{
			_logger.Logs("Saving items into file.", _loggerpath);			
			if (_items.Count <= 0) {
				_logger.Error("Saving failed. Item list is empty.", _loggerpath);
				MessageBox.Show("Items list is empty. Please add one or load from file.", "Save failed.", MessageBoxButtons.OK, MessageBoxType.Information);
				return;
			}
			_categoryManagement = new DataManagement<Category>(_loggerpath);
			_locationManagement = new DataManagement<Locations>(_loggerpath);
			Uri homeDir = new Uri(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile));	
			var saveDialog = new SaveFileDialog
			{
				Filters =
				{
					new FileFilter("CSV FIle", ".csv")
				},
				Directory = homeDir
			};
			var result = saveDialog.ShowDialog(this);
			if (result == DialogResult.Cancel) {
				saveDialog.Dispose();
				return;
			}
			if (saveDialog.FileName != string.Empty && ModelsList.items.Count > 0) {
				if (!saveDialog.FileName.Contains(".csv"))
					saveDialog.FileName = saveDialog.FileName + ".csv";
				_dataManagement.Save(ModelsList.items, saveDialog.FileName);
				var categoryPath = Path.Combine(Path.GetDirectoryName(saveDialog.FileName), $"{saveDialog.FileName}-Category.csv");
				var locationPath = Path.Combine(Path.GetDirectoryName(saveDialog.FileName), $"{saveDialog.FileName}-Location.csv");
				_categoryManagement.Save(ModelsList.categories, categoryPath);
				if (File.Exists(TmpCategoryPath))
					File.Delete(TmpCategoryPath);
				_locationManagement.Save(ModelsList.locations, locationPath);
				if (File.Exists(TmpLocationPath))
					File.Delete(TmpLocationPath);
				File.Delete(TmpPath);
				_logger.Logs("Data saved successfully.", _loggerpath);
			}
			else if (string.IsNullOrEmpty(saveDialog.FileName)) {
				_logger.Logs("Saving data cancelled.", _loggerpath);
				saveDialog.Dispose();
				return;
			}
			saveDialog.Dispose();
		}
		/// <summary>
		/// Loading the item data from the file.
		/// </summary>
		public void OnLoad()
		{
			_logger.Logs("Loading data from file.", _loggerpath);
			Uri path = new Uri(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile));		
			var loadDialog = new OpenFileDialog
			{
				Filters =
				{
					new FileFilter("CSV File", ".csv")		
				},
				Directory = path
			};
			var result = loadDialog.ShowDialog(this);
			if (result == DialogResult.Cancel) {
				_logger.Logs("Loading data cancelled.", _loggerpath);
				loadDialog.Dispose();
				return;
			}
			if (!loadDialog.FileName.Contains(".csv"))
				loadDialog.FileName = string.Empty;
			if (!string.IsNullOrEmpty(loadDialog.FileName)) {
				ModelsList.items = _dataManagement.Load(loadDialog.FileName, true);
				if (ModelsList.items.Count == 0)
				{
					_logger.Error("Loading item failed. Invalid data.", _loggerpath);
					MessageBox.Show("Invalid data. Please choose a different file", "Load failed.", MessageBoxButtons.OK, MessageBoxType.Information);
					return;
				}
				_items = ModelsList.items;
				foreach (var item in _items)
				{
					if ((!ModelsList.categories.Contains(ModelsList.categories.Find(i => i.Id == item.Category.Id)) && item.Category.Id > 0) && item.Category != null)
						ModelsList.categories.Add(item.Category);
					if ((!ModelsList.locations.Contains(ModelsList.locations.Find(i => i.Id == item.Locations.Id)) && item.Locations.Id > 0) && item.Locations != null)
						ModelsList.locations.Add(item.Locations);
				}
				_logger.Logs("Loading data successfully.", _loggerpath);
				Content = CreateDynamicLayout();
				RefreshData();
			} else
			{
				_logger.Error("Loading data failed. File name is empty.", _loggerpath);
				MessageBox.Show("File name is empty.", "Load failed.", MessageBoxButtons.OK, MessageBoxType.Information);
			}
			loadDialog.Dispose();
		}
		/// <summary>
		/// Edit the items from the selected list.
		/// </summary>
		public void OnEdit()
		{
			_logger.Logs("Editing the item.", _loggerpath);
			if (_items.Count <= 0) {
				_logger.Error("Editing item failed. Item list is empty.", _loggerpath);
				MessageBox.Show("The list is empty. Please create one or load from file.", "Item not selected", MessageBoxButtons.OK, MessageBoxType.Information);
				return;
			}
			if (_grid.SelectedItem == null) {
				_logger.Error("Editing item failed. Item is not selected.", _loggerpath);
				MessageBox.Show("Item have not been selected. Please select one", "Item not selected", MessageBoxButtons.OK, MessageBoxType.Information);
				return;
			}
			Items item = (Items)_grid.SelectedItem;
			var index = ModelsList.items.IndexOf(item);
			ItemModelView modelView = ModelViewCopy(item);
			var editItemDialog = new ItemsDialog(modelView, Mode.Edit, item => ModelsList.items[index] = item, _loggerpath, _logger);
			editItemDialog.Closed += (sender, e) => { 
				_utils.CreateDirectory(TmpDir);
				_dataManagement.Save(ModelsList.items, TmpPath);
				_items = ModelsList.items;
				_logger.Logs("Item edited successfully.", _loggerpath);
				RefreshData();			
			};
			editItemDialog.ShowModal();
		}
		/// <summary>
		/// Delete the item from the list.
		/// </summary>
		public void OnDelete()
		{
			_logger.Logs("Deleting item from the list.", _loggerpath);
			if (_items.Count <= 0) {
				_logger.Error("Deleting item failed. Item list is empty.", _loggerpath);
				MessageBox.Show("The list is empty. Please create one or load from file.", "Item not selected", MessageBoxButtons.OK, MessageBoxType.Information);
				return;
			}
			if (_grid.SelectedItem == null) {
				_logger.Error("Deleting item failed. Item is not selected.", _loggerpath);
				MessageBox.Show("Item have not been selected. Please select one", "Item not selected", MessageBoxButtons.OK, MessageBoxType.Information);
				return;
			}
			Items item = (Items)_grid.SelectedItem;
			var index = ModelsList.items.IndexOf(item);
			var deleteDialog = MessageBox.Show("Are you sure to delete the selected item?", "Delete selected item", MessageBoxButtons.YesNo, MessageBoxType.Question, MessageBoxDefaultButton.Yes);
			if (deleteDialog != DialogResult.Yes) {
				_logger.Logs("Deleting item cancelled.", _loggerpath);
				return;
			}
			ModelsList.items.Remove(item);
			_utils.CreateDirectory(TmpDir);
			if (ModelsList.items.Count > 0)
				_dataManagement.Save(ModelsList.items, TmpPath);
			else
				File.Delete(TmpPath);
			_items = ModelsList.items;
			_logger.Logs("Item deleted successfully.", _loggerpath);
			Content = CreateDynamicLayout();
			RefreshData();
		}
		/// <summary>
		/// Copy the item data into model view.
		/// </summary>
		/// <param name="item">The data for the item.</param>
		/// <returns>Model view for the item.</returns>
		public ItemModelView ModelViewCopy(Items item) {
			return new ItemModelView
			{
				Id = item.Id,
				Name = item.Name,
				Description = item.Description,
				Quantity = item.Quantity,
				SerialNumber = item.SerialNumber,
				ModelNumber = item.ModelNumber,
				Manufacturer = item.Manufacturer,
				Notes = item.Notes,
				CreatedAt = item.CreatedAt,
				UpdatedAt = item.UpdatedAt,
				Conditions = item.Conditions,
				Category = item.Category,
				Locations = item.Locations
			};
		}
	}
}
