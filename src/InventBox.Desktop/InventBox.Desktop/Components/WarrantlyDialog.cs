using System;
using Eto.Forms;
using Eto.Drawing;
using InventBox.Desktop.Interfaces;
using InventBox.Core.Models;
using InventBox.Core;
using InventBox.Desktop.Components.ItemsForm;
using InventBox.Desktop.ModelViews;
using InventBox.Core.Enums;

namespace InventBox.Desktop.Components
{
	/// <summary>
	/// Represents a dialog for creating warrantly.
	/// </summary>
	public partial class WarrantlyDialog : Dialog, IDialogs<WarrantlyModelView>
	{
		private static FileLogger _logger;
		private string _path;
		private readonly Mode _mode;
		private readonly Action<Warrantly> _onSubmit;
		TextBox startDatePicker;
		TextBox endDatePicker;
		/// <summary>
		/// Initialize a new instance for dialog.
		/// </summary>
		/// <param name="modelView">Model view for warrantly.</param>
		/// <param name="path">Path for logger.</param>
		/// <param name="logger">file logger for logging purpose.</param>
		/// <param name="mode">Mode for creating or editing warrantly data.</param>
		/// <param name="size">Size for the dialog.</param>
		/// <param name="onSubmit">Event handler for submitting data.</param>
		public WarrantlyDialog(WarrantlyModelView modelView, string path, FileLogger logger, Mode mode, Size size, Action<Warrantly> onSubmit)
		{
			_path = path;
			_logger = logger;
			_mode = mode;
			_logger.Logs("Opening create warrantly dialog.", _path);
			Size = size;
			DataContext = modelView;
			Content = CreatePanel(modelView);
			_onSubmit = onSubmit;
			Title = _mode == Mode.Create ? "Add Warrantly" : "Edit Warrantly";
		}
		/// <summary>
		/// Create the panel for warrantly dialog.
		/// </summary>
		/// <param name="modelView">Model view for warrantly data.</param>
		/// <returns>Panel for display.</returns>
		private Panel CreatePanel(WarrantlyModelView modelView)
		{
			return new Panel
			{
				Content = CreateForm(modelView)
			};
		}
		/// <summary>
		/// Create a dialog for creating warrantly with user input.
		/// </summary>
		/// <param name="modelView">Model view for warrantly.</param>
		/// <returns>layout for display.</returns>
        public DynamicLayout CreateForm(WarrantlyModelView modelView)
        {
			// Create input variable
			startDatePicker = new TextBox() {Text = modelView.StartDate.ToString("yyyy-MM-dd")};
			endDatePicker = new TextBox() {Text = modelView.EndDate.ToString("yyyy-MM-dd")};
			EnumDropDown<Status> statusDropDown = new EnumDropDown<Status>();
			TextBox providerTextBox = new TextBox();
			TextBox contactNoTextBox = new TextBox();
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
		/// <summary>
		/// Create the command for submitting warrantly data.
		/// </summary>
		/// <returns>Submit command for button.</returns>
        public Command CreateSubmitButton()
        {
			var command = new Command();
			command.Executed += (sender, e) =>
			{
				_logger.Logs("Submitting warrantly data.", _path);
				var model = (WarrantlyModelView)DataContext;
				if (DateTime.TryParse(startDatePicker.Text, out var start))
					model.StartDate = start;
				if (DateTime.TryParse(endDatePicker.Text, out var end))
					model.EndDate = end;
				DateTime expiredate = GetExpireDate(end);
				if (expiredate == DateTime.MinValue)
					return;
				if (DateTime.Now > end)
					model.Status = Status.Expired;
				else if (DateTime.Now >= expiredate && DateTime.Now < end)
					model.Status = Status.Expiring;
				else
					model.Status = Status.Covered;
				_onSubmit?.Invoke(model);
				Close();
			};
			return command;
        }
		/// <summary>
		/// Calculate the expiring date from the end date.
		/// </summary>
		/// <param name="end">Date for warrantly expired</param>
		/// <returns>Date and time for date before expire date.</returns>
        private DateTime GetExpireDate(DateTime end)
		{
			try {
				var day = end.Day - 30;
				int month = end.Month;
				int year = end.Year;
				if (day <= 0)
				{
					month = end.Month - 1;
					day = (day % 31) * -1;
				}
				if (month <= 0) {
					year = end.Year - 1;
					month = (month % 12) * -1;
				}
				var date = $"{year}/{month}/{day}";
				return DateTime.Parse(date);
			} catch (Exception ex)
			{
				MessageBox.Show("Failed to create Warrantly. Please select valid date", MessageBoxButtons.OK, MessageBoxType.Error);
				_logger.Error($"Failed to parse date: {ex.Message}", _path);
				return new DateTime();
			}
		}
    }
}
