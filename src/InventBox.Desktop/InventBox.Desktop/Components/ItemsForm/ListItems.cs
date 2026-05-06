using Eto.Forms;
using Eto.Drawing;
using InventBox.Desktop.ModelView;
using InventBox.Core.Models;
using InventBox.Core;
using System.Linq;
using System;
using InventBox.Desktop.Interfaces;

namespace InventBox.Desktop.Components.ItemsForm
{
	public partial class ListItems : Panel, IEventHandler
	{
		private static string _path;
		private static FileLogger _logger;
		private DataManagement<Items> _dataManagement;
		private GridView _grid;
		public ListItems(string path, FileLogger logger, Size size)
		{
			_path = path;
			_logger = logger;
			_dataManagement = new DataManagement<Items>(_path);
			_grid = CreateGrid();
			Size = size;

			_grid = CreateGrid();
			RefreashData();

			var content = CreateDynamicLayout();

			Content = content;
		}

		void RefreashData()
		{
			_grid.DataStore = ModelsList.items.ToArray<Items>();
		}

        private GridView CreateGrid()
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
					GetColumn("Insured", i => i.Insured.ToString()),
					GetColumn("Notes", i => i.Notes),
					GetColumn("Conditions", i => i.Conditions.ToString())
				}	
			};
        }

		private GridColumn GetColumn(string header, Func<Items, string> data) {
			return new GridColumn
			{
				HeaderText = header,
				Editable = false,
				DataCell = GetData(data)
			};
		}

        private TextBoxCell GetData(Func<Items, string> data)
        {
			return new TextBoxCell
			{
				Binding = Binding.Delegate<Items, string>(data, null)
			};
        }


		DynamicLayout CreateDynamicLayout()
		{
			DynamicLayout layout = new DynamicLayout();
			layout.BeginVertical();
			layout.Add(_grid, true, true);
			layout.AddSeparateRow(4, null, true, false,
				new [] { 
					AddButton("Create new item", 50, OnCreate),
					AddButton("Save Data", 50, OnSave),
					AddButton("Load Data", 50, OnLoad),
					AddButton("Edit selected item", 50, OnEdit),
					AddButton("Delete", 50, OnDelete),
					null
				}
			);
			layout.EndVertical();
			return layout;
		}

		public void OnCreate()
		{
			ItemModelView modelView = new ItemModelView(){Id = ModelsList.items.Count + 1, Conditions = Conditions.New};
			var createItemDialog = new ItemsDialog(modelView, Mode.Create, item => ModelsList.items.Add(item), _path, _logger);
			createItemDialog.Closed += (sender, e) => RefreashData();
			createItemDialog.ShowModal();
		}

		private Button AddButton(string text, int width, Action eventHandler)
		{
			var command = new Command();
			command.Executed += (sender, eventArgs) => eventHandler();
			return new Button { Text = text, Width = width, Command = command};
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
				RefreashData();
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
			editItemDialog.Closed += (sender, e) => RefreashData();
			editItemDialog.ShowModal();
		}

		public void OnDelete()
		{
			Items item = (Items)_grid.SelectedItem;
			var index = ModelsList.items.IndexOf(item);
			if (index < 0)
				return;
			var deleteDialog = MessageBox.Show("Are you sure to delete the selected items", MessageBoxButtons.YesNo, MessageBoxType.Question, MessageBoxDefaultButton.Yes);
			if (deleteDialog != DialogResult.Yes)
				return;
			ModelsList.items.Remove(item);
			RefreashData();
		}

		private ItemModelView ModelViewCopy(Items item) {
			return new ItemModelView
			{
				Id = item.Id,
				Name = item.Name,
				Description = item.Description,
				Quantity = item.Quantity,
				SerialNumber = item.SerialNumber,
				ModelNumber = item.ModelNumber,
				Manufacturer = item.Manufacturer,
				Insured = item.Insured,
				Notes = item.Notes,
				CreatedAt = item.CreatedAt,
				UpdatedAt = item.UpdatedAt,
				Conditions = item.Conditions
			};
		}
	}
}
