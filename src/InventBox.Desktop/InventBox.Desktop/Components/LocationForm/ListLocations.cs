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
		private List<Locations> _locations = new List<Locations>();
		private static string _path;
		private FileLogger _logger;
		private DataManagement<Locations> _dataManagement;
		private GridView _grid;

		public ListLocations(string path, FileLogger logger, Size size)
		{
			_locations = ModelsList.locations;
			_path = path;
			_logger = logger;
			_dataManagement = new DataManagement<Locations>(_path);
			Size = size;

			_grid = CreateGrid();
			RefreshData();
			Visible = false;
			Content = CreateDynamicLayout();
		}

        public Button AddButton(string text, int width, int height, Action eventHandler)
        {
			var command = new Command();
			command.Executed += (sender, eventArgs) => eventHandler();
			return new Button { Text = text, Width = width, Height = height, Command = command };
        }

        public void ClearFilter()
        {
			_locations = ModelsList.locations;
			RefreshData();
        }

        public DynamicLayout CreateDynamicLayout()
        {
			var layout = new DynamicLayout
			{
				Padding = 10				
			};
			layout.BeginVertical();
			layout.AddSeparateRow(null, CreateSearchBar(), AddButton("Clear filter", 100, 50, () => ClearFilter()));
			layout.Add(_grid, true, true);
			layout.AddSeparateRow(4, null, true, false,
				new []
				{
					AddButton("Create new location", 50, 50, OnCreate),
					AddButton("Save Location", 50, 50, OnSave),
					AddButton("Load Location", 50, 50, OnLoad),
					AddButton("Edit selected location", 50, 50, OnEdit),
					AddButton("Delete", 50, 50, OnDelete),
					null
				}
			);
			layout.EndVertical();
			return layout;
        }

        public GridView CreateGrid()
        {
			return new GridView()
			{
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
			TextBox textBox = new TextBox() {Text = "Search", Width = 500};
			textBox.TextBinding.BindDataContext((Locations locations) => locations.Room);
			textBox.TextChanged += (sender, e) =>
			{
				if (string.IsNullOrEmpty(textBox.Text))
					_locations = ModelsList.locations;
				else
					_locations = ModelsList.locations.Where(location => location.Room.Contains(textBox.Text)).ToList();
				RefreshData();
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
			LocationsModelView modelView = new LocationsModelView() {Id = ModelsList.locations.Count};
			var createLocationDialog = new LocationsDialog(modelView, Mode.Create, location => ModelsList.locations.Add(location), _path, _logger);
			createLocationDialog.Closed += (sender, e) => RefreshData();
			createLocationDialog.ShowModal();
			_locations = ModelsList.locations;
			RefreshData();
        }

        public void OnDelete()
        {
			Locations location = (Locations)_grid.SelectedItem;
			var index = ModelsList.locations.IndexOf(location);
			if (index < 0)
				return;
			var deleteDialog = MessageBox.Show("Are you sure to delete the selected location?", MessageBoxButtons.YesNo, MessageBoxType.Question, MessageBoxDefaultButton.Yes);
			if (deleteDialog != DialogResult.Yes)
				return;
			ModelsList.locations.Remove(location);
			_locations = ModelsList.locations;
			RefreshData();
        }

        public void OnEdit()
        {
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
    }
}
