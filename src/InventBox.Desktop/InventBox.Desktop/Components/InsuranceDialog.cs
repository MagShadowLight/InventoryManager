using System;
using Eto.Forms;
using Eto.Drawing;
using InventBox.Desktop.ModelViews;
using InventBox.Desktop.Interfaces;
using InventBox.Core;
using InventBox.Desktop.Components.ItemsForm;
using InventBox.Core.Models;

namespace InventBox.Desktop.Components
{
	/// <summary>
	/// Represents a dialog for creating insurance.
	/// </summary>
	public partial class InsuranceDialog : Dialog, IDialogs<InsuranceModelView>
	{
		private static FileLogger _logger;
		private string _path;
		private readonly Mode _mode;
		private readonly Action<Insurance> _onSubmit;
		TextBox startDatePicker;
		TextBox endDatePicker;
		/// <summary>
		/// Initialize a new instance for dialog.
		/// </summary>
		/// <param name="modelView">Model view for insurance.</param>
		/// <param name="path">Path for logger.</param>
		/// <param name="logger">Logger for logging purposes.</param>
		/// <param name="mode">Mode for creating or editing.</param>
		/// <param name="size">Size for the dialog.</param>
		/// <param name="onSubmit">Event handler for submitting data.</param>
		public InsuranceDialog(InsuranceModelView modelView, string path, FileLogger logger, Mode mode, Size size, Action<Insurance> onSubmit)
		{
			_path = path;
			_logger = logger;
			_mode = mode;
			Size = size;
			DataContext = modelView;
			Content = CreatePanel(modelView);
			_onSubmit = onSubmit;
			Title = _mode == Mode.Create ? "Add Insurance" : "Edit Insurance";
		}
		/// <summary>
		/// Create the panel for the insurance dialog.
		/// </summary>
		/// <param name="modelView">The model view for insurance.</param>
		/// <returns>Panel for display.</returns>
        private Control CreatePanel(InsuranceModelView modelView)
        {
			return new Panel
			{
				Content = CreateForm(modelView)
			};
        }
		/// <summary>
		/// Create the layout for creating insurance.
		/// </summary>
		/// <param name="modelView">Model view for insurance.</param>
		/// <returns>Layout for display.</returns>
        public DynamicLayout CreateForm(InsuranceModelView modelView)
        {
			// Create input variable
			startDatePicker = new TextBox() {Text = modelView.StartDate.ToString("yyyy-MM-dd")};
			endDatePicker = new TextBox() {Text = modelView.EndDate.ToString("yyyy-MM-dd")};
			EnumDropDown<Status> statusDropDown = new EnumDropDown<Status>();
			TextBox providerTextBox = new TextBox();
			TextBox contactNoTextBox = new TextBox();
			
			// Bind those input to models
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
		/// Create a command for submitting data.
		/// </summary>
		/// <returns>submit command for button.</returns>
        public Command CreateSubmitButton()
        {
			var command = new Command();
			command.Executed += (sender, e) =>
			{				
				var model = (InsuranceModelView)DataContext;
				if (DateTime.TryParse(startDatePicker.Text, out var start))
					model.StartDate = start;
				if (DateTime.TryParse(endDatePicker.Text, out var end))
					model.EndDate = end;
				DateTime expiredate = GetExpireDate(end);
				if (DateTime.Now >= expiredate && DateTime.Now < end)
					model.Insured = Status.Expiring;
				if (DateTime.Now >= end)
					model.Insured = Status.Expired;
				_onSubmit?.Invoke(model);
				Close();
			};
			return command;
        }
		/// <summary>
		/// Calculate the expiring date from the end date.
		/// </summary>
		/// <param name="end">Date for insurance expired</param>
		/// <returns>Date and time for date before expire date.</returns>
        private DateTime GetExpireDate(DateTime end)
        {
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
        }
    }
}
