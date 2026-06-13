using System;
using Eto.Forms;
using Eto.Drawing;
using InventBox.Desktop.Components.ItemsForm;
using InventBox.Core;
using System.IO;
using InventBox.Desktop.Components.CategoryForm;
using System.Collections.Generic;
using InventBox.Desktop.Components.LocationForm;
using InventBox.Desktop.Utils;
using InventBox.Core.Models;
using InventBox.Desktop.ModelView;

namespace InventBox.Desktop
{	
	/// <summary>
	/// Represents the whole application UI.
	/// </summary>
	public partial class MainForm : Form
	{
		/// <summary>
		/// Set the properties for the whole application
		/// </summary>
		private static string TmpDir = Path.Combine(Path.GetTempPath(), "InventBox", "Data");
		private static string TmpItemPath = Path.Combine(TmpDir, "Items", "Data-Item-tmp.csv");
		private static string TmpCategoryPath = Path.Combine(TmpDir, "Category", "Data-Category-tmp.csv");
		private static string TmpLocationPath = Path.Combine(TmpDir, "Locations", "Data-Location-tmp.csv");
		private static string _path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".tmp", "InventBox", "Logs", $"{DateTime.Now.Month}-{DateTime.Now.Day}-{DateTime.Now.Year}_{DateTime.Now.Hour}:{DateTime.Now.Minute}-InventBox.log");
		private DataManagement<Items> _itemManagement = new DataManagement<Items>(_path);
		private DataManagement<Category> _categoryManagement = new DataManagement<Category>(_path);
		private DataManagement<Locations> _locationsManagement = new DataManagement<Locations>(_path);
		private bool IsFullScreen = false; 

		private static FileLogger _logger = new FileLogger();
		ListItems listItemsForm = null;
		ListCategories listCategories = null;
		ListLocations listLocations = null;
		List<Panel> panels;
		private AboutDialog aboutDialog;
		Control panel = null;
		AppUtils<object> appUtils;

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
		Command FullScreenCommand;
        Command quitCommand;
		Command aboutCommand;
		private Size currentSize = new Size(950, 850);


		Tutorial tutorial = new Tutorial(new Size(500,500),
			"MainDone.md",
			"HomeInventBox is the inventory management application where you can manage the inventory in your home. It include the items, categories, locations, and optional warrantly and insurance.",
			"Inventory section is where you can create, manage, and delete items inside the grid. In this section, you can search for the items by name, category, floor, and room via dropdown and scan the barcode from the camera.",
			"Category section is where you can create, manage, and delete category inside the grid. In this section, you can search the category by name",
			"Location section is similar to category section where you can create, manage, and delete location plus searching by room.",
			"All of those section have the options to save and load the data from the file."
			);
		/// <summary>
		/// Initialize a new instance for the application.
		/// </summary>
		public MainForm()
		{
			appUtils = new AppUtils<object>(_logger, _path);
			CreateLogFile();
			_logger.Logs($"Opening HomeInventory at {DateTime.Now}", _path);
			RecoverData();

			SizeChanged += (sender, e) => CreateMainApp();
			
			/// <summary>
			/// Create an About Dialog for the application
			/// </summary>
			aboutDialog = CreateAboutDialog();

			///<summary>
			/// Set the properties for the window.
			/// </summary>
			Title = "HomeInventBox";
			MinimumSize =  new Size(950, 850);
			Resizable = true;
			Content = CreateMainApp();
			CreateCommand();	

			// create menu
			Menu = CreateMenuBar();

			// create toolbar			
			// ToolBar = CreateToolbar();
			if (!File.Exists("MainDone.md"))
				ShowTutorial();
		}
        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
			currentSize = ClientSize; 
        }
		/// <summary>
		/// Recover the data if the application have crashed or close without saving.
		/// </summary>
		private void RecoverData()
		{	
			if (File.Exists(TmpItemPath) || File.Exists(TmpCategoryPath) || File.Exists(TmpLocationPath)) {
				_logger.Logs("Attempting to recover data.", _path);
				var recoverMessage = MessageBox.Show("InventBox will attempt to recover data.\nClick Ok to recover the data. Click Cancel to discard the data", MessageBoxButtons.OKCancel, MessageBoxType.Information, MessageBoxDefaultButton.OK);
				if (recoverMessage == DialogResult.Ok) {
					_logger.Logs("Recovering data.", _path);
					List<Items> items = new List<Items>();
					List<Category> categories = new List<Category>();
					List<Locations> locations = new List<Locations>();
					if (File.Exists(TmpCategoryPath))
						categories = _categoryManagement.Load(TmpCategoryPath);
					if (File.Exists(TmpLocationPath))
						locations = _locationsManagement.Load(TmpLocationPath);
					if (File.Exists(TmpItemPath))
						items = _itemManagement.Load(TmpItemPath, true);
					if (!ModelsList.categories.Equals(categories))
						ModelsList.categories = categories;
					if (!ModelsList.locations.Equals(locations))
						ModelsList.locations = locations;
					if (!ModelsList.items.Equals(items))
						ModelsList.items = items;
				} else
				{
					_logger.Logs("Discarding data.", _path);
					if (File.Exists(TmpItemPath))
						File.Delete(TmpItemPath);
					if (File.Exists(TmpCategoryPath))
						File.Delete(TmpCategoryPath);
					if (File.Exists(TmpLocationPath))
						File.Delete(TmpLocationPath);
				}
			}
		}
	/// <summary>
	/// Create the main panel to show user where to press.
	/// </summary>
	/// <returns>Dynamic Layout to display.</returns>
	private DynamicLayout CreateMainPanel()
	{
		_logger.Logs("Creating the main panel.", _path);
		return new DynamicLayout
		{
			Padding = 20,
			Rows =
			{
				new Label
				{
					Text = "Click the button on the left to open either inventory, category, or location list",
					TextAlignment = TextAlignment.Center,
					VerticalAlignment = VerticalAlignment.Center,
					Font = new Font(FontFamilies.Serif, 12.0f, FontStyle.Bold, FontDecoration.None),
				}
			}
		};
	}
	/// <summary>
	/// Show the tutorial dialog (Wall of text)
	/// </summary>
	async void ShowTutorial() {
		_logger.Logs("Showing tutorial for first time users.", _path);
		await tutorial.ShowModalAsync();
	}
	/// <summary>
	/// Create the log file for logging purpose.
	/// </summary>
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
		/// <summary>
		/// Create the commands for the whole application.
		/// </summary>
		private void CreateCommand()
		{
			_logger.Logs("Creating commands for the application", _path);
			listItemCommand = CreateCommand("Inventory", "List items", Application.Instance.CommonModifier | Keys.I);
			listItemCommand.Executed += (sender, e) => {
				CreateItemListPanel();
				CreateMainApp();
			};

			listCategoryCommand = CreateCommand("Category", "List categories", Application.Instance.CommonModifier | Keys.Shift | Keys.C);
			listCategoryCommand.Executed += (sender, e) =>
			{
				CreateCategoryListPanel();
				CreateMainApp();
			};
            listLocationCommand = CreateCommand("Locations", "List locations", Application.Instance.CommonModifier | Keys.L);
            listLocationCommand.Executed += (sender, e) =>
			{
				createLocationListPanel();
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
			FullScreenCommand = CreateCommand((!IsFullScreen ? "Enter full screen" : "Exit full screen"), (!IsFullScreen ? "Enter full screen" : "Exit full screen"), Keys.F11);
			FullScreenCommand.Executed += (sender, e) => FullScreen();

            quitCommand = CreateCommand("Quit", null, Application.Instance.CommonModifier | Keys.Q);
			quitCommand.Executed += (sender, e) => Application.Instance.Quit();

			aboutCommand = CreateCommand("About...");
			aboutCommand.Executed += (sender, e) => aboutDialog.ShowDialog(this);
			
			_logger.Logs("Commands created", _path);
		}
		/// <summary>
		/// Create the menu bar for the appplication.
		/// </summary>
		/// <returns>Menu bar to display.</returns>
		private MenuBar CreateMenuBar()
		{
			_logger.Logs("Creating menu bar", _path);
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
					new SubMenuItem { Text = "&File", Items = { listItemCommand, listCategoryCommand, listLocationCommand, FullScreenCommand } },
					itemSubMenuItem,
					categorySubMenuItem,
					locationSubMenuItem
					// new SubMenuItem { Text = "&View", Items = { /* commands/items */ } },
				},
				ApplicationItems =
				{
					// application (OS X) or file menu (others)
					// new ButtonMenuItem { Text = "&Preferences..." },
				},
				QuitItem = quitCommand,
				AboutItem = aboutCommand
			};
		}
		private void FullScreen()
		{
			if (!IsFullScreen) {
				Maximize();
				WindowStyle = WindowStyle.None;
				IsFullScreen = true;
				FullScreenCommand = CreateCommand((!IsFullScreen ? "Enter full screen" : "Exit full screen"), (!IsFullScreen ? "Enter full screen" : "Exit full screen"), Keys.F11);
				FullScreenCommand.Executed += (sender, e) => FullScreen();
				Menu = CreateMenuBar();
			} else
			{
				ClientSize = currentSize;
				WindowStyle = WindowStyle.Default;
				WindowState = WindowState.Normal;
				IsFullScreen = false;
				FullScreenCommand = CreateCommand((!IsFullScreen ? "Enter full screen" : "Exit full screen"), (!IsFullScreen ? "Enter full screen" : "Exit full screen"), Keys.F11);
				FullScreenCommand.Executed += (sender, e) => FullScreen();
				Menu = CreateMenuBar();
			}
		}
		/// <summary>
		/// Create a dialog to explain to users what the application is with credits.
		/// </summary>
		/// <returns></returns>
		private AboutDialog CreateAboutDialog()
		{
			_logger.Logs("Creating about application.", _path);
			var link = new Uri("https://github.com/MagShadowLight/InventoryManager");
			return new AboutDialog()
			{
				Developers = new [] {"MagShadowLight"},
				Documenters = new [] {"MagShadowLight"},
				ProgramName = "HomeInventBox",
				ProgramDescription = "HomeInventBox is a Inventory Management App where you can manage items in your own home.",
				Title = "HomeInventBox about",
				Version = "Version 1.0",
				WebsiteLabel = "Github",
				Website = link
			};
		}
		/// <summary>
		/// Create the dynamic layout for the whole application.
		/// </summary>
		/// <returns>Layout to display.</returns>
		private DynamicLayout CreateMainApp()
		{
			_logger.Logs("Creating whole application.", _path);
			var layout = new DynamicLayout
			{
				Padding = 10,
				DefaultSpacing = new Size(5,5)
			};
			ChangeActivePanel();
			layout.BeginHorizontal();
			layout.Add(NavigationButton());
			layout.Add(panel);
			layout.EndHorizontal();
			return layout;
		}
		/// <summary>
		/// Change the active panel depending on which button that was pressed.
		/// </summary>
        private void ChangeActivePanel()
        {
			_logger.Logs("Switching panel", _path);
			if (listItemsForm != null && listItemsForm.Visible)
				panel = listItemsForm;
			else if (listCategories != null && listCategories.Visible)
				panel = listCategories;
			else if (listLocations != null && listLocations.Visible)
				panel = listLocations;
			else 
				panel = CreateMainPanel();
        }
		/// <summary>
		/// Create the stack layout for navigation.
		/// </summary>
		/// <returns>Layout to display.</returns>
        private StackLayout NavigationButton()
		{
			_logger.Logs("Creating navigation button.", _path);
			var InventoryButton = appUtils.AddButton("Inventory", 100, 50, CreateItemListPanel);
			var CategoryButton = appUtils.AddButton("Category", 100, 50, CreateCategoryListPanel);
			var LocationButton = appUtils.AddButton("Locations", 100, 50, createLocationListPanel);
			
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
		/// <summary>
		/// Create a panel with list of items.
		/// </summary>
		private void CreateItemListPanel()
		{
			_logger.Logs("Opening the item list.", _path);
			if (listItemsForm != null)
				listItemsForm.Dispose();
			panels = new List<Panel>() {listCategories, listLocations};
			ClearOtherPanel(panels);
			listItemsForm = new ListItems(_path, _logger);
			listItemsForm.Visible = true;
			Menu = CreateMenuBar();
			Content = CreateMainApp();
			_logger.Logs("Item list opened", _path);
		}
		/// <summary>
		/// Create a panel with list of categories.
		/// </summary>
		private void CreateCategoryListPanel()
		{
			_logger.Logs("Opening the category list.", _path);
			if (listCategories != null)
				listCategories.Dispose();
			panels = new List<Panel>() {listItemsForm, listLocations};
			ClearOtherPanel(panels);
			listCategories = new ListCategories(_path, _logger);
			listCategories.Visible = true;
			Menu = CreateMenuBar();
			Content = CreateMainApp();
			_logger.Logs("Category list opened", _path);
		}
		/// <summary>
		/// Create a panel with list of locations.
		/// </summary>
		private void createLocationListPanel()
		{
			_logger.Logs("Opening the location list.", _path);
			if (listLocations != null)
				listLocations.Dispose();
			panels = new List<Panel>() {listItemsForm, listCategories};
			ClearOtherPanel(panels);
			listLocations = new ListLocations(_path, _logger);
			listLocations.Visible = true;
			Menu = CreateMenuBar();
			Content = CreateMainApp();
			_logger.Logs("Location list opened.", _path);
		}
		/// <summary>
		/// Clear the active panel before switching.
		/// </summary>
		/// <param name="panels">Panel to switch.</param>
		private void ClearOtherPanel(List<Panel> panels)
		{
			foreach (var panel in panels)
				if (panel != null)
					panel.Visible = false;
		}
		/// <summary>
		/// Create command for the button, menu bar, and other.
		/// </summary>
		/// <param name="menuText">The text for menu.</param>
		/// <param name="toolbarText">The text for toolbar.</param>
		/// <param name="shortcut">The key shortcut for the command.</param>
		/// <returns>Command for use.</returns>
		private Command CreateCommand(string menuText, string toolbarText = null, Keys shortcut = Keys.None)
		{
			return new Command() {MenuText = menuText, ToolBarText = toolbarText, Shortcut = shortcut};
		}
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
			_logger.Logs($"Closing HomeInventBox at {DateTime.Now}", _path);
        }
    }
}
