using Eto.Forms;
using Eto.Drawing;
using InventBox.Desktop.Models;
using System.Collections.Generic;
using InventBox.Core.Models;
using InventBox.Core;

namespace InventBox.Desktop.Components.ItemsForm
{
	public partial class ListItems : Form
	{
		private DataManagement<Items> dataManagement = new DataManagement<Items>("InventBox.log");
		public ListItems()
		{
			ModelsList.items = dataManagement.Load("Items.csv");
			Title = "InventBox";
			Size = new Size(1000,1000);
			var createDialog = new CreateItemsDialog();

			var createItemsButton = new Command { MenuText = "Create item" };
			createItemsButton.Executed += (sender, e) => createDialog.ShowModal();
			
			var tableContent = new TableLayout
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
			foreach (var item in ModelsList.items)
			{
				tableContent.Rows.Add(
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
			var content = new DynamicLayout();
			content.BeginVertical();
			content.Add(tableContent, true, true);
			content.AddSeparateRow(4, null, true, false,
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
			content.EndVertical();
			Content = content;
		}		
	}
}
