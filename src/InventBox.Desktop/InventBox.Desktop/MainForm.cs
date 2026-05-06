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
		/// <summary>
		/// Set the properties for the whole application
		/// </summary>
		private static string _path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".tmp", "InventBox", $"{DateTime.Now.Month}-{DateTime.Now.Day}-{DateTime.Now.Year}_{DateTime.Now.Hour}:{DateTime.Now.Minute}-InventBox.log");
		private static FileLogger _logger = new FileLogger();
		ListItems listItemsForm = null;
		private AboutDialog aboutDialog;

		/// <summary>
		/// Create a commands
		/// </summary>
		Command listItemCommand;
		Command quitCommand;
		Command aboutCommand;


		
		public MainForm()
		{
			/// <summary>
			/// Create an About Dialog for the application
			/// </summary>
			aboutDialog = CreateAboutDialog();

			///<summary>
			/// Set the properties for the window.
			/// </summary>
			Title = "InventBox";
			MinimumSize = new Size(1500, 1250);
			Content = CreateMainApp();
			CreateCommand();	

			// create menu
			Menu = CreateMenuBar();

			// create toolbar			
			// ToolBar = CreateToolbar();
		}

		private void CreateCommand()
		{
			listItemCommand = CreateCommand("List items", "List items", Application.Instance.CommonModifier | Keys.I);
			listItemCommand.Executed += (sender, e) => {
				OnItemListPanel(1250, 1000);
				CreateMainApp();
			};

			quitCommand = CreateCommand("Quit", null, Application.Instance.CommonModifier | Keys.Q);
			quitCommand.Executed += (sender, e) => Application.Instance.Quit();

			aboutCommand = CreateCommand("About...");
			aboutCommand.Executed += (sender, e) => aboutDialog.ShowDialog(this);
		}

		private MenuBar CreateMenuBar()
		{
			return new MenuBar
			{
				Items =
				{
					// File submenu
					new SubMenuItem { Text = "&File", Items = { listItemCommand } },
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
		}

		private ToolBar CreateToolbar()
		{
			return new ToolBar { Items = { listItemCommand } };
		}

		private AboutDialog CreateAboutDialog()
		{
			var link = new Uri("https://github.com/MagShadowLight/InventoryManager");
			return new AboutDialog()
			{
				Developers = new [] {"MagShadowLight"},
				Documenters = new [] {"MagShadowLight"},
				ProgramName = "InventBox",
				ProgramDescription = "InventBox is a Inventory Management App where you can manage your own inventory.",
				Title = "InventBox about",
				Version = "Alpha 0.0.1",
				WebsiteLabel = "Github",
				Website = link
			};
		}
		private DynamicLayout CreateMainApp()
		{
			var layout = new DynamicLayout
			{
				Padding = 10,
				
			};
			layout.BeginHorizontal();
			layout.Add(NavigationButton());
			layout.AddColumn(listItemsForm, null);
			layout.AddSpace();
			layout.EndHorizontal();
			return layout;
		}

		private StackLayout NavigationButton()
		{
			return new StackLayout()
			{
				Padding = 5,
				Items =
				{
					AddButton("Inventory", 100, 50, OnItemListPanel),
				}
			};
		}

		private Button AddButton(string text, int width, int height, Action<int,int> eventHandler)
		{
			var command = new Command(){MenuText = text, ToolBarText = text};
			command.Executed += (sender, eventArgs) => eventHandler(1250,1000);
			return new Button { Text = text, Width = width, Height = height, Command = command};
		}

		private void OnItemListPanel(int width, int height)
		{
			if (listItemsForm != null)
				listItemsForm.Dispose();
			listItemsForm = new ListItems(_path, _logger, new Size(width,height));
			Content = CreateMainApp();
		}

		private Command CreateCommand(string menuText, string toolbarText = null, Keys shortcut = Keys.None)
		{
			return new Command() {MenuText = menuText, ToolBarText = toolbarText, Shortcut = shortcut};
		}
    }
}
