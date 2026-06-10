using System;
using Eto.Forms;
using Eto.Drawing;
using InventBox.Desktop.Interfaces;
using InventBox.Core.Models;
using InventBox.Desktop.ModelViews;
using System.Collections.Generic;
using InventBox.Core;
using InventBox.Desktop.ModelView;
using System.Linq;
using InventBox.Desktop.Components.ItemsForm;
using System.IO;

namespace InventBox.Desktop.Components.LocationForm
{
	/// <summary>
	/// Represents the panel for list of locations.
	/// </summary>
	public partial class ListLocations : Panel, IEventHandler, IControls<Locations, LocationsModelView>
	{
		private static string TmpDir = Path.Combine(Path.GetTempPath(), "InventBox", "Data", "Locations");
		private string TmpPath = Path.Combine(TmpDir, "Data-Location-tmp.csv");
		private static string TmpItemDir = Path.Combine(Path.GetTempPath(), "InventBox", "Data", "Items");
		private string TmpItemPath = Path.Combine(TmpItemDir, "Data-Item-tmp.csv");
		private string searchText = "";
		private JsonParser<Locations> _jsonParser;
		private TextBox searchBar;
		private List<Locations> _locations = new List<Locations>();
		private static string _path;
		private FileLogger _logger;
		private DataManagement<Locations> _dataManagement;
		private DataManagement<Items> _itemmanagement;

		private GridView _grid;
		/// <summary>
		/// Initialize a new instance for the panel.
		/// </summary>
		/// <param name="path">The path for the logger.</param>
		/// <param name="logger">The logger for logging purposes.</param>
		public ListLocations(string path, FileLogger logger)
		{
			_jsonParser = new JsonParser<Locations>();
			_locations = ModelsList.locations;
			_path = path;
			_logger = logger;
			_dataManagement = new DataManagement<Locations>(_path);

			_grid = CreateGrid();
			RefreshData();
			Visible = false;
			Content = CreateDynamicLayout();
		}
		/// <summary>
		/// Create a directory if it does not exists.
		/// </summary>
		/// <param name="dir">The path for the directory.</param>
		private void CreateDirectory(string dir)
		{
			if (!Directory.Exists(dir))
				Directory.CreateDirectory(dir);
		}
		/// <summary>
		/// Create a button for the panel.
		/// </summary>
		/// <param name="text">The text for the button.</param>
		/// <param name="width">The width for the button.</param>
		/// <param name="height">The height for the button.</param>
		/// <param name="eventHandler">The handler for clicking the button.</param>
		/// <returns>The button to display on panel.</returns>
        public Button AddButton(string text, int width, int height, Action eventHandler)
        {
			var command = new Command();
			command.Executed += (sender, eventArgs) => eventHandler();
			return new Button { Text = text, Width = width, Height = height, Command = command, Cursor = Cursors.Pointer };
        }
		/// <summary>
		/// Search the location by room.
		/// </summary>
        public void Search()
        {
			if (string.IsNullOrEmpty(searchText)) {
			if (_locations.Count == ModelsList.locations.Count)
					MessageBox.Show("Search bar is empty. Please type in the search bar", "Search", MessageBoxButtons.OK, MessageBoxType.Information);
				_locations = ModelsList.locations;
			}
			else
				_locations = ModelsList.locations.Where(location => location.Room.Contains(searchText)).ToList();
			RefreshData();
        }
		/// <summary>
		/// Create the layout for the panel.
		/// </summary>
		/// <returns>Layout to display.</returns>
        public DynamicLayout CreateDynamicLayout()
        {
			searchBar = CreateSearchBar();
			var layout = new DynamicLayout
			{
				Padding = 10				
			};
			layout.BeginVertical(null, null, true, true);
			layout.BeginVertical();
			layout.BeginHorizontal();
			layout.Add(searchBar, true, false);
			layout.Add(AddButton("Search", 100, 50, () => Search()));
			layout.EndVertical();
			layout.BeginVertical();
			layout.Add(((ModelsList.locations.Count > 0) ? _grid : new Label{
					Text = "The list is empty, Create new location or load from file to add it to the list.", 
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
				new []
				{
					AddButton("Create new location", 100, 50, OnCreate),
					AddButton("Edit selected location", 100, 50, OnEdit),
					AddButton("Delete selected locations", 100, 50, OnDelete),
					null,
					AddButton("Save Location", 100, 50, OnSave),
					AddButton("Load Location", 100, 50, OnLoad)
				}
			);
			layout.EndVertical();
			layout.EndVertical();
			return layout;
        }
		/// <summary>
		/// Create the grid for the panel.
		/// </summary>
		/// <returns>Grid for display.</returns>
        public GridView CreateGrid()
        {
			return new GridView()
			{
				ContextMenu = CreateContextMenu(),
				GridLines = GridLines.Both,
				AllowMultipleSelection = false,
				Columns =
				{
					GetColumn("Id", l => l.Id.ToString()),
					GetColumn("Floor", l => l.Floor),
					GetColumn("Room", l => l.Room),
					GetColumn("Container", l => l.Container),
					GetColumn("X", l => l.X.ToString()),
					GetColumn("Y", l => l.Y.ToString())
				}	
			};
        }
		/// <summary>
		/// Create the text box for searching.
		/// </summary>
		/// <returns>Text box for display</returns>
        public TextBox CreateSearchBar()
        {
			TextBox textBox = new TextBox() {Text = "Search", PlaceholderText = "Search rooms"};
			textBox.TextBinding.BindDataContext((Locations locations) => locations.Room);
			textBox.TextChanged += (sender, e) =>
			{
				searchText = textBox.Text;
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
		/// Create the column for the grid.
		/// </summary>
		/// <param name="header">The header text for the column.</param>
		/// <param name="data">The data for the column.</param>
		/// <returns>The column for grid to display.</returns>
        public GridColumn GetColumn(string header, Func<Locations, string> data)
        {
			return new GridColumn
			{
				HeaderText = header,
				Editable = false,
				DataCell = GetData(data)	
			};
        }
		/// <summary>
		/// Create the text box cell for the grid.
		/// </summary>
		/// <param name="data">The location data for the cell.</param>
		/// <returns>The text box cell for display.</returns>
        public TextBoxCell GetData(Func<Locations, string> data)
        {
			return new TextBoxCell
			{
				Binding = Binding.Delegate<Locations, string>(data, null)
			};
        }
		/// <summary>
		/// Copy the location data for model view.
		/// </summary>
		/// <param name="location">The data for the location.</param>
		/// <returns>Model view for location.</returns>
        public LocationsModelView ModelViewCopy(Locations location)
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
		/// Create the location with user input into the list.
		/// </summary>
        public void OnCreate()
        {
			LocationsModelView modelView = new LocationsModelView() {Id = ModelsList.locations.Count + 1};
			var createLocationDialog = new LocationsDialog(modelView, Mode.Create, location => ModelsList.locations.Add(location), _path, _logger);
			createLocationDialog.Closed += (sender, e) => RefreshData();
			createLocationDialog.ShowModal();
			CreateDirectory(TmpDir);
			_dataManagement.Save(ModelsList.locations, TmpPath);
			_locations = ModelsList.locations;
			Content = CreateDynamicLayout();
			RefreshData();
        }
		/// <summary>
		/// Delete the location from the list.
		/// </summary>
        public void OnDelete()
        {
			_itemmanagement = new DataManagement<Items>(_path);
			if (_locations.Count <= 0) {
				MessageBox.Show("The list is empty. Please create one or load from file.", "Location not selected", MessageBoxButtons.OK, MessageBoxType.Information);
				return;
			}
			if (_grid.SelectedItem == null) {
				MessageBox.Show("Location have not been selected. Please select one", "Location not selected", MessageBoxButtons.OK, MessageBoxType.Information);
				return;
			}
			Locations location = (Locations)_grid.SelectedItem;
			var index = ModelsList.locations.IndexOf(location);
			if (index < 0)
				return;
			var deleteDialog = MessageBox.Show("Are you sure to delete the selected location?", "Delete selected location", MessageBoxButtons.YesNo, MessageBoxType.Question, MessageBoxDefaultButton.Yes);
			if (deleteDialog != DialogResult.Yes)
				return;
			ModelsList.locations.Remove(location);
			foreach (var item in ModelsList.items)
			{
				if (item.Locations.Id == location.Id)
					item.Locations = null;
			}
			CreateDirectory(TmpDir);
			if (ModelsList.locations.Count > 0)
				_dataManagement.Save(ModelsList.locations, TmpPath);
			else
				File.Delete(TmpPath);
			_itemmanagement.Save(ModelsList.items, TmpItemPath);
			_locations = ModelsList.locations;
			Content = CreateDynamicLayout();
			RefreshData();
        }
		/// <summary>
		/// Edit the location from the list.
		/// </summary>
        public void OnEdit()
        {
			_itemmanagement = new DataManagement<Items>(_path);
			if (_locations.Count <= 0) {
				MessageBox.Show("The list is empty. Please create one or load from file.", "Location not selected", MessageBoxButtons.OK, MessageBoxType.Information);
				return;
			}
			if (_grid.SelectedItem == null) {
				MessageBox.Show("Location have not been selected. Please select one", "Location not selected", MessageBoxButtons.OK, MessageBoxType.Information);
				return;
			}
			Locations location = (Locations)_grid.SelectedItem;
			var index = ModelsList.locations.IndexOf(location);
			if (index < 0)
				return;
			LocationsModelView modelView = ModelViewCopy(location);
			var editLocationDialog = new LocationsDialog(modelView, Mode.Edit, location => ModelsList.locations[index] = location, _path, _logger);
			editLocationDialog.Closed += (sender, e) =>
			{
				foreach (var item in ModelsList.items)
				{
					if (item.Locations.Id == location.Id)
						item.Locations = ModelsList.locations[index];
				}
				CreateDirectory(TmpDir);
				_dataManagement.Save(ModelsList.locations, TmpPath);
				_itemmanagement.Save(ModelsList.items, TmpItemPath);
				_locations = ModelsList.locations;
				RefreshData();
			};
			editLocationDialog.ShowModal();
        }
		/// <summary>
		/// Load the location data from the file.
		/// </summary>
        public void OnLoad()
        {
			Uri homeDir = new Uri(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile));
			var loadDialog = new OpenFileDialog
			{
				Filters =
				{
					new FileFilter("CSV File", ".csv")
				},
				Directory = homeDir
			};
			loadDialog.ShowDialog(this);
			if (!loadDialog.FileName.Contains(".csv"))
				loadDialog.FileName = string.Empty;
			if (!string.IsNullOrEmpty(loadDialog.FileName))
			{
				_locations = ModelsList.locations = _dataManagement.Load(loadDialog.FileName);
				if (ModelsList.locations.Count == 0)
				{
					MessageBox.Show("Invalid data. Please choose a different file", "Load failed.", MessageBoxButtons.OK, MessageBoxType.Information);
					return;
				}
				Content = CreateDynamicLayout();
				RefreshData();
			}
			loadDialog.Dispose();
        }
		/// <summary>
		/// Save the data from the list into the file.
		/// </summary>
        public void OnSave()
        {
			Uri homeDir = new Uri(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile));
			var saveDialog = new SaveFileDialog
			{
				Filters =
				{
					new FileFilter("CSV File", ".csv")
				},
				Directory = homeDir
			};
			saveDialog.ShowDialog(this);
			if (saveDialog.FileName != string.Empty && ModelsList.locations.Count > 0)
			{
				_dataManagement.Save(ModelsList.locations, saveDialog.FileName);
				File.Delete(TmpPath);
			}
			else if (string.IsNullOrEmpty(saveDialog.FileName)){
				saveDialog.Dispose();
				return;
			}
			else
				MessageBox.Show("locations list is empty. Please add one or load from file.", "Save failed.", MessageBoxButtons.OK, MessageBoxType.Information);
			saveDialog.Dispose();
        }
		/// <summary>
		/// Refresh the data for the grid.
		/// </summary>
        public void RefreshData()
        {
			_grid.DataStore = _locations.ToArray();
        }
		/// <summary>
		/// copy the location data into the clipboard.
		/// </summary>
        public void OnCopy()
        {
			Locations SelectedLocation = (Locations)_grid.SelectedItem;
			var jsonItem = _jsonParser.ParseJson(SelectedLocation);
			Clipboard.Instance.Clear();		
			Clipboard.Instance.Text = jsonItem;
        }
		/// <summary>
		/// Create the context menu for the grid.
		/// </summary>
		/// <returns>Context menu for grid with options.</returns>
        public ContextMenu CreateContextMenu()
        {
			var CopyLocationCommand = CreateMenuItem("Copy location", OnCopy);
			var CreateLocationCommand = CreateMenuItem("Create new location", OnCreate);
			var UpdateLocationCommand = CreateMenuItem("Edit location", OnEdit);			
			var DeleteLocationCommand = CreateMenuItem("Delete location", OnDelete);			
			var SaveLocationCommand = CreateMenuItem("Save location", OnSave);			
			var LoadLocationCommand = CreateMenuItem("Load location", OnLoad);
			var EditMenu = new ButtonMenuItem
			{
				Text = "Edit",
				Items =
				{
					CreateLocationCommand,
					UpdateLocationCommand,
					DeleteLocationCommand,
					SaveLocationCommand,
					LoadLocationCommand
				}
			};
			return new ContextMenu
			{
				Items =
				{
					EditMenu,
					CopyLocationCommand
				},
			};
        }
		/// <summary>
		/// Create the menu item for the context menu.
		/// </summary>
		/// <param name="text">The text for the menu.</param>
		/// <param name="clickHandler">The handler for clicking the menu.</param>
		/// <param name="keys">The key shortcut for the menu.</param>
		/// <returns>The menu item for context menu with commands.</returns>
        public ButtonMenuItem CreateMenuItem(string text, Action clickHandler, Keys keys = Keys.None)
        {
			var menuItem = new ButtonMenuItem{Text = text, Shortcut = keys};
			menuItem.Click += (sender, eventArgs) => clickHandler();
			return menuItem;
        }
    }
}
