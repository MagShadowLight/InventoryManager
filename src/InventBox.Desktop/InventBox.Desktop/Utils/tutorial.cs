using System;
using Eto.Forms;
using Eto.Drawing;
using System.IO;

namespace InventBox.Desktop.Utils
{
	public partial class Tutorial : Dialog
	{
		private string title = "InventBox";
		private string p1 = "InventBox is the inventory management application where you can manage the inventory in your home."
		+ "It include the items, categories, locations, and optional warrantly and insurance.";
		private string p2 = "Inventory section is where you can create, manage, and delete items inside the grid."
		+ "In this section, you can search for the items by name, category, floor, and room via dropdown and scan the barcode from the camera.";
		private string p3 = "Category section is where you can create, manage, and delete category inside the grid."
		+ "In this section, you can search the category by name";
		private string p4 = "Location section is similar to category section where you can create, manage, and delete location plus searching by room.";
		private string p5 = "All of those section have the options to save and load the data from the file.";
		private Size _size;	
		public Tutorial(Size size)
		{
			_size = size;
			var layout = new DynamicLayout
			{
				Size = _size,
				Rows =
				{
					CreateText(FontFamilies.Cursive, 16.0f, title, TextAlignment.Center, FontStyle.Bold, FontDecoration.None),
					CreateSpace(),
					CreateText(FontFamilies.Serif, 12.0f, p1, TextAlignment.Left, FontStyle.None, FontDecoration.None),
					CreateSpace(),
					CreateText(FontFamilies.Serif, 12.0f, p2, TextAlignment.Left, FontStyle.None, FontDecoration.None),
					CreateSpace(),
					CreateText(FontFamilies.Serif, 12.0f, p3, TextAlignment.Left, FontStyle.None, FontDecoration.None),
					CreateSpace(),
					CreateText(FontFamilies.Serif, 12.0f, p4, TextAlignment.Left, FontStyle.None, FontDecoration.None),
					CreateSpace(),
					CreateText(FontFamilies.Serif, 12.0f, p5, TextAlignment.Left, FontStyle.None, FontDecoration.None),
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

		private DynamicRow CreateSpace()
		{
			return new DynamicRow(null, true, false);
		}
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
		private Button CreateButton(string text, Size size, Action eventHandler, Cursor cursor)
		{
			var command = new Command();
			command.Executed += (sender, eventArgs) => eventHandler();
			return new Button{Text = text, Size = size, Command = command, Cursor = cursor};
		}
		private void EndTutorial()
		{
			FileStream stream = File.Create("Done.md");
			stream.Dispose();
			Close();
		}
	}
}
