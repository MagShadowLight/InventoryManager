using System;
using System.Threading.Tasks;
using Eto.Forms;

namespace InventBox.Desktop.Gtk
{
	class Program
	{
		
		[STAThread]
		public static async Task Main(string[] args)
		{			
			new Application(Eto.Platforms.Gtk).Run(new MainForm());
		}
	}
}
