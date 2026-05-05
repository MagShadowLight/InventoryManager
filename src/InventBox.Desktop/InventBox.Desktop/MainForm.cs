using System;
using Eto.Forms;
using Eto.Drawing;
using InventBox.Desktop.Components.ItemsForm;
using InventBox.Core;
using System.IO;

namespace InventBox.Desktop
{
	public partial class MainForm : Form
	{
		private static string _path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".tmp", "InventBox", $"{DateTime.Now.ToShortDateString()}-InventBox.log");
		private static FileLogger _logger = new FileLogger();
		
		public MainForm()
		{
			ListItems listItemsForm = new ListItems(_path, _logger);
			Title = "InventBox";
			MinimumSize = new Size(1000, 1000);
			
			Content = new StackLayout
			{
				Padding = 10,
				Items =
				{
					"Hello World!",
					// add more controls here
				}
			};

			// create a few commands that can be used for the menu and toolbar
			var clickMe = new Command { MenuText = "Click Me!", ToolBarText = "Click Me!" };
			clickMe.Executed += (sender, e) => MessageBox.Show(this, "I was clicked!");

			var listItemCommand = new Command {MenuText = "List Items", ToolBarText = "List items" };
			listItemCommand.Executed += (sender, e) => {
				if (listItemsForm.IsDisposed)
					listItemsForm = new ListItems(_path, _logger);
				listItemsForm.Show();
			};

			var quitCommand = new Command { MenuText = "Quit", Shortcut = Application.Instance.CommonModifier | Keys.Q };
			quitCommand.Executed += (sender, e) => Application.Instance.Quit();

			var aboutCommand = new Command { MenuText = "About..." };
			aboutCommand.Executed += (sender, e) => new AboutDialog().ShowDialog(this);

			// create menu
			Menu = new MenuBar
			{
				Items =
				{
					// File submenu
					new SubMenuItem { Text = "&File", Items = { clickMe, listItemCommand } },
					// new SubMenuItem { Text = "&Edit", Items = { /* commands/items */ } },
					// new SubMenuItem { Text = "&View", Items = { /* commands/items */ } },
				},
				ApplicationItems =
				{
					// application (OS X) or file menu (others)
					new ButtonMenuItem { Text = "&Preferences..." },
				},
				QuitItem = quitCommand,
				AboutItem = aboutCommand
			};

			// create toolbar			
			ToolBar = new ToolBar { Items = { clickMe, listItemCommand } };
		}

    }
}
