using Eto.Forms;
using Eto.Drawing;
using InventBox.Desktop.ModelView;
using InventBox.Core.Models;
using InventBox.Core;
using System.Linq;
using System;
using InventBox.Desktop.Interfaces;
using System.Threading.Tasks;
using EtoApp;
using System.IO;
using System.Collections.Generic;
using InventBox.Desktop.Enum;

namespace InventBox.Desktop.Components.ItemsForm
{
	public partial class ListItems : Panel, IEventHandler, IControls<Items, ItemModelView>
	{
		private static string TmpDir = Path.Combine(Path.GetTempPath(), "InventBox", "Data", "Items");
		private string TmpPath = Path.Combine(TmpDir, "Data-Item-tmp.csv");
		private static string TmpDir2 = Path.Combine(Path.GetTempPath(), "InventBox", "Data");
		private string TmpCategoryPath = Path.Combine(TmpDir2, "Category", "Data-Category-tmp.csv");
		private string TmpLocationPath = Path.Combine(TmpDir2, "Locations", "Data-Location-tmp.csv");
		private JsonParser<Items> jsonParser;
		private Searchable search = Searchable.Name;
		private TextBox searchBar;
		private List<Items> _items = new List<Items>();
		private ImageCapture _capture = new ImageCapture();
		private static string _path;
		private BarCodeScanner _scanner;
		private static FileLogger _logger;
		private DataManagement<Items> _dataManagement;
		private DataManagement<Category> _categoryManagement;
		private DataManagement<Locations> _locationManagement;
		private GridView _grid;
		private string searchtext = "";
		public ListItems(string path, FileLogger logger)
		{
			jsonParser = new JsonParser<Items>();
			_items = ModelsList.items;
			_path = path;
			_logger = logger;
			_scanner = new BarCodeScanner(_path);
			_dataManagement = new DataManagement<Items>(_path);

			_grid = CreateGrid();
			RefreshData();
			Visible = false;
			Content = CreateDynamicLayout();
		}

		private void CreateDirectory(string dir)
		{
			if (!Directory.Exists(dir))
				Directory.CreateDirectory(dir);
		}

		public ContextMenu CreateContextMenu()
		{
			var CopyItemCommand = CreateMenuItem("Copy Item", OnCopy);
			var CreateItemCommand = CreateMenuItem("Create new item", OnCreate);
			var UpdateItemCommand = CreateMenuItem("Edit item", OnEdit);			
			var DeleteItemCommand = CreateMenuItem("Delete item", OnDelete);			
			var SaveItemCommand = CreateMenuItem("Save items", OnSave);			
			var LoadItemCommand = CreateMenuItem("Load items", OnLoad);
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

		public void OnCopy()
		{
			Items copiedItems = (Items)_grid.SelectedItem;
			var jsonItem = jsonParser.ParseJson(copiedItems);
			Clipboard.Instance.Clear();		
			Clipboard.Instance.Text = jsonItem;
		}

		public ButtonMenuItem CreateMenuItem(string text, Action clickHandler, Keys keys = Keys.None)
		{			
			var menuItem = new ButtonMenuItem{Text = text, Shortcut = keys};
			menuItem.Click += (sender, eventArgs) => clickHandler();
			return menuItem;
		}

		public void RefreshData()
		{
			_grid.DataStore = _items.ToArray<Items>();
		}

        public GridView CreateGrid()
        {
			var grid = new GridView()
			{
				GridLines = GridLines.Both,
				AllowMultipleSelection = false,
				Columns =
				{
					GetColumn("Id", i => i.Id.ToString()),
					GetColumn("Name", i => i.Name),
					GetColumn("Description", i => i.Description),
					GetColumn("Quantity", i => i.Quantity.ToString()),
					GetColumn("Serial Number", i => i.SerialNumber),
					GetColumn("Model Number", i => i.ModelNumber),
					GetColumn("Manufacturer", i => i.Manufacturer),
					GetColumn("Notes", i => i.Notes),
					GetColumn("Conditions", i => i.Conditions.ToString()),
					GetColumn("Category", i => (i.Category != null) ? i.Category.Name : ""),
					GetColumn("Floor", i => (i.Locations != null) ? i.Locations.Floor : ""),
					GetColumn("Room", i => (i.Locations != null) ? i.Locations.Room : ""),
					GetColumn("Container", i => (i.Locations != null) ? i.Locations.Container : ""),
					GetColumn("Warrantly Status", i => (i.Warrantly != null && i.Warrantly.Status != 0) ? i.Warrantly.Status.ToString() : "Not Warranted"),
					GetColumn("Warrant Provider", i => (i.Warrantly != null) ? i.Warrantly.Provider : ""),
					GetColumn("Warrant Contact #", i => (i.Warrantly != null) ? i.Warrantly.ContactNumber : "" ),
					GetColumn("Insurance Status", i => (i.Insurance != null && i.Insurance.Insured != 0) ? i.Insurance.Insured.ToString() : "Not Insured"),
					GetColumn("Insurance Provider", i => (i.Insurance != null) ? i.Insurance.Provider : ""),
					GetColumn("Insurance Contact #", i => (i.Insurance != null) ? i.Insurance.ContactNumber : "")
				},
				ContextMenu = CreateContextMenu()
			};
			return grid;
        }

		public GridColumn GetColumn(string header, Func<Items, string> data) {
			return new GridColumn
			{
				HeaderText = header,
				Editable = false,
				DataCell = GetData(data)
			};
		}

        public TextBoxCell GetData(Func<Items, string> data)
        {
			return new TextBoxCell
			{
				Binding = Binding.Delegate<Items, string>(data, null)
			};
        }


		public DynamicLayout CreateDynamicLayout()
		{
			EnumDropDown<Searchable> searchDropDown = CreateSearchDropDown();
			searchBar = CreateSearchBar();
			DynamicLayout layout = new DynamicLayout();
			layout.BeginVertical(null, null, true, true);
			layout.BeginVertical();
			layout.BeginHorizontal();
			layout.Add(searchDropDown, false);
			layout.Add(searchBar, true);
			layout.Add(AddButton("Search", 100, 50, () => Search()), false);
			layout.Add(AddButton("Scan barcode", 100, 50, async () => await OnScanBarCode()), false);
			layout.EndHorizontal();
			layout.EndVertical();
			// layout.AddSeparateRow(null, searchDropDown, searchBar, AddButton("Clear Search", 100, 50, () => ClearFilter()), AddButton("Scan barcode", 100, 50, async () => await OnScanBarCode()));
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
					AddButton("Create new item", 100, 50, OnCreate),
					AddButton("Edit selected item", 100, 50, OnEdit),
					AddButton("Delete selected item", 100, 50, OnDelete),
					null,
					AddButton("Save Data", 100, 50, OnSave),
					AddButton("Load Data", 100, 50, OnLoad)
				}
			);
			layout.EndVertical();
			layout.EndVertical();
			return layout;
		}
		private EnumDropDown<Searchable> CreateSearchDropDown()
		{
			var dropdown = new EnumDropDown<Searchable>() {Cursor = Cursors.Pointer, Width = 100 };
			dropdown.SelectedValue = search;
			dropdown.SelectedValueChanged += (sender, e) =>
			{
				search = dropdown.SelectedValue;
				Content = CreateDynamicLayout();
			};
			return dropdown;
		}

		public TextBox CreateSearchBar()
		{
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

		public void Search()
		{
			if (string.IsNullOrEmpty(searchtext)) 
			{
				if (_items.Count == ModelsList.items.Count)
					MessageBox.Show("Search bar is empty. Please type in the search bar", "Search", MessageBoxButtons.OK, MessageBoxType.Information);
				_items = ModelsList.items;
			}
			else {
				if (search == Searchable.Name)
					_items = ModelsList.items.Where((item) => item.Name.Contains(searchtext)).ToList();
				if (search == Searchable.Category)
					_items = ModelsList.items.Where(item => (item.Category != null) ? item.Category.Name.Contains(searchtext) : item.Name.Contains(string.Empty)).ToList();
				if (search == Searchable.Floor)
					_items = ModelsList.items.Where(item => (item.Locations != null) ? item.Locations.Floor.Contains(searchtext) : item.Name.Contains(string.Empty)).ToList();
				if (search == Searchable.Room)
					_items = ModelsList.items.Where(item => (item.Locations != null) ? item.Locations.Room.Contains(searchtext) : item.Name.Contains(string.Empty)).ToList();
			}
			RefreshData();
		} 

		private async Task OnScanBarCode()
		{
			Items items = new Items();
			await _capture.OpenCapture(new System.Threading.CancellationTokenSource());
			if (!_capture.IsCaptureOpen)
				return;
			var dialog = new BarCodeScannerDialog(_capture, _path);
			dialog.ShowModal();
			string name = _scanner.DecodeBarCode(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),".tmp", "InventBox", "Images", "Barcode.png"));
			if (string.IsNullOrEmpty(name)) {
				var faileddialog = MessageBox.Show("Failed to scan barcode", MessageBoxButtons.OK, MessageBoxType.Error);
				return;
			}
			_items = ModelsList.items.Where(item => item.Name == name).ToList();
			RefreshData();
		}

		public void OnCreate()
		{
			ItemModelView modelView = new ItemModelView(){Id = ModelsList.items.Count + 1, Conditions = Conditions.NA};
			var createItemDialog = new ItemsDialog(modelView, Mode.Create, item => ModelsList.items.Add(item), _path, _logger);
			createItemDialog.Closed += (sender, e) => RefreshData();
			createItemDialog.ShowModal();
			CreateDirectory(TmpDir);
			_dataManagement.Save(ModelsList.items, TmpPath);
			_items = ModelsList.items;
			Content = CreateDynamicLayout();
			RefreshData();
		}

		public Button AddButton(string text, int width, int height, Action eventHandler)
		{
			var command = new Command();
			command.Executed += (sender, eventArgs) => eventHandler();
			return new Button { Text = text, Width = width, Height = height, Command = command, Cursor = Cursors.Pointer};
		}

		public void OnSave()
		{
			_categoryManagement = new DataManagement<Category>(_path);
			_locationManagement = new DataManagement<Locations>(_path);
			Uri homeDir = new Uri(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile));	
			var saveDialog = new SaveFileDialog
			{
				Filters =
				{
					new FileFilter("CSV FIle", ".csv")
				},
				Directory = homeDir
			};
			saveDialog.ShowDialog(this);
			if (saveDialog.FileName != string.Empty && ModelsList.items.Count > 0) {
				if (!saveDialog.FileName.Contains(".csv"))
					saveDialog.FileName = saveDialog.FileName + ".csv";
				_dataManagement.Save(ModelsList.items, saveDialog.FileName);
				var categoryPath = Path.Combine(Path.GetDirectoryName(saveDialog.FileName), $"{saveDialog.FileName}-Category.csv");
				var locationPath = Path.Combine(Path.GetDirectoryName(saveDialog.FileName), $"{saveDialog.FileName}-Location.csv");
				_categoryManagement.Save(ModelsList.categories, categoryPath);
				File.Delete(TmpCategoryPath);
				_locationManagement.Save(ModelsList.locations, locationPath);
				File.Delete(TmpLocationPath);
				File.Delete(TmpPath);
			}
			else if (string.IsNullOrEmpty(saveDialog.FileName)) {
				saveDialog.Dispose();
				return;
			}
			else
				MessageBox.Show("Items list is empty. Please add one or load from file.", "Save failed.", MessageBoxButtons.OK, MessageBoxType.Information);
			saveDialog.Dispose();
		}

		public void OnLoad()
		{
			Uri path = new Uri(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile));		
			var loadDialog = new OpenFileDialog
			{
				Filters =
				{
					new FileFilter("CSV File", ".csv")		
				},
				Directory = path
			};
			loadDialog.ShowDialog(this);
			if (!loadDialog.FileName.Contains(".csv"))
				loadDialog.FileName = string.Empty;
			if (!string.IsNullOrEmpty(loadDialog.FileName)) {
				ModelsList.items = _dataManagement.Load(loadDialog.FileName, true);
				if (ModelsList.items.Count == 0)
				{
					MessageBox.Show("Invalid data. Please choose a different file", "Load failed.", MessageBoxButtons.OK, MessageBoxType.Information);
					return;
				}
				_items = ModelsList.items;
				foreach (var item in _items)
				{
					if (!ModelsList.categories.Contains(ModelsList.categories.Find(i => i.Id == item.Category.Id)) && item.Category.Id > 0 || item.Category != null)
						ModelsList.categories.Add(item.Category);
					if (!ModelsList.locations.Contains(ModelsList.locations.Find(i => i.Id == item.Locations.Id)) && item.Locations.Id > 0 || item.Locations != null)
						ModelsList.locations.Add(item.Locations);
				}
				Content = CreateDynamicLayout();
				RefreshData();
			} else
			{
				// MessageBox.Show("File name is empty.", "Load failed.", MessageBoxButtons.OK, MessageBoxType.Information);
			}
			loadDialog.Dispose();
		}

		public void OnEdit()
		{
			if (_items.Count <= 0) {
				MessageBox.Show("The list is empty. Please create one or load from file.", "Item not selected", MessageBoxButtons.OK, MessageBoxType.Information);
				return;
			}
			if (_grid.SelectedItem == null) {
				MessageBox.Show("Item have not been selected. Please select one", "Item not selected", MessageBoxButtons.OK, MessageBoxType.Information);
				return;
			}
			Items item = (Items)_grid.SelectedItem;
			var index = ModelsList.items.IndexOf(item);
			if (index < 0 )
				return;
			ItemModelView modelView = ModelViewCopy(item);
			var editItemDialog = new ItemsDialog(modelView, Mode.Edit, item => ModelsList.items[index] = item, _path, _logger);
			editItemDialog.Closed += (sender, e) => { 
				CreateDirectory(TmpDir);
				_dataManagement.Save(ModelsList.items, TmpPath);
				_items = ModelsList.items;
				RefreshData();			
			};
			editItemDialog.ShowModal();
		}

		public void OnDelete()
		{
			if (_items.Count <= 0) {
				MessageBox.Show("The list is empty. Please create one or load from file.", "Item not selected", MessageBoxButtons.OK, MessageBoxType.Information);
				return;
			}
			if (_grid.SelectedItem == null) {
				MessageBox.Show("Item have not been selected. Please select one", "Item not selected", MessageBoxButtons.OK, MessageBoxType.Information);
				return;
			}
			Items item = (Items)_grid.SelectedItem;
			var index = ModelsList.items.IndexOf(item);
			if (index < 0)
				return;
			var deleteDialog = MessageBox.Show("Are you sure to delete the selected item?", "Delete selected item", MessageBoxButtons.YesNo, MessageBoxType.Question, MessageBoxDefaultButton.Yes);
			if (deleteDialog != DialogResult.Yes)
				return;
			ModelsList.items.Remove(item);
			CreateDirectory(TmpDir);
			if (ModelsList.items.Count > 0)
				_dataManagement.Save(ModelsList.items, TmpPath);
			else
				File.Delete(TmpPath);
			_items = ModelsList.items;
			Content = CreateDynamicLayout();
			RefreshData();
		}

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
