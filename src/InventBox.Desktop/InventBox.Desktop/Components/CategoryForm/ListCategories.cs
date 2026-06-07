using System;
using Eto.Forms;
using InventBox.Desktop.Interfaces;
using InventBox.Core.Models;
using InventBox.Desktop.ModelViews;
using System.Collections.Generic;
using InventBox.Core;
using Eto.Drawing;
using InventBox.Desktop.ModelView;
using System.Linq;
using InventBox.Desktop.Components.ItemsForm;
using System.IO;

namespace InventBox.Desktop.Components.CategoryForm
{
	public partial class ListCategories : Panel, IEventHandler, IControls<Category, CategoryModelView>
	{
		private static string TmpDir = Path.Combine(Path.GetTempPath(), "InventBox", "Data", "Category");
		private string TmpPath = Path.Combine(TmpDir, "Data-Category-tmp.csv");
		private JsonParser<Category> jsonParser;
		private TextBox searchBar;
		private List<Category> _categories = new List<Category>();
		private static string _path;
		private static FileLogger _logger;
		private DataManagement<Category> _datamanagement;
		private GridView _grid;
		private string searchText = "";
		public ListCategories(string path, FileLogger logger)
		{
			jsonParser = new JsonParser<Category>();
			_categories = ModelsList.categories;
			_path = path;
			_logger = logger;
			_datamanagement = new DataManagement<Category>(_path);
			_grid = CreateGrid();
			RefreshData();
			Visible = false;
			Content = CreateDynamicLayout();
		}
		private void CreateDirectory(string dir)
		{
			if (!Directory.Exists(dir))
				Directory.CreateDirectory(dir);
		}
		public ContextMenu CreateContextMenu()
		{
			var CopyCategoryCommand = CreateMenuItem("Copy category", OnCopy);
			var CreateCategoryCommand = CreateMenuItem("Create new category", OnCreate);
			var UpdateCategoryCommand = CreateMenuItem("Edit category", OnEdit);			
			var DeleteCategoryCommand = CreateMenuItem("Delete category", OnDelete);			
			var SaveCategoryCommand = CreateMenuItem("Save category", OnSave);			
			var LoadCategoryCommand = CreateMenuItem("Load category", OnLoad);
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

		public void OnCopy()
		{
			Category SelectedCategory = (Category)_grid.SelectedItem;
			var jsonItem = jsonParser.ParseJson(SelectedCategory);
			Clipboard.Instance.Clear();		
			Clipboard.Instance.Text = jsonItem;
		}

		public ButtonMenuItem CreateMenuItem(string text, Action clickHandler, Keys keys = Keys.None)
		{			
			var menuItem = new ButtonMenuItem{Text = text, Shortcut = keys};
			menuItem.Click += (sender, eventArgs) => clickHandler();
			return menuItem;
		}

        public Button AddButton(string text, int width, int height, Action eventHandler)
        {
			var command = new Command();
			command.Executed += (sender, eventArgs) => eventHandler();
			return new Button {Text = text, Width = width, Height = height, Command = command, Cursor = Cursors.Pointer};
        }

        public void Search()
        {
			if (string.IsNullOrEmpty(searchText)) {
				if (_categories.Count == ModelsList.categories.Count)
					MessageBox.Show("Search bar is empty. Please type in the search bar", "Search", MessageBoxButtons.OK, MessageBoxType.Information);
				_categories = ModelsList.categories;
			}
			else
				_categories = ModelsList.categories.Where(category => category.Name.Contains(searchText)).ToList();
			RefreshData();
        }

        public DynamicLayout CreateDynamicLayout()
        {
			searchBar = CreateSearchBar();
			DynamicLayout layout = new DynamicLayout()
			{
				Padding = 10
			};
			layout.BeginVertical(null, null, true, true);
			layout.BeginVertical();
			layout.BeginHorizontal();
			layout.Add(searchBar, true);
			layout.Add(AddButton("Search", 100, 50, () => Search()));
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
					AddButton("Create new category", 100, 50, OnCreate),
					AddButton("Edit selected category", 100, 50, OnEdit),
					AddButton("Delete selected category", 100, 50, OnDelete),
					null,
					AddButton("Save Category", 100, 50, OnSave),
					AddButton("Load Category", 100, 50, OnLoad)
				}
			);
			layout.EndHorizontal();
			layout.EndVertical();
			layout.EndVertical();
			return layout;
        }

        public GridView CreateGrid()
        {
			return new GridView()
			{
				ContextMenu = CreateContextMenu(),
				GridLines = GridLines.Both,
				AllowMultipleSelection = false,
				Columns =
				{
					GetColumn("Id", c => c.Id.ToString()),
					GetColumn("Name", c => c.Name),
					GetColumn("Description", c => c.Description)
				}
			};
        }

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

        public GridColumn GetColumn(string header, Func<Category, string> data)
        {
			return new GridColumn
			{
				HeaderText = header,
				Editable = false,
				DataCell = GetData(data)
			};
        }

        public TextBoxCell GetData(Func<Category, string> data)
        {
			return new TextBoxCell
			{
				Binding = Binding.Delegate<Category, string>(data, null)
			};
        }

        public CategoryModelView ModelViewCopy(Category category)
        {
			return new CategoryModelView
			{
				Id = category.Id,
				Name = category.Name,
				Description = category.Description
			};
        }

        public void OnCreate()
        {
			CategoryModelView modelView = new CategoryModelView(){Id = ModelsList.categories.Count + 1};
			var createCategoryDialog = new CategoryDialog(modelView, Mode.Create, category => ModelsList.categories.Add(category), _path, _logger);
			createCategoryDialog.Closed += (sender, e) => RefreshData();
			createCategoryDialog.ShowModal();
			CreateDirectory(TmpDir);
			_datamanagement.Save(ModelsList.categories, TmpPath);
			_categories = ModelsList.categories;
			Content = CreateDynamicLayout();
			RefreshData();
        }

        public void OnDelete()
        {
			if (_categories.Count <= 0) {
				MessageBox.Show("The list is empty. Please create one or load from file.", "Category not selected", MessageBoxButtons.OK, MessageBoxType.Information);
				return;
			}
			if (_grid.SelectedItem == null) {
				MessageBox.Show("Category have not been selected. Please select one", "Category not selected", MessageBoxButtons.OK, MessageBoxType.Information);
				return;
			}
			Category category = (Category)_grid.SelectedItem;
			var index = ModelsList.categories.IndexOf(category);
			if (index < 0)
				return;
			var deleteDialog = MessageBox.Show("Are you sure to delete the selected category?", "Delete selected category", MessageBoxButtons.YesNo, MessageBoxType.Information, MessageBoxDefaultButton.Yes);
			if (deleteDialog != DialogResult.Yes)
				return;
			ModelsList.categories.Remove(category);
			CreateDirectory(TmpDir);
			if (ModelsList.categories.Count > 0)
				_datamanagement.Save(ModelsList.categories, TmpPath);
			else
				File.Delete(TmpPath);
			_categories = ModelsList.categories;
			Content = CreateDynamicLayout();
			RefreshData();
        }

        public void OnEdit()
        {
			if (_categories.Count <= 0) {
				MessageBox.Show("The list is empty. Please create one or load from file.", "Item not selected", MessageBoxButtons.OK, MessageBoxType.Information);
				return;
			}
			if (_grid.SelectedItem == null) {
				MessageBox.Show("Category have not been selected. Please select one", "Item not selected", MessageBoxButtons.OK, MessageBoxType.Information);
				return;
			}
			Category category = (Category)_grid.SelectedItem;
			var index = ModelsList.categories.IndexOf(category);
			if (index < 0)
				return;
			CategoryModelView modelView = ModelViewCopy(category);
			var editCategoryDialog = new CategoryDialog(modelView, Mode.Edit, category => ModelsList.categories[index] = category, _path, _logger);
			editCategoryDialog.Closed += (sender, e) =>
			{
				CreateDirectory(TmpDir);
				_datamanagement.Save(ModelsList.categories, TmpPath);
				_categories = ModelsList.categories;
				RefreshData();
			};
			editCategoryDialog.ShowModal();
        }

        public void OnLoad()
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
			if (!loadDialog.FileName.Contains(".csv"))
				loadDialog.FileName = string.Empty;
			if (!string.IsNullOrEmpty(loadDialog.FileName))
			{
				ModelsList.categories = _datamanagement.Load(loadDialog.FileName);
				if (ModelsList.categories.Count == 0)
				{
					MessageBox.Show("Invalid data. Please choose a different file", "Load failed.", MessageBoxButtons.OK, MessageBoxType.Information);
					return;
				}
				_categories = ModelsList.categories;
				Content = CreateDynamicLayout();
				RefreshData();
			}
			loadDialog.Dispose();
        }

        public void OnSave()
        {
			Uri homeDir = new Uri(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile));
			var saveDialog = new SaveFileDialog
			{
				Filters =
				{
					new FileFilter("CSV File", ".csv")
				},
				Directory = homeDir
			};
			saveDialog.ShowDialog(this);
			if (saveDialog.FileName != string.Empty  && ModelsList.categories.Count > 0) {
				_datamanagement.Save(ModelsList.categories, saveDialog.FileName);
				File.Delete(TmpPath);
			}
			else if (string.IsNullOrEmpty(saveDialog.FileName)){
				saveDialog.Dispose();
				return;
			} else
				MessageBox.Show("categories list is empty. Please add one or load from file.", "Save failed.", MessageBoxButtons.OK, MessageBoxType.Information);
			saveDialog.Dispose();
        }

        public void RefreshData()
        {			
			_grid.DataStore = _categories.ToArray();
        }
    }
}
