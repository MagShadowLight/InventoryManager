using System;
using Eto.Forms;
using InventBox.Desktop.Interfaces;
using InventBox.Core.Models;
using InventBox.Desktop.ModelViews;
using System.Collections.Generic;
using InventBox.Core;
using Eto.Drawing;
using System.Linq;
using InventBox.Desktop.Components.ItemsForm;
using System.IO;
using InventBox.Desktop.Utils;

namespace InventBox.Desktop.Components.CategoryForm
{
	/// <summary>
	/// Represents a panel for list of categories.
	/// </summary>
	public partial class ListCategories : Panel, IEventHandler, IControls<Category, CategoryModelView>
	{
		private static string TmpDir = Path.Combine(Path.GetTempPath(), "InventBox", "Data", "Category");
		private AppUtils<Category> _utils;
		private string TmpPath = Path.Combine(TmpDir, "Data-Category-tmp.csv");
		private static string TmpItemDir = Path.Combine(Path.GetTempPath(), "InventBox", "Data", "Items");
		private string TmpItemPath = Path.Combine(TmpItemDir, "Data-Item-tmp.csv");

		private JsonParser<Category> jsonParser;
		private TextBox searchBar;
		private List<Category> _categories = new List<Category>();
		private static string _path;
		private static FileLogger _logger;
		private DataManagement<Category> _datamanagement;
		private DataManagement<Items> _itemmanagement;
		private GridView _grid;
		private string searchText = "";
		private Tutorial categoryTutorial = new Tutorial(new Size(500,550),
		"CategoryTutorialDone.md",
		"Category section is where you can create, manage, and delete category into or from the list.",
		"It include the category id, name, and description.",
		"To create, click the 'Create new category' button. To edit or delete, you must select category from the list first then click either 'Edit selected category' or 'Delete selected category'. You can also save and load category from the list into a file. Click 'Save Category' button to save and 'Load Category' button to load.",
		"You can search through the category. To do so, type in the search bar to filter and click the 'search' button"
		);
		/// <summary>
		/// Initialize a new instance for the panel.
		/// </summary>
		/// <param name="path"></param>
		/// <param name="logger"></param>
		public ListCategories(string path, FileLogger logger)
		{
			_path = path;
			_logger = logger;
			jsonParser = new JsonParser<Category>(_logger, _path);
			_categories = ModelsList.categories;
			_datamanagement = new DataManagement<Category>(_path);
			_utils = new AppUtils<Category>(_logger, _path, _grid, jsonParser);
			_grid = CreateGrid();
			RefreshData();
			Visible = false;
			Content = CreateDynamicLayout();
			if (!File.Exists("CategoryTutorialDone.md"))
				ShowTutorial();
		}
		/// <summary>
		/// Copy the items into the clipboard.
		/// </summary>
		public void OnCopy()
		{
			_logger.Logs("Copying data to clipboard.", _path);
			if (_grid.SelectedItem == null)
				return;
			Category SelectedValues = (Category)_grid.SelectedItem;
			var jsonItem = jsonParser.ParseJson(SelectedValues);
			Clipboard.Instance.Clear();		
			Clipboard.Instance.Text = jsonItem;
			_logger.Logs("Data copied successfully", _path);
		}
		/// <summary>
		/// Show the tutorial dialog (Wall of text)
		/// </summary>
		async void ShowTutorial() {
			_logger.Logs("Showing item tutorial for first time users.", _path);
			await categoryTutorial.ShowModalAsync();
		}
		/// <summary>
		/// Create the context menu for the grid.
		/// </summary>
		/// <returns>The context menu with options.</returns>
		public ContextMenu CreateContextMenu()
		{
			_logger.Logs("Creating context menu", _path);
			var CopyCategoryCommand = _utils.CreateMenuItem("Copy category", OnCopy);
			var CreateCategoryCommand = _utils.CreateMenuItem("Create new category", OnCreate);
			var UpdateCategoryCommand = _utils.CreateMenuItem("Edit category", OnEdit);			
			var DeleteCategoryCommand = _utils.CreateMenuItem("Delete category", OnDelete);			
			var SaveCategoryCommand = _utils.CreateMenuItem("Save category", OnSave);			
			var LoadCategoryCommand = _utils.CreateMenuItem("Load category", OnLoad);
			var EditMenu = new ButtonMenuItem
			{
				Text = "Edit",
				Items =
				{
					CreateCategoryCommand,
					UpdateCategoryCommand,
					DeleteCategoryCommand,
					SaveCategoryCommand,
					LoadCategoryCommand
				}
			};
			return new ContextMenu
			{
				Items =
				{
					EditMenu,
					CopyCategoryCommand
				},
			};
		}
		/// <summary>
		/// Search the category by name.
		/// </summary>
        public void Search()
        {
			_logger.Logs($"Searching category by {searchText}", _path);
			if (ModelsList.categories.Count <= 0) {
				MessageBox.Show("The category list is empty. Please add one to search.", "Search", MessageBoxButtons.OK, MessageBoxType.Information);
				return;
			}
			if (string.IsNullOrEmpty(searchText)) {				
				if (_categories.Count == ModelsList.categories.Count)
					MessageBox.Show("Search bar is empty. Please type in the search bar", "Search", MessageBoxButtons.OK, MessageBoxType.Information);
				_categories = ModelsList.categories;
			}
			else {
				_categories = ModelsList.categories.Where(category => category.Name.ToLower().Contains(searchText.ToLower())).ToList();
			}
			RefreshData();
        }
		/// <summary>
		/// Create the layout for the panel.
		/// </summary>
		/// <returns>The layout for the category list.</returns>
        public DynamicLayout CreateDynamicLayout()
        {
			_logger.Logs("Creating layout for category.", _path);
			searchBar = CreateSearchBar();
			DynamicLayout layout = new DynamicLayout()
			{
				Padding = 10
			};
			layout.BeginVertical(null, null, true, true);
			layout.BeginVertical();
			layout.BeginHorizontal();
			layout.Add(searchBar, true);
			layout.Add(_utils.AddButton("Search", 100, 50, () => Search()));
			layout.EndHorizontal();
			layout.EndVertical();
			// layout.AddSeparateRow(null, searchBar, AddButton("Clear Search", 100, 50, () => ClearFilter()));
			layout.BeginVertical();
			layout.Add(((ModelsList.categories.Count > 0) 
			? _grid 
			: new Label{
			Text = "The list is empty, Create new category or load from file to add it to the list.", 
			TextAlignment = TextAlignment.Center, 
			VerticalAlignment = VerticalAlignment.Center, 
			Font = new Font(
				FontFamilies.Serif, 
				12.0f, 
				FontStyle.Bold, 
				FontDecoration.None)
			}), true, true);
			layout.Add(null, true, false);
			layout.BeginHorizontal(false);
			layout.AddSeparateRow(4, null, false, false, new []
				{
					_utils.AddButton("Create new category", 100, 50, OnCreate),
					_utils.AddButton("Edit selected category", 100, 50, OnEdit),
					_utils.AddButton("Delete selected category", 100, 50, OnDelete),
					null,
					_utils.AddButton("Save Category", 100, 50, OnSave),
					_utils.AddButton("Load Category", 100, 50, OnLoad)
				}
			);
			layout.EndHorizontal();
			layout.EndVertical();
			layout.EndVertical();
			return layout;
        }
		/// <summary>
		/// Create the grid view for category list.
		/// </summary>
		/// <returns>The grid for viewed.</returns>
        public GridView CreateGrid()
        {
			return new GridView()
			{
				ContextMenu = CreateContextMenu(),
				GridLines = GridLines.Both,
				AllowMultipleSelection = false,
				Columns =
				{
					_utils.GetColumn("Id", c => c.Id.ToString()),
					_utils.GetColumn("Name", c => c.Name),
					_utils.GetColumn("Description", c => c.Description)
				}
			};
        }
		/// <summary>
		/// Create the search bar for searching the category name.
		/// </summary>
		/// <returns>The text box for searching.</returns>
        public TextBox CreateSearchBar()
        {
			TextBox textBox = new TextBox()
			{
				Text = "Search",
				PlaceholderText = "Search category name"
			};
			textBox.TextBinding.BindDataContext((Category category) => category.Name);
			textBox.TextChanged += (sender, e) =>
			{
				searchText = textBox.Text;
			};
			textBox.KeyDown += (sender, e) =>
			{
				if (e.Key == Keys.Enter)
				{
					Search();
				}
			};
			return textBox;
        }
		/// <summary>
		/// Copy the category into the model view.
		/// </summary>
		/// <param name="category">The data from the category.</param>
		/// <returns>Model view for the category.</returns>
        public CategoryModelView ModelViewCopy(Category category)
        {
			return new CategoryModelView
			{
				Id = category.Id,
				Name = category.Name,
				Description = category.Description
			};
        }
		/// <summary>
		/// Create the category into the list.
		/// </summary>
        public void OnCreate()
        {
			_logger.Logs("Creating category.", _path);
			CategoryModelView modelView = new CategoryModelView(){Id = ModelsList.categories.Count + 1};
			var createCategoryDialog = new CategoryDialog(modelView, Mode.Create, category => ModelsList.categories.Add(category), _path, _logger);
			createCategoryDialog.Closed += (sender, e) => RefreshData();
			createCategoryDialog.ShowModal();
			_utils.CreateDirectory(TmpDir);
			_datamanagement.Save(ModelsList.categories, TmpPath);
			_categories = ModelsList.categories;
			_logger.Logs("Category created.", _path);
			Content = CreateDynamicLayout();
			RefreshData();
        }
		/// <summary>
		/// Delete the category from the list.
		/// </summary>
        public void OnDelete()
        {
			_logger.Logs("Deleting category from list.", _path);
			_itemmanagement = new DataManagement<Items>(_path);
			if (_categories.Count <= 0) {
				_logger.Error("Deleting category failed. List is empty.", _path);
				MessageBox.Show("The list is empty. Please create one or load from file.", "Category not selected", MessageBoxButtons.OK, MessageBoxType.Information);
				return;
			}
			if (_grid.SelectedItem == null) {
				_logger.Error("Deleting category failed. Category is not selected.", _path);
				MessageBox.Show("Category have not been selected. Please select one", "Category not selected", MessageBoxButtons.OK, MessageBoxType.Information);
				return;
			}
			Category category = (Category)_grid.SelectedItem;
			var deleteDialog = MessageBox.Show("Are you sure to delete the selected category?", "Delete selected category", MessageBoxButtons.YesNo, MessageBoxType.Information, MessageBoxDefaultButton.Yes);
			if (deleteDialog != DialogResult.Yes)
				return;
			ModelsList.categories.Remove(category);
			foreach (var item in ModelsList.items)
			{
				if (item.Category.Id == category.Id)
					item.Category = null;
			}
			_utils.CreateDirectory(TmpDir);
			if (ModelsList.categories.Count > 0) 
				_datamanagement.Save(ModelsList.categories, TmpPath);
			else
				File.Delete(TmpPath);
			_itemmanagement.Save(ModelsList.items, TmpItemPath);
			_categories = ModelsList.categories;
			_logger.Logs("Category deleted successfully.", _path);
			Content = CreateDynamicLayout();
			RefreshData();
        }
		/// <summary>
		/// Edit the category and replace the old data to the new data.
		/// </summary>
        public void OnEdit()
        {
			_logger.Logs("Editing category.", _path);
			_itemmanagement = new DataManagement<Items>(_path);
			if (_categories.Count <= 0) {
				_logger.Error("Editing category failed. List is empty.", _path);
				MessageBox.Show("The list is empty. Please create one or load from file.", "Item not selected", MessageBoxButtons.OK, MessageBoxType.Information);
				return;
			}
			if (_grid.SelectedItem == null) {
				_logger.Error("Editing category failed. Category is not selected.", _path);
				MessageBox.Show("Category have not been selected. Please select one", "Item not selected", MessageBoxButtons.OK, MessageBoxType.Information);
				return;
			}
			Category category = (Category)_grid.SelectedItem;
			var index = ModelsList.categories.IndexOf(category);
			CategoryModelView modelView = ModelViewCopy(category);
			var categories = ModelsList.categories;
			var editCategoryDialog = new CategoryDialog(modelView, Mode.Edit, category => ModelsList.categories[index] = category, _path, _logger);
			editCategoryDialog.Closed += (sender, e) =>
			{
				foreach (var item in ModelsList.items)
				{
					if (item.Category.Id == category.Id)
						item.Category = categories[index];
				}
				_utils.CreateDirectory(TmpDir);
				_datamanagement.Save(ModelsList.categories, TmpPath);
				_itemmanagement.Save(ModelsList.items, TmpItemPath);
				_categories = ModelsList.categories;
				_logger.Logs("Category edited successfully.", _path);
				RefreshData();
			};
			editCategoryDialog.ShowModal();
        }
		/// <summary>
		/// Load the data from the file.
		/// </summary>
        public void OnLoad()
        {
			_logger.Logs("Loading category from file.", _path);
			Uri path = new Uri(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile));
			var loadDialog = new OpenFileDialog
			{
				Filters =
				{
					new FileFilter("CSV File", ".csv")
				},
				Directory = path
			};
			var result = loadDialog.ShowDialog(this);
			if (result == DialogResult.Cancel) {
				_logger.Logs("Loading data cancelled.", _path);
				loadDialog.Dispose();
				return;
			}
			if (!loadDialog.FileName.Contains(".csv"))
				loadDialog.FileName = string.Empty;
			if (!string.IsNullOrEmpty(loadDialog.FileName))
			{
				ModelsList.categories = _datamanagement.Load(loadDialog.FileName);
				if (ModelsList.categories.Count == 0)
				{
					_logger.Error("Loading category failed. Invalid data.", _path);
					MessageBox.Show("Invalid data. Please choose a different file", "Load failed.", MessageBoxButtons.OK, MessageBoxType.Information);
					return;
				}
				_categories = ModelsList.categories;
				_logger.Logs("Category loaded successfully.", _path);
				Content = CreateDynamicLayout();
				RefreshData();
			}
			loadDialog.Dispose();
        }
		/// <summary>
		/// Save the data from the list.
		/// </summary>
        public void OnSave()
        {
			_logger.Logs("Saving the category to file", _path);
			if (ModelsList.categories.Count <= 0) {
				_logger.Error("Saving category failed. Category List is empty.", _path);
				MessageBox.Show("categories list is empty. Please add one or load from file.", "Save failed.", MessageBoxButtons.OK, MessageBoxType.Information);
				return;
			}
			Uri homeDir = new Uri(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile));
			var saveDialog = new SaveFileDialog
			{
				Filters =
				{
					new FileFilter("CSV File", ".csv")
				},
				Directory = homeDir
			};
			var result = saveDialog.ShowDialog(this);
			if (result == DialogResult.Cancel) {
				saveDialog.Dispose();
				return;
			}
			if (saveDialog.FileName != string.Empty  && ModelsList.categories.Count > 0) {
				var fileName = saveDialog.FileName + saveDialog.Filters[0].Extensions[0];
				_datamanagement.Save(ModelsList.categories, fileName);
				File.Delete(TmpPath);
				_logger.Logs("Category saved successfully.", _path);
			}
			else if (string.IsNullOrEmpty(saveDialog.FileName)){
				saveDialog.Dispose();
				_logger.Logs("Saving category cancelled.", _path);
				return;
			}
			saveDialog.Dispose();
        }
		/// <summary>
		/// Refresh the data for the grid.
		/// </summary>
        public void RefreshData()
        {			
			_logger.Logs("Refreshing the data.", _path);
			_grid.DataStore = _categories.ToArray();
        }
    }
}
