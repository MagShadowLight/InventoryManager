using System;
using Eto.Forms;
using Eto.Drawing;
using InventBox.Desktop.Interfaces;
using InventBox.Desktop.ModelViews;
using InventBox.Core;
using InventBox.Desktop.Components.ItemsForm;
using InventBox.Core.Models;

namespace EtoApp
{
	public partial class LocationsDialog : Dialog, IDialogs<LocationsModelView>
	{
		private FileLogger _logger;
		private string _path;
		private readonly Mode _mode;
		private readonly Action<Locations> _onSubmit;
		public LocationsDialog(LocationsModelView modelView, Mode mode, Action<Locations> onSubmitEvent, string path, FileLogger logger)
		{
			_path = path;
			_logger = logger;
			_mode = mode;
			_onSubmit = onSubmitEvent;
			DataContext = modelView;
			Title = _mode == Mode.Create ? "Create Location" : "Edit Location";
			Size = new Size(750,750);
			Content = CreateForm(modelView);
		}

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
						new Button {Text = "Submit", Size = new Size(100, 10), Command = SubmitButton },
						""
					},
					new DynamicRow{}
				}
			};
        }

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
