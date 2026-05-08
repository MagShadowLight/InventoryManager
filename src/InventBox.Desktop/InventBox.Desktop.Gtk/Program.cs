using System;
using System.Threading.Tasks;
using Eto.Forms;
using InventBox.Core;

namespace InventBox.Desktop.Gtk
{
	class Program
	{
		
		[STAThread]
		public static async Task Main(string[] args)
		{
			// var scanner = new BarCodeScanner("Test.log");
			// scanner.EncodeBarCode("Test1", "Test.png");
			new Application(Eto.Platforms.Gtk).Run(new MainForm());
		}
	}
}
