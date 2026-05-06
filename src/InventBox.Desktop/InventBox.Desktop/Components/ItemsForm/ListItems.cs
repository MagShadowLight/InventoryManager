using Eto.Forms;
using Eto.Drawing;
using InventBox.Desktop.ModelView;
using System.Collections.Generic;
using InventBox.Core.Models;
using InventBox.Core;
using System.Linq;
using System;

namespace InventBox.Desktop.Components.ItemsForm
{
	public partial class ListItems : Form
	{
		private static string _path;
		private static FileLogger _logger;
		private DataManagement<Items> _dataManagement;
		private GridView _grid;
		public ListItems(string path, FileLogger logger)
		{
			_path = path;
			_logger = logger;
			_dataManagement = new DataManagement<Items>(_path);
			_grid = CreateGrid();
			Title = "InventBox";
			Size = new Size(1000,1000);

			_grid = CreateGrid();
			RefreashData();

			var content = CreateDynamicLayout();

			Content = content;
			Closed += (sender, e) =>
			{
				Dispose();
			};
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
					null
				}
			);
			layout.EndVertical();
			return layout;
		}

		private void OnCreate()
		{
			ItemModelView modelView = new ItemModelView(){Id = ModelsList.items.Count + 1, Conditions = Conditions.New};
			var createItemDialog = new CreateItemsDialog(modelView, Mode.Create, item => ModelsList.items.Add(item), _path, _logger);
			createItemDialog.Closed += (sender, e) => RefreashData();
			createItemDialog.ShowModal();
		}

		private Button AddButton(string text, int width, Action eventHandler)
		{
			var command = new Command();
			command.Executed += (sender, eventArgs) => eventHandler();
			return new Button { Text = text, Width = width, Command = command};
		}

		private void OnSave()
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

		private void OnLoad()
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
			if (loadDialog.CheckFileExists) {
				ModelsList.items = _dataManagement.Load(loadDialog.FileName);
				RefreashData();
			}
			loadDialog.Dispose();
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
