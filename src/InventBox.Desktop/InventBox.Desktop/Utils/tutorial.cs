using System;
using Eto.Forms;
using Eto.Drawing;
using System.IO;

namespace InventBox.Desktop.Utils
{
	/// <summary>
	/// Represents the dialog for newcomers.
	/// </summary>
	public partial class Tutorial : Dialog
	{
		private string title = "InventBox";
		private string _p1;
		private string _p2;
		private string _p3;
		private string _p4;
		private string _p5;
		private Size _size;	
		private string _path;
		/// <summary>
		/// Initialize a new instance for dialog.
		/// </summary>
		/// <param name="size">The size for dialog.</param>
		public Tutorial(Size size, string path, string p1 = null, string p2 = null, string p3 = null, string p4 = null, string p5 = null)
		{
			_path = path;
			_p1 = p1;
			_p2 = p2;
			_p3 = p3;
			_p4 = p4;
			_p5 = p5;
			_size = size;
			var layout = new DynamicLayout
			{
				Size = _size,
				Rows =
				{
					CreateText(FontFamilies.Cursive, 16.0f, title, TextAlignment.Center, FontStyle.Bold, FontDecoration.None),
					CreateSpace(),
					CreateText(FontFamilies.Serif, 12.0f, _p1, TextAlignment.Left, FontStyle.None, FontDecoration.None),
					CreateSpace(),
					CreateText(FontFamilies.Serif, 12.0f, _p2, TextAlignment.Left, FontStyle.None, FontDecoration.None),
					CreateSpace(),
					CreateText(FontFamilies.Serif, 12.0f, _p3, TextAlignment.Left, FontStyle.None, FontDecoration.None),
					CreateSpace(),
					CreateText(FontFamilies.Serif, 12.0f, _p4, TextAlignment.Left, FontStyle.None, FontDecoration.None),
					CreateSpace(),
					CreateText(FontFamilies.Serif, 12.0f, _p5, TextAlignment.Left, FontStyle.None, FontDecoration.None),
					new StackLayout
					{
						VerticalContentAlignment = VerticalAlignment.Top,
						HorizontalContentAlignment = HorizontalAlignment.Center,
						Items =
						{
							CreateButton("Close", new Size(100,50), EndTutorial, Cursors.Pointer)
						}
					}
				},
				Spacing = new Size(0, 5)
			};
			Content = layout;
		}
		/// <summary>
		/// Create the space for the dialog.
		/// </summary>
		/// <returns>Dynamic row for display.</returns>
		private DynamicRow CreateSpace()
		{
			return new DynamicRow(null, true, false);
		}
		/// <summary>
		/// Create a text for tutorial dialog.
		/// </summary>
		/// <param name="family">Font family for dialog.</param>
		/// <param name="fontSize">Size for the font.</param>
		/// <param name="message">Text to display.</param>
		/// <param name="alignment">Align the text.</param>
		/// <param name="style">Style the font.</param>
		/// <param name="decoration">decorate the font.</param>
		/// <returns>The label with text for display.</returns>
		private Label CreateText(FontFamily family, float fontSize, string message, TextAlignment alignment, FontStyle style, FontDecoration decoration)
		{
			var label = new Label
			{
				Font = new Font(family, fontSize, style, decoration),
				Text = message,
				Width = _size.Width,
				VerticalAlignment = VerticalAlignment.Top,
				TextAlignment = alignment
			};
			return label;
		}
		/// <summary>
		/// Create button for dialog.
		/// </summary>
		/// <param name="text">The text for button.</param>
		/// <param name="size">The size for button.</param>
		/// <param name="eventHandler">handler for clicking the button.</param>
		/// <param name="cursor">Cursor for button to show.</param>
		/// <returns>Button to display.</returns>
		private Button CreateButton(string text, Size size, Action eventHandler, Cursor cursor)
		{
			var command = new Command();
			command.Executed += (sender, eventArgs) => eventHandler();
			return new Button{Text = text, Size = size, Command = command, Cursor = cursor};
		}
		/// <summary>
		/// End tutorial and create file for not showing tutorial for future uses.
		/// </summary>
		private void EndTutorial()
		{
			FileStream stream = File.Create(_path);
			stream.Dispose();
			Close();
		}
	}
}
