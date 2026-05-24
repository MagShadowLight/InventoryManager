using System;
using Eto.Forms;
using Eto.Drawing;
using InventBox.Desktop.Components.ItemsForm;
using InventBox.Core;
using System.IO;
using InventBox.Desktop.Components.CategoryForm;
using System.Collections.Generic;
using InventBox.Desktop.Components.LocationForm;

namespace InventBox.Desktop
{
	public partial class MainForm : Form
	{
		/// <summary>
		/// Set the properties for the whole application
		/// </summary>
		private static string _path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".tmp", "InventBox", "Logs", $"{DateTime.Now.Month}-{DateTime.Now.Day}-{DateTime.Now.Year}_{DateTime.Now.Hour}:{DateTime.Now.Minute}-InventBox.log");
		private static FileLogger _logger = new FileLogger();
		ListItems listItemsForm = null;
		ListCategories listCategories = null;
		ListLocations listLocations = null;
		List<Panel> panels;
		private AboutDialog aboutDialog;

		/// <summary>
		/// Create commands variables
		/// </summary>
		Command listItemCommand;
		Command CreateItemCommand;
		Command UpdateItemCommand;
		Command DeleteItemCommand;
		Command SaveItemCommand;
		Command LoadItemCommand;
		Command listCategoryCommand;
        Command CreateCategoryCommand;
        Command UpdateCategoryCommand;
        Command DeleteCategoryCommand;
		Command SaveCategoryCommand;
		Command LoadCategoryCommand;
        Command listLocationCommand;
        Command CreateLocationCommand;
        Command UpdateLocationCommand;
        Command DeleteLocationCommand;
		Command SaveLocationCommand;
		Command LoadLocationCommand;
        Command quitCommand;
		Command aboutCommand;

		public MainForm()
		{
			
			CreateLogFile();
			/// <summary>
			/// Create an About Dialog for the application
			/// </summary>
			aboutDialog = CreateAboutDialog();

			///<summary>
			/// Set the properties for the window.
			/// </summary>
			Title = "InventBox";
			// MinimumSize = new Size(500, 500);
			MinimumSize = new Size(1500, 1000);
			Resizable = true;
			Content = CreateMainApp();
			CreateCommand();	

			// create menu
			Menu = CreateMenuBar();

			// create toolbar			
			// ToolBar = CreateToolbar();
		}
	private void CreateLogFile()
		{
			string[] path = new string[10];
			if (Platform.IsGtk)
				path = _path.Split("/");
			else if (Platform.IsWpf)
				path = _path.Split("\\");
			else if (Platform.IsMac)
				path = _path.Split(".");
			
			string temp = string.Empty;
			foreach (var test in path)
			{
				if (Platform.IsGtk)
					temp += Path.Combine(test + "/");
				else if (Platform.IsWpf)
					temp += Path.Combine(test + "\\");
				else if (Platform.IsMac)
					temp += Path.Combine(test + ".");
				if (test.Contains(".log"))
					return;
				if (!Directory.Exists(temp))
					Directory.CreateDirectory(temp);
			}
		}

		private void CreateCommand()
		{
			listItemCommand = CreateCommand("List items", "List items", Application.Instance.CommonModifier | Keys.I);
			listItemCommand.Executed += (sender, e) => {
				// OnItemListPanel(1250, 1000);
				OnItemListPanel(300, 300);
				CreateMainApp();
			};

			listCategoryCommand = CreateCommand("List categories", "List categories", Application.Instance.CommonModifier | Keys.C);
			listCategoryCommand.Executed += (sender, e) =>
			{
				// CreateCategoryListPanel(1250, 1000);
				CreateCategoryListPanel(300, 300);
				CreateMainApp();
			};
            listLocationCommand = CreateCommand("List locations", "List locations", Application.Instance.CommonModifier | Keys.L);
            listLocationCommand.Executed += (sender, e) =>
			{
				// createLocationListPanel(1250, 1000);
				CreateCategoryListPanel(300, 300);
				CreateMainApp();
			};

			CreateItemCommand = CreateCommand("Create new item", "Create new items");
			CreateItemCommand.Executed += (sender, e) =>
			{
				if (listItemsForm != null && listItemsForm.Visible)
					listItemsForm.OnCreate();
			};
			UpdateItemCommand = CreateCommand("Edit selected item", "Edit selected items");
			UpdateItemCommand.Executed += (sender, e) =>
			{
				if (listItemsForm != null && listItemsForm.Visible)
					listItemsForm.OnEdit();
			};
			DeleteItemCommand = CreateCommand("Delete selected item", "Delete Selected items");
			DeleteItemCommand.Executed += (sender, e) =>
			{
				if (listItemsForm != null && listItemsForm.Visible)
					listItemsForm.OnDelete();
			};
			SaveItemCommand = CreateCommand("Save Items", "Save Items");
			SaveItemCommand.Executed += (sender, e) =>
			{
				if (listItemsForm != null && listItemsForm.Visible)
					listItemsForm.OnSave();
			};
			LoadItemCommand = CreateCommand("Load Items", "Load Items");
			LoadItemCommand.Executed += (sender, e) =>
			{
				if (listItemsForm != null && listItemsForm.Visible)
					listItemsForm.OnLoad();
			};
            CreateCategoryCommand = CreateCommand("Create new category", "Create new category");
            CreateCategoryCommand.Executed += (sender, e) =>
            {
                if (listCategories != null && listCategories.Visible)
                    listCategories.OnCreate();
            };
            UpdateCategoryCommand = CreateCommand("Edit selected category", "Edit selected category");
            UpdateCategoryCommand.Executed += (sender, e) =>
            {
                if (listCategories != null && listCategories.Visible)
                    listCategories.OnEdit();
            };
            DeleteCategoryCommand = CreateCommand("Delete selected category", "Delete selected category");
            DeleteCategoryCommand.Executed += (sender, e) =>
            {
                if (listCategories != null && listCategories.Visible)
                    listCategories.OnDelete();
            };
            SaveCategoryCommand = CreateCommand("Save Category", "Save Category");
            SaveCategoryCommand.Executed += (sender, e) =>
            {
                if (listCategories != null && listCategories.Visible)
                    listCategories.OnSave();
            };
            LoadCategoryCommand = CreateCommand("Load Category", "Load Category");
            LoadCategoryCommand.Executed += (sender, e) =>
            {
                if (listCategories != null && listCategories.Visible)
                    listCategories.OnLoad();
            };
            CreateLocationCommand = CreateCommand("Create new location", "Create new location");
            CreateLocationCommand.Executed += (sender, e) =>
            {
                if (listLocations != null && listLocations.Visible)
                    listLocations.OnCreate();
            };
            UpdateLocationCommand = CreateCommand("Edit selected location", "Edit selected location");
            UpdateLocationCommand.Executed += (sender, e) =>
            {
                if (listLocations != null && listLocations.Visible)
                    listLocations.OnEdit();
            };
            DeleteLocationCommand = CreateCommand("Delete selected location", "Delete selected location");
            DeleteLocationCommand.Executed += (sender, e) =>
            {
                if (listLocations != null && listLocations.Visible)
                    listCategories.OnDelete();
            };
            SaveLocationCommand = CreateCommand("Save location", "Save location");
            SaveLocationCommand.Executed += (sender, e) =>
            {
                if (listLocations != null && listLocations.Visible)
                    listLocations.OnSave();
            };
            LoadLocationCommand = CreateCommand("Load location", "Load location");
            LoadLocationCommand.Executed += (sender, e) =>
            {
                if (listLocations != null && listLocations.Visible)
                    listLocations.OnLoad();
            };

            quitCommand = CreateCommand("Quit", null, Application.Instance.CommonModifier | Keys.Q);
			quitCommand.Executed += (sender, e) => Application.Instance.Quit();

			aboutCommand = CreateCommand("About...");
			aboutCommand.Executed += (sender, e) => aboutDialog.ShowDialog(this);
		}

		private MenuBar CreateMenuBar()
		{
			var itemSubMenuItem = new SubMenuItem { Text = "&Edit", Items = { CreateItemCommand, UpdateItemCommand, DeleteItemCommand, SaveItemCommand, LoadItemCommand }, Visible = false };
			var categorySubMenuItem = new SubMenuItem { Text = "&Edit", Items = { CreateCategoryCommand, UpdateCategoryCommand, DeleteCategoryCommand, SaveCategoryCommand, LoadCategoryCommand }, Visible = false };
			var locationSubMenuItem = new SubMenuItem { Text = "&Edit", Items = { CreateLocationCommand, UpdateLocationCommand, DeleteLocationCommand, SaveLocationCommand, LoadLocationCommand }, Visible = false };
			if (listItemsForm != null && listItemsForm.Visible == true)
				itemSubMenuItem.Visible = true;
			if (listCategories != null && listCategories.Visible == true)
				categorySubMenuItem.Visible = true;
			if (listLocations != null && listLocations.Visible == true)
				locationSubMenuItem.Visible = true;
			return new MenuBar
			{
				Items =
				{
					// File submenu
					new SubMenuItem { Text = "&File", Items = { listItemCommand, listCategoryCommand, listLocationCommand } },
					itemSubMenuItem,
					categorySubMenuItem,
					locationSubMenuItem
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
			layout.Add(listItemsForm);
			layout.Add(listCategories);
			layout.Add(listLocations);
			layout.AddSpace();
			layout.EndHorizontal();
			return layout;
		}

		private StackLayout NavigationButton()
		{
			var InventoryButton = AddButton("Inventory", 50, 50, OnItemListPanel);
			var CategoryButton = AddButton("Category", 50, 50, CreateCategoryListPanel);
			var LocationButton = AddButton("Locations", 50, 50, createLocationListPanel);
			return new StackLayout()
			{
				Padding = 5,
				Items =
				{
					InventoryButton,
					CategoryButton,
					LocationButton
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
			panels = new List<Panel>() {listCategories, listLocations};
			ClearOtherPanel(panels);
			listItemsForm = new ListItems(_path, _logger, new Size(width,height));
			listItemsForm.Visible = true;
			Menu = CreateMenuBar();
			Content = CreateMainApp();
		}

		private void CreateCategoryListPanel(int width, int height)
		{
			if (listCategories != null)
				listCategories.Dispose();
			panels = new List<Panel>() {listItemsForm, listLocations};
			ClearOtherPanel(panels);
			listCategories = new ListCategories(_path, _logger, new Size(width, height));
			listCategories.Visible = true;
			Menu = CreateMenuBar();
			Content = CreateMainApp();
		}
		private void createLocationListPanel(int width, int height)
		{
			if (listLocations != null)
				listLocations.Dispose();
			panels = new List<Panel>() {listItemsForm, listCategories};
			ClearOtherPanel(panels);
			listLocations = new ListLocations(_path, _logger, new Size(width, height));
			listLocations.Visible = true;
			Menu = CreateMenuBar();
			Content = CreateMainApp();
		}
		private void ClearOtherPanel(List<Panel> panels)
		{
			foreach (var panel in panels)
				if (panel != null)
					panel.Visible = false;
		}
		private Command CreateCommand(string menuText, string toolbarText = null, Keys shortcut = Keys.None)
		{
			return new Command() {MenuText = menuText, ToolBarText = toolbarText, Shortcut = shortcut};
		}
    }
}
