using System;
using Eto.Forms;
using Eto.Drawing;
using InventBox.Desktop.Interfaces;
using InventBox.Desktop.ModelViews;
using InventBox.Core;
using InventBox.Desktop.Components.ItemsForm;
using InventBox.Core.Models;

namespace InventBox.Desktop.Components.LocationForm
{
	/// <summary>
	/// Represents the dialog for creating and editing the locations.
	/// </summary>
	public partial class LocationsDialog : Dialog, IDialogs<LocationsModelView>
	{
		private FileLogger _logger;
		private string _path;
		private readonly Mode _mode;
		private readonly Action<Locations> _onSubmit;
		/// <summary>
		/// Initialize a new instance for the dialog.
		/// </summary>
		/// <param name="modelView">Model view for locations.</param>
		/// <param name="mode">Mode for either creating or editing.</param>
		/// <param name="onSubmitEvent">Event handler for submitting data.</param>
		/// <param name="path">The path for logger.</param>
		/// <param name="logger">The logger for logging purpose.</param>
		public LocationsDialog(LocationsModelView modelView, Mode mode, Action<Locations> onSubmitEvent, string path, FileLogger logger)
		{
			_path = path;
			_logger = logger;
			_mode = mode;
			_onSubmit = onSubmitEvent;
			DataContext = modelView;
			Title = _mode == Mode.Create ? "Create Location" : "Edit Location";
			Size = new Size(300,250);
			Content = CreateForm(modelView);
		}
		/// <summary>
		/// Create a dynamic layout for the panel.
		/// </summary>
		/// <param name="modelView">Model view for locations.</param>
		/// <returns>Layout for display.</returns>
        public DynamicLayout CreateForm(LocationsModelView modelView)
        {
			var floorInput = new TextBox{ Width = 200 };
			var roomInput = new TextBox{ Width = 200 };
			var containerInput = new TextBox { Width = 200 };
			var xInput = new NumericStepper { Width = 200 };
			var yInput = new NumericStepper { Width = 200 };
			var SubmitButton = CreateSubmitButton();

			floorInput.TextBinding.BindDataContext((LocationsModelView location) => location.Floor);
			roomInput.TextBinding.BindDataContext((LocationsModelView location) => location.Room);
			containerInput.TextBinding.BindDataContext((LocationsModelView location) => location.Container);
			xInput.ValueBinding.BindDataContext((LocationsModelView location) => location.X);
			yInput.ValueBinding.BindDataContext((LocationsModelView location) => location.Y);

			return new DynamicLayout
			{
				Padding = 10,
				Rows =
				{
					new DynamicRow
					{
						new Label { Text = "Floor" },
						floorInput
					},
					new DynamicRow
					{
						new Label { Text = "Room" },
						roomInput
					},
					new DynamicRow
					{
						new Label { Text = "Container" },
						containerInput
					},
					new DynamicRow
					{
						new Label {TextAlignment = TextAlignment.Center, Text = "Coordinate: "}
					},
					new DynamicRow
					{
						new Label { Text = "X" },
						xInput
					},
					new DynamicRow
					{
						new Label { Text = "Y" },
						yInput
					},
					new DynamicRow
					{
						"",
						new Button {Text = "Submit", Size = new Size(100, 25), Command = SubmitButton },
						""
					},
					new DynamicRow{}
				}
			};
        }
		/// <summary>
		/// Create a command for submitting the data.
		/// </summary>
		/// <returns>Command for the button.</returns>
        public Command CreateSubmitButton()
        {
			var createCommand = new Command();
			createCommand.Executed += (sender, e) =>
			{
				var model = (LocationsModelView)DataContext;
				_onSubmit?.Invoke(model);
				Close();
			};
			return createCommand;
        }
    }
}
