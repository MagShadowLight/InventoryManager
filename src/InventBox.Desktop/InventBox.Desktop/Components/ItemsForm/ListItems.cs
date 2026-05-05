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
		public ListItems(string path, FileLogger logger)
		{
			_path = path;
			_logger = logger;
			_dataManagement = new DataManagement<Items>(_path);
			Title = "InventBox";
			Size = new Size(1000,1000);

			OpenFileDialog loadDialog = null;
			var tableContent = LoadData();
			var createDialog = new CreateItemsDialog(new ItemModelView(){Id = ModelsList.items.Count + 1}, _path, _logger);
			var loadCommand = LoadCommand(loadDialog, tableContent);
			var createItemsButton = CreateItemsButtons(createDialog, loadCommand);

			var content = CreateDynamicLayout(tableContent, createItemsButton, loadCommand);

			Content = content;
			loadCommand.Executed += delegate
			{
				if (ModelsList.items.Count >= 0)
					ReloadData(tableContent, content, createDialog, createItemsButton, loadCommand);
			};
			createDialog.Closed += (sender, e) =>
			{
				ReloadData(tableContent, content, createDialog, createItemsButton, loadCommand);
			};
			Closed += (sender, e) =>
			{
				Dispose();
			};
		}

        private void ReloadData(TableLayout tableContent, DynamicLayout content, CreateItemsDialog createDialog, Command createItemsButton, Command loadCommand)
        {
			tableContent = LoadData();
			content = CreateDynamicLayout(tableContent, createItemsButton, loadCommand);
			createDialog = new CreateItemsDialog(new ItemModelView(){Id = ModelsList.items.Count + 1, Conditions = Conditions.New}, _path, _logger);
			createItemsButton = CreateItemsButtons(createDialog, loadCommand);
			Content = content;
        }

        TableLayout CreateItemsTable()
		{
			return new TableLayout
			{
				Padding = 10,
				Spacing = new (4,4),
				Size = new Size(10,10),
				Rows =
				{
					new TableRow("Id",
					"Name",
					"Description",
					"Quantity",
					"Serial Number",
					"Model Number",
					"Manufacturer",
					"Insured",
					"Notes",
					"Conditions")
				}
			};
		}
		void GetItems(TableLayout tableLayout)
		{
			foreach (var item in ModelsList.items)
			{
				tableLayout.Rows.Add(
					new TableRow(
						item.Id.ToString(),
						item.Name,
						item.Description,
						item.Quantity.ToString(),
						item.SerialNumber,
						item.ModelNumber,
						item.Manufacturer,
						item.Insured.ToString(),
						item.Notes,
						item.Conditions.ToString()
					)
				);
			}
		}
		DynamicLayout CreateDynamicLayout(TableLayout tableLayout, Command createItemsButton, Command loadCommand)
		{
			DynamicLayout layout = new DynamicLayout();
			layout.BeginVertical();
			layout.Add(tableLayout, true, true);
			layout.AddSeparateRow(4, null, true, false,
					new [] { 
					new Button()
					{
						Command = createItemsButton,
						Text = "Create new item",
						Width = 50
					},
					SaveButton(),
					LoadButton(loadCommand),
					null
				}
			);
			layout.EndVertical();
			return layout;
		}

		Command CreateItemsButtons(CreateItemsDialog createItemsDialog, Command loadCommand)
		{
			var button = new Command { MenuText = "Create item" };
			button.Executed += (sender, e) => {
				createItemsDialog.DataContext = new ItemModelView(){Id = ModelsList.items.Count + 1, Conditions = Conditions.New};
				createItemsDialog.ShowModal(this);
				Content = CreateDynamicLayout(LoadData(), button, loadCommand);
			};
			return button;
		}

		Control LoadButton(Command loadCommand)
		{
			var button = new Button{ Text = "Load Data", Width = 50, Command = loadCommand };
			return button;
		}

		Control SaveButton()
		{
			var button = new Button{ Text = "Save Data", Width = 50, Command = SaveCommand() };
			return button;
		}

		Command SaveCommand()
		{
			var saveCommand = new Command();
			Uri path = new Uri(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile));		
			saveCommand.Executed += delegate
			{
				var saveDialog = new SaveFileDialog
				{
					Filters =
					{
						new FileFilter("CSV FIle", ".csv")
					},
					Directory = path
				};
				saveDialog.ShowDialog(this);
				if (saveDialog.FileName != string.Empty)
					_dataManagement.Save(ModelsList.items, saveDialog.FileName);
				saveDialog.Dispose();
			};
			return saveCommand;
		}

		Command LoadCommand(OpenFileDialog loadDialog, TableLayout tableLayout)
		{
			var loadCommand = new Command();
			Uri path = new Uri(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile));		
			loadCommand.Executed += delegate
			{
				loadDialog = new OpenFileDialog
				{
					Filters =
					{
						new FileFilter("CSV File", ".csv")		
					},
					Directory = path
				};
				loadDialog.ShowDialog(this);
				if (loadDialog.FileName != string.Empty)
					ModelsList.items = _dataManagement.Load(loadDialog.FileName);
				loadDialog.Dispose();
			};
			return loadCommand;
		}

		TableLayout LoadData()
		{
			var table = CreateItemsTable();
			GetItems(table);
			return table;
		}
	}
}
