using System;
using System.IO;
using Eto.Forms;
using InventBox.Core;

namespace InventBox.Desktop.Mac
{
	class Program
	{
		[STAThread]
		public static void Main(string[] args)
		{
			new Application(Eto.Platforms.Mac64).Run(new MainForm());
		}
	}
}
