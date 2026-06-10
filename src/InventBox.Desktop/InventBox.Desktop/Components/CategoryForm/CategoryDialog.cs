using System;
using Eto.Forms;
using Eto.Drawing;
using InventBox.Desktop.Interfaces;
using InventBox.Desktop.ModelViews;
using InventBox.Core;
using InventBox.Desktop.Components.ItemsForm;
using InventBox.Core.Models;

namespace InventBox.Desktop.Components.CategoryForm
{
	/// <summary>
	/// Represents the dialog for creating and editing category.
	/// </summary>
	public partial class CategoryDialog : Dialog, IDialogs<CategoryModelView>
	{
		private FileLogger _logger;
		private string _path;
		private readonly Mode _mode;
		private readonly Action<Category> _onSubmit;
		/// <summary>
		/// Initialize a new instance for dialog to create or edit category.
		/// </summary>
		/// <param name="modelView">Model view for category.</param>
		/// <param name="mode">Editing or creating mode.</param>
		/// <param name="onSubmitEvent">Occurs when the user press the submit button.</param>
		/// <param name="path">The path for the logger.</param>
		/// <param name="logger">the logger for logging purpose.</param>
		public CategoryDialog(CategoryModelView modelView, Mode mode, Action<Category> onSubmitEvent, string path, FileLogger logger)
		{
			_path = path;
			_logger = logger;
			_mode = mode;
			_onSubmit = onSubmitEvent;
			DataContext = modelView;
			Title = _mode == Mode.Create ? "Create Category" : "Edit Category";
			Size = new Size(300, 150);
			Content = CreateForm(modelView);
			

		}
		/// <summary>
		/// Create a layout for creating or editing category.
		/// </summary>
		/// <param name="modelView">Model view for category.</param>
		/// <returns>Layout for the dialog.</returns>
        public DynamicLayout CreateForm(CategoryModelView modelView)
        {
			var nameInput = new TextBox() { Width = 200 };
			var descriptionInput = new TextBox() { Width = 200 };
			var SubmitButton = CreateSubmitButton();

			nameInput.TextBinding.BindDataContext((CategoryModelView category) => category.Name);
			descriptionInput.TextBinding.BindDataContext((CategoryModelView category) => category.Description);

			return new DynamicLayout
			{
				Padding = 10,
				Rows =
				{
					new DynamicRow
					{
						new Label() { Text = "Name"},
						nameInput
					},
					new DynamicRow
					{
						new Label() { Text = "Description"},
						descriptionInput
					},
					new DynamicRow
					{
						"",
						new Button {Text = "Submit", Size = new Size(100, 25), Command = SubmitButton},
						""
					},
					new DynamicRow
					{
						
					}
				}
			};
        }
		/// <summary>
		/// Create the command for submit.
		/// </summary>
		/// <returns>Command for the button.</returns>
        public Command CreateSubmitButton()
        {
			var createCommand = new Command();
			createCommand.Executed += (sender, e) =>
			{
				var model = (CategoryModelView)DataContext;
				_onSubmit?.Invoke(model);
				Close();	
			};
			return createCommand;
        }
    }
}
