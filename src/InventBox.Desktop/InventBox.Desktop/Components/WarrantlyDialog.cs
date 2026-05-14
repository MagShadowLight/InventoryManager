using System;
using Eto.Forms;
using Eto.Drawing;
using InventBox.Desktop.Interfaces;
using InventBox.Core.Models;
using InventBox.Core;
using InventBox.Desktop.Components.ItemsForm;
using InventBox.Desktop.ModelViews;

namespace InventBox.Desktop.Components
{
	public partial class WarrantlyDialog : Dialog, IDialogs<WarrantlyModelView>
	{
		private static FileLogger _logger;
		private string _path;
		private readonly Mode _mode;
		private readonly Action<Warrantly> _onSubmit;
		public WarrantlyDialog(WarrantlyModelView modelView, string path, FileLogger logger, Mode mode, Size size, Action<Warrantly> onSubmit)
		{
			_path = path;
			_logger = logger;
			_mode = mode;
			Size = size;
			Content = CreateForm(modelView);
			_onSubmit = onSubmit;
			Title = _mode == Mode.Create ? "Add Warrantly" : "Edit Warrantly";
		}

        public DynamicLayout CreateForm(WarrantlyModelView modelView)
        {
			// Create input variable
			DateTimePicker startDatePicker = new DateTimePicker();
			DateTimePicker endDatePicker = new DateTimePicker();
			EnumDropDown<Status> statusDropDown = new EnumDropDown<Status>();
			TextBox providerTextBox = new TextBox();
			TextBox contactNoTextBox = new TextBox();
			// Bind those input to models
			startDatePicker.ValueBinding.BindDataContext(Binding.Property((WarrantlyModelView model) => model.StartDate).Convert<DateTime?>(
				v => v
			));
			endDatePicker.ValueBinding.BindDataContext(Binding.Property((WarrantlyModelView model) => model.EndDate).Convert<DateTime?>(
				v => v
			));
			statusDropDown.SelectedValueBinding.BindDataContext(Binding.Property((WarrantlyModelView model) => model.Status));
			providerTextBox.TextBinding.BindDataContext(Binding.Property((WarrantlyModelView model) => model.Provider));
			contactNoTextBox.TextBinding.BindDataContext(Binding.Property((WarrantlyModelView model) => model.ContactNumber));
			var SubmitButton = CreateSubmitButton();
			// Add input to form
			var form = new DynamicLayout();
			form.BeginVertical();
			form.AddRow(
				"Start Date",
				startDatePicker
			);
			form.AddRow(
				"End Date",
				endDatePicker
			);
			form.AddRow(
				"Status",
				statusDropDown
			);
			form.AddRow(
				"Provider",
				providerTextBox
			);
			form.AddRow(
				"Contact Number",
				contactNoTextBox
			);
			form.EndVertical();
			form.BeginHorizontal();
			form.AddSeparateRow(
				null,
				new Button() {Command = SubmitButton, Width = 100, Height = 10, Text = "Submit"},
				null
			);
			form.EndHorizontal();
			return form;
        }

        public Command CreateSubmitButton()
        {
			var command = new Command();
			command.Executed += (sender, e) =>
			{
				var model = (Warrantly)DataContext;
				_onSubmit?.Invoke(model);
				Close();
			};
			return command;
        }
    }
}
