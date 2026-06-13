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
using InventBox.Desktop.Utils;

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
		private AppUtils<Locations> _utils;
		private Tutorial locationTutorial = new Tutorial(new Size(500,550),
		"LocationTutorialDone.md",
		"Location section is where you can create, manage, and delete places into or from the list.",
		"It include the id, floor, room, container, and coordinate with x and y.",
		"To create, click the 'Create new place' button. To edit or delete, you must select place from the list first then click either 'Edit selected place' or 'Delete selected place'. You can also save and load item locations from the list into a file. Click 'Save place' button to save and 'Load place' button to load.",
		"You can search through the place. To do so, type the room in search bar to filter and click the 'search' button"
		);

		private GridView _grid;
		/// <summary>
		/// Initialize a new instance for the panel.
		/// </summary>
		/// <param name="path">The path for the logger.</param>
		/// <param name="logger">The logger for logging purposes.</param>
		public ListLocations(string path, FileLogger logger)
		{
			_jsonParser = new JsonParser<Locations>(_logger, _path);
			_locations = ModelsList.locations;
			_path = path;
			_logger = logger;
			_dataManagement = new DataManagement<Locations>(_path);
			_utils = new AppUtils<Locations>(_logger, _path, _grid, _jsonParser);
			_grid = CreateGrid();
			RefreshData();
			Visible = false;
			Content = CreateDynamicLayout();
			if (!File.Exists("LocationTutorialDone.md"))
				ShowTutorial();
		}
		/// <summary>
		/// Copy the locations into the clipboard.
		/// </summary>
		public void OnCopy()
		{
			_logger.Logs("Copying data to clipboard.", _path);
			if (_grid.SelectedItem == null)
				return;
			Locations SelectedValues = (Locations)_grid.SelectedItem;
			var jsonItem = _jsonParser.ParseJson(SelectedValues);
			Clipboard.Instance.Clear();		
			Clipboard.Instance.Text = jsonItem;
			_logger.Logs("Data copied successfully", _path);
		}
		/// <summary>
		/// Show the tutorial dialog (Wall of text)
		/// </summary>
		async void ShowTutorial() {
			_logger.Logs("Showing item tutorial for first time users.", _path);
			await locationTutorial.ShowModalAsync();
		}
		/// <summary>
		/// Search the location by room.
		/// </summary>
        public void Search()
        {
			_logger.Logs($"Searching the room by {searchText}", _path);
			if (string.IsNullOrEmpty(searchText)) {
			if (_locations.Count == ModelsList.locations.Count)
					MessageBox.Show("Search bar is empty. Please type in the search bar", "Search", MessageBoxButtons.OK, MessageBoxType.Information);
				_locations = ModelsList.locations;
			}
			else
				_locations = ModelsList.locations.Where(location => location.Room.ToLower().Contains(searchText.ToLower())).ToList();
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
			layout.Add(_utils.AddButton("Search", 100, 50, () => Search()));
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
					_utils.AddButton("Create new place", 100, 50, OnCreate),
					_utils.AddButton("Edit selected place", 100, 50, OnEdit),
					_utils.AddButton("Delete selected place", 100, 50, OnDelete),
					null,
					_utils.AddButton("Save places", 100, 50, OnSave),
					_utils.AddButton("Load places", 100, 50, OnLoad)
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
					_utils.GetColumn("Id", l => l.Id.ToString()),
					_utils.GetColumn("Floor", l => l.Floor),
					_utils.GetColumn("Room", l => l.Room),
					_utils.GetColumn("Container", l => l.Container),
					_utils.GetColumn("X", l => l.X.ToString()),
					_utils.GetColumn("Y", l => l.Y.ToString())
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
			_logger.Logs("Creating the location.", _path);
			LocationsModelView modelView = new LocationsModelView() {Id = ModelsList.locations.Count + 1};
			var createLocationDialog = new LocationsDialog(modelView, Mode.Create, location => ModelsList.locations.Add(location), _path, _logger);
			createLocationDialog.Closed += (sender, e) => RefreshData();
			createLocationDialog.ShowModal();
			_utils.CreateDirectory(TmpDir);
			_dataManagement.Save(ModelsList.locations, TmpPath);
			_locations = ModelsList.locations;
			_logger.Logs("Location created successfully.", _path);
			Content = CreateDynamicLayout();
			RefreshData();
        }
		/// <summary>
		/// Delete the location from the list.
		/// </summary>
        public void OnDelete()
        {
			_logger.Logs("Deleting the location from the list", _path);
			_itemmanagement = new DataManagement<Items>(_path);
			if (_locations.Count <= 0) {
				_logger.Error("Deleting location failed. List is empty.", _path);
				MessageBox.Show("The list is empty. Please create one or load from file.", "Location not selected", MessageBoxButtons.OK, MessageBoxType.Information);
				return;
			}
			if (_grid.SelectedItem == null) {
				_logger.Error("Deleting location failed. Location is not selected.", _path);
				MessageBox.Show("Location have not been selected. Please select one", "Location not selected", MessageBoxButtons.OK, MessageBoxType.Information);
				return;
			}
			Locations location = (Locations)_grid.SelectedItem;
			var deleteDialog = MessageBox.Show("Are you sure to delete the selected location?", "Delete selected location", MessageBoxButtons.YesNo, MessageBoxType.Question, MessageBoxDefaultButton.Yes);
			if (deleteDialog != DialogResult.Yes) {
				_logger.Logs("Deleting location cancelled.", _path);
				return;
			}
			ModelsList.locations.Remove(location);
			foreach (var item in ModelsList.items)
			{
				if (item.Locations.Id == location.Id)
					item.Locations = null;
			}
			_utils.CreateDirectory(TmpDir);
			if (ModelsList.locations.Count > 0)
				_dataManagement.Save(ModelsList.locations, TmpPath);
			else
				File.Delete(TmpPath);
			_itemmanagement.Save(ModelsList.items, TmpItemPath);
			_locations = ModelsList.locations;
			_logger.Logs("Location deleted successfully.", _path);
			Content = CreateDynamicLayout();
			RefreshData();
        }
		/// <summary>
		/// Edit the location from the list.
		/// </summary>
        public void OnEdit()
        {
			_logger.Logs("Editing location.", _path);
			_itemmanagement = new DataManagement<Items>(_path);
			if (_locations.Count <= 0) {
				_logger.Error("Editing location failed. List is empty.", _path);
				MessageBox.Show("The list is empty. Please create one or load from file.", "Location not selected", MessageBoxButtons.OK, MessageBoxType.Information);
				return;
			}
			if (_grid.SelectedItem == null) {
				_logger.Error("Editing location failed. Location is not selected.", _path);
				MessageBox.Show("Location have not been selected. Please select one", "Location not selected", MessageBoxButtons.OK, MessageBoxType.Information);
				return;
			}
			Locations location = (Locations)_grid.SelectedItem;
			var index = ModelsList.locations.IndexOf(location);
			LocationsModelView modelView = ModelViewCopy(location);
			var editLocationDialog = new LocationsDialog(modelView, Mode.Edit, location => ModelsList.locations[index] = location, _path, _logger);
			editLocationDialog.Closed += (sender, e) =>
			{
				foreach (var item in ModelsList.items)
				{
					if (item.Locations.Id == location.Id)
						item.Locations = ModelsList.locations[index];
				}
				_utils.CreateDirectory(TmpDir);
				_dataManagement.Save(ModelsList.locations, TmpPath);
				_itemmanagement.Save(ModelsList.items, TmpItemPath);
				_locations = ModelsList.locations;
				_logger.Logs("Location edited successfully.", _path);
				RefreshData();
			};
			editLocationDialog.ShowModal();
        }
		/// <summary>
		/// Load the location data from the file.
		/// </summary>
        public void OnLoad()
        {
			_logger.Logs("Loading location from file.", _path);
			Uri homeDir = new Uri(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile));
			var loadDialog = new OpenFileDialog
			{
				Filters =
				{
					new FileFilter("CSV File", ".csv")
				},
				Directory = homeDir
			};
			var result = loadDialog.ShowDialog(this);
			if (result == DialogResult.Cancel) {
				_logger.Logs("Loading file cancelled.", _path);
				loadDialog.Dispose();
				return;
			}
			if (!loadDialog.FileName.Contains(".csv"))
				loadDialog.FileName = string.Empty;
			if (!string.IsNullOrEmpty(loadDialog.FileName))
			{
				_locations = ModelsList.locations = _dataManagement.Load(loadDialog.FileName);
				if (ModelsList.locations.Count == 0)
				{
					_logger.Error("Loading location failed. Data is invalid.", _path);
					MessageBox.Show("Invalid data. Please choose a different file", "Load failed.", MessageBoxButtons.OK, MessageBoxType.Information);
					return;
				}
				_logger.Logs("Location loaded successfully", _path);
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
			_logger.Logs("Saving location into file.", _path);
			Uri homeDir = new Uri(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile));
			var saveDialog = new SaveFileDialog
			{
				Filters =
				{
					new FileFilter("CSV File", ".csv")
				},
				Directory = homeDir
			};
			var result = saveDialog.ShowDialog(this);
			if (result == DialogResult.Cancel) {
				saveDialog.Dispose();
				return;
			}
			if (saveDialog.FileName != string.Empty && ModelsList.locations.Count > 0)
			{
				_dataManagement.Save(ModelsList.locations, saveDialog.FileName);
				File.Delete(TmpPath);
				_logger.Logs("Location saved successfully.", _path);
			}
			else if (string.IsNullOrEmpty(saveDialog.FileName)){
				_logger.Logs("Saving location cancelled.", _path);
				saveDialog.Dispose();
				return;
			}
			else {
				_logger.Error("Saving location failed. List is empty.", _path);
				MessageBox.Show("locations list is empty. Please add one or load from file.", "Save failed.", MessageBoxButtons.OK, MessageBoxType.Information);
			}
			saveDialog.Dispose();
        }
		/// <summary>
		/// Refresh the data for the grid.
		/// </summary>
        public void RefreshData()
        {
			_logger.Logs("Refreshing data.", _path);
			_grid.DataStore = _locations.ToArray();
        }
		/// <summary>
		/// Create the context menu for the grid.
		/// </summary>
		/// <returns>Context menu for grid with options.</returns>
        public ContextMenu CreateContextMenu()
        {
			_logger.Logs("Creating context menu.", _path);
			var CopyLocationCommand = _utils.CreateMenuItem("Copy location", OnCopy);
			var CreateLocationCommand = _utils.CreateMenuItem("Create new location", OnCreate);
			var UpdateLocationCommand = _utils.CreateMenuItem("Edit location", OnEdit);			
			var DeleteLocationCommand = _utils.CreateMenuItem("Delete location", OnDelete);			
			var SaveLocationCommand = _utils.CreateMenuItem("Save location", OnSave);			
			var LoadLocationCommand = _utils.CreateMenuItem("Load location", OnLoad);
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
    }
}
