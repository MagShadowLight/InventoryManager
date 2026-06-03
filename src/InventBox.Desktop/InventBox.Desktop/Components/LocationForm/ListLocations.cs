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
using EtoApp;
using InventBox.Desktop.Components.ItemsForm;

namespace InventBox.Desktop.Components.LocationForm
{
	public partial class ListLocations : Panel, IEventHandler, IControls<Locations, LocationsModelView>
	{
		private string searchText = "";
		private JsonParser<Locations> _jsonParser;
		private TextBox searchBar;
		private List<Locations> _locations = new List<Locations>();
		private static string _path;
		private FileLogger _logger;
		private DataManagement<Locations> _dataManagement;
		private GridView _grid;

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

        public Button AddButton(string text, int width, int height, Action eventHandler)
        {
			var command = new Command();
			command.Executed += (sender, eventArgs) => eventHandler();
			return new Button { Text = text, Width = width, Height = height, Command = command, Cursor = Cursors.Pointer };
        }

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

        public GridColumn GetColumn(string header, Func<Locations, string> data)
        {
			return new GridColumn
			{
				HeaderText = header,
				Editable = false,
				DataCell = GetData(data)	
			};
        }

        public TextBoxCell GetData(Func<Locations, string> data)
        {
			return new TextBoxCell
			{
				Binding = Binding.Delegate<Locations, string>(data, null)
			};
        }

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

        public void OnCreate()
        {
			LocationsModelView modelView = new LocationsModelView() {Id = ModelsList.locations.Count + 1};
			var createLocationDialog = new LocationsDialog(modelView, Mode.Create, location => ModelsList.locations.Add(location), _path, _logger);
			createLocationDialog.Closed += (sender, e) => RefreshData();
			createLocationDialog.ShowModal();
			_locations = ModelsList.locations;
			Content = CreateDynamicLayout();
			RefreshData();
        }

        public void OnDelete()
        {
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
			_locations = ModelsList.locations;
			Content = CreateDynamicLayout();
			RefreshData();
        }

        public void OnEdit()
        {
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
				_locations = ModelsList.locations;
				RefreshData();
			};
			editLocationDialog.ShowModal();
        }

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
			if (loadDialog.FileName != null)
			{
				_locations = ModelsList.locations = _dataManagement.Load(loadDialog.FileName);
				Content = CreateDynamicLayout();
				RefreshData();
			}
			loadDialog.Dispose();
        }

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
			if (saveDialog.FileName != string.Empty)
				_dataManagement.Save(ModelsList.locations, saveDialog.FileName);
			saveDialog.Dispose();
        }

        public void RefreshData()
        {
			_grid.DataStore = _locations.ToArray();
        }

        public void OnCopy()
        {
			Locations SelectedLocation = (Locations)_grid.SelectedItem;
			var jsonItem = _jsonParser.ParseJson(SelectedLocation);
			Clipboard.Instance.Clear();		
			Clipboard.Instance.Text = jsonItem;
        }

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

        public ButtonMenuItem CreateMenuItem(string text, Action clickHandler, Keys keys = Keys.None)
        {
			var menuItem = new ButtonMenuItem{Text = text, Shortcut = keys};
			menuItem.Click += (sender, eventArgs) => clickHandler();
			return menuItem;
        }
    }
}
