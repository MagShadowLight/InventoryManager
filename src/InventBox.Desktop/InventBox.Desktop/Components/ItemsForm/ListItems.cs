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
		public ListItems(string path, FileLogger logger)
		{
			_path = path;
			_logger = logger;
			Title = "InventBox";
			Size = new Size(1000,1000);

			OpenFileDialog loadDialog = null;
			var loadCommand = LoadCommand(loadDialog);
			var tableContent = LoadData();
			var createDialog = new CreateItemsDialog(new ItemModelView(){Id = ModelsList.items.Count + 1}, _path, _logger);
			var createItemsButton = CreateItemsButtons(createDialog, loadCommand);



			var content = CreateDynamicLayout(tableContent, createItemsButton, loadCommand);

			Content = content;
			createDialog.Closed += (sender, e) =>
			{
				ReloadData(tableContent, content, createDialog, createItemsButton, loadCommand);
			};
			Closed += (sender, e) =>
			{
				Dispose();
			};
		}
		private DataManagement<Items> dataManagement = new DataManagement<Items>(_path);

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
			saveCommand.Executed += delegate
			{
				var saveDialog = new SaveFileDialog
				{
					Filters =
					{
						new FileFilter("CSV FIle", ".csv")
					}
				};
				dataManagement.Save(ModelsList.items, saveDialog.FileName);
				saveDialog.Dispose();
			};
			return saveCommand;
		}

		Command LoadCommand(OpenFileDialog loadDialog)
		{
			var loadCommand = new Command();
			loadCommand.Executed += delegate
			{
				loadDialog = new OpenFileDialog
				{
					Filters =
					{
						new FileFilter("CSV File", ".csv")		
					}
				};
				loadDialog.ShowDialog(this);
				ModelsList.items = dataManagement.Load(loadDialog.FileName);
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
