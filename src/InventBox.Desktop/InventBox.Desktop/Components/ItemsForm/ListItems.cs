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
		private Searchable search;
		private TextBox searchBar;
		private List<Items> _items = new List<Items>();
		private ImageCapture _capture = new ImageCapture();
		private static string _path;
		private BarCodeScanner _scanner;
		private static FileLogger _logger;
		private DataManagement<Items> _dataManagement;
		private GridView _grid;
		public ListItems(string path, FileLogger logger, Size size)
		{
			_items = ModelsList.items;
			_path = path;
			_logger = logger;
			_scanner = new BarCodeScanner(_path);
			_dataManagement = new DataManagement<Items>(_path);
			Size = size;

			_grid = CreateGrid();
			RefreshData();
			Visible = false;
			Content = CreateDynamicLayout();
		}

		public void RefreshData()
		{
			_grid.DataStore = _items.ToArray<Items>();
		}

        public GridView CreateGrid()
        {
			return new GridView()
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
					GetColumn("Category", i => i.Category.Name),
					GetColumn("Floor", i => i.Locations.Floor),
					GetColumn("Room", i => i.Locations.Room),
					GetColumn("Container", i => i.Locations.Container),
					GetColumn("Warrant", i => (i.Warrantly != null) ? i.Warrantly.Status.ToString() : "Not Warranted"),
					GetColumn("Warrant Provider", i => (i.Warrantly != null) ? i.Warrantly.Provider : ""),
					GetColumn("Warrant Contact #", i => (i.Warrantly != null) ? i.Warrantly.ContactNumber : "" ),
					GetColumn("Insured", i => (i.Insurance != null) ? i.Insurance.Insured.ToString() : "Not Insured"),
					GetColumn("Insurance Provider", i => (i.Insurance != null) ? i.Insurance.Provider : ""),
					GetColumn("Insurance Contact #", i => (i.Insurance != null) ? i.Insurance.ContactNumber : "")
				}	
			};
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
			searchBar = CreateSearchBar();
			EnumDropDown<Searchable> searchDropDown = CreateSearchDropDown();
			DynamicLayout layout = new DynamicLayout();
			layout.BeginVertical();
			layout.AddSeparateRow(null, searchDropDown, searchBar, AddButton("Clear Search", 100, 50, () => ClearFilter()), AddButton("Scan barcode", 100, 50, async () => await OnScanBarCode()));
			layout.Add(_grid, true, true);
			layout.AddSeparateRow(4, null, true, false,
				new [] { 
					AddButton("Create new item", 50, 50, OnCreate),
					AddButton("Edit selected item", 50, 50, OnEdit),
					AddButton("Delete", 50, 50, OnDelete),
					null,
					AddButton("Save Data", 50, 50, OnSave),
					AddButton("Load Data", 50, 50, OnLoad)
				}
			);
			layout.EndVertical();
			return layout;
		}
		private EnumDropDown<Searchable> CreateSearchDropDown()
		{
			var dropdown = new EnumDropDown<Searchable>();
			dropdown.SelectedValue = Searchable.Name;
			dropdown.SelectedValueChanged += (sender, e) =>
			{
				search = dropdown.SelectedValue;
			};
			return dropdown;
		}

		public TextBox CreateSearchBar()
		{
			TextBox textBox = new TextBox() {Text = "Search", Width = 500};
			textBox.TextBinding.BindDataContext((Items items) => items.Name);
			textBox.TextChanged += (sender, e) =>
			{
				if (string.IsNullOrEmpty(textBox.Text))
					_items = ModelsList.items;
				else {
					if (search == Searchable.Name)
						_items = ModelsList.items.Where(item => item.Name.Contains(textBox.Text)).ToList();
					if (search == Searchable.Category)
						_items = ModelsList.items.Where(item => item.Category.Name.Contains(textBox.Text)).ToList();
					if (search == Searchable.Floor)
						_items = ModelsList.items.Where(item => item.Locations.Floor.Contains(textBox.Text)).ToList();
					if (search == Searchable.Room)
						_items = ModelsList.items.Where(item => item.Locations.Room.Contains(textBox.Text)).ToList();
				}
				RefreshData();
			};
			return textBox;
		}

		public void ClearFilter()
		{
			_items = ModelsList.items;
			searchBar.Text = "";
			RefreshData();
		} 

		private async Task OnScanBarCode()
		{
			Items items = new Items();
			await _capture.OpenCapture(new System.Threading.CancellationTokenSource());
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
			ItemModelView modelView = new ItemModelView(){Id = ModelsList.items.Count + 1, Conditions = Conditions.New};
			var createItemDialog = new ItemsDialog(modelView, Mode.Create, item => ModelsList.items.Add(item), _path, _logger);
			createItemDialog.Closed += (sender, e) => RefreshData();
			createItemDialog.ShowModal();
			_items = ModelsList.items;
			RefreshData();
		}

		public Button AddButton(string text, int width, int height, Action eventHandler)
		{
			var command = new Command();
			command.Executed += (sender, eventArgs) => eventHandler();
			return new Button { Text = text, Width = width, Height = height, Command = command};
		}

		public void OnSave()
		{
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
			if (saveDialog.FileName != string.Empty)
				_dataManagement.Save(ModelsList.items, saveDialog.FileName);
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
			if (loadDialog.FileName != null) {
				ModelsList.items = _dataManagement.Load(loadDialog.FileName);
				_items = ModelsList.items;
				foreach (var item in _items)
				{
					if (!ModelsList.categories.Contains(item.Category))
						ModelsList.categories.Add(item.Category);
					if (!ModelsList.locations.Contains(item.Locations))
						ModelsList.locations.Add(item.Locations);
				}
				RefreshData();
			}
			loadDialog.Dispose();
		}

		public void OnEdit()
		{
			Items item = (Items)_grid.SelectedItem;
			var index = ModelsList.items.IndexOf(item);
			if (index < 0 )
				return;
			ItemModelView modelView = ModelViewCopy(item);
			var editItemDialog = new ItemsDialog(modelView, Mode.Edit, item => ModelsList.items[index] = item, _path, _logger);
			editItemDialog.Closed += (sender, e) => { 
				_items = ModelsList.items;
				RefreshData();			
			};
			editItemDialog.ShowModal();
		}

		public void OnDelete()
		{
			Items item = (Items)_grid.SelectedItem;
			var index = ModelsList.items.IndexOf(item);
			if (index < 0)
				return;
			var deleteDialog = MessageBox.Show("Are you sure to delete the selected item?", MessageBoxButtons.YesNo, MessageBoxType.Question, MessageBoxDefaultButton.Yes);
			if (deleteDialog != DialogResult.Yes)
				return;
			ModelsList.items.Remove(item);
			_items = ModelsList.items;
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
