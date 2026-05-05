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
		
		private DataManagement<Items> dataManagement = new DataManagement<Items>("InventBox.log");
		public ListItems()
		{
			if (ModelsList.items.Count == 0)
				ModelsList.items = dataManagement.Load("Items.csv");
			Title = "InventBox";
			Size = new Size(1000,1000);

			
			var tableContent = LoadData();
			var createDialog = new CreateItemsDialog(new ItemModelView(){Id = ModelsList.items.Count + 1});
			var createItemsButton = CreateItemsButtons(createDialog);




			var content = CreateDynamicLayout(tableContent, createItemsButton);
			Content = content;
			createDialog.Closed += (sender, e) =>
			{
				tableContent = LoadData();
				content = CreateDynamicLayout(tableContent, createItemsButton);
				createDialog = new CreateItemsDialog(new ItemModelView(){Id = ModelsList.items.Count + 1, Conditions = Conditions.New});
				createItemsButton = CreateItemsButtons(createDialog);
				Content = content;
			};
			Closed += (sender, e) =>
			{
				Dispose();
			};
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
		DynamicLayout CreateDynamicLayout(TableLayout tableLayout, Command createItemsButton)
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
					null
				}
			);
			layout.EndVertical();
			return layout;
		}

		Command CreateItemsButtons(CreateItemsDialog createItemsDialog)
		{
			var button = new Command { MenuText = "Create item" };
			button.Executed += (sender, e) => {
				createItemsDialog.DataContext = new ItemModelView(){Id = ModelsList.items.Count + 1, Conditions = Conditions.New};
				createItemsDialog.ShowModal(this);
				Content = CreateDynamicLayout(LoadData(), button);
			};
			return button;
		}

		TableLayout LoadData()
		{
			var table = CreateItemsTable();
			GetItems(table);
			return table;
		}
	}
}
