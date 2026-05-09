using System;
using Eto.Forms;
using Eto.Drawing;
using System.Threading.Tasks;
using InventBox.Core;
using System.IO;
using ZXing;

namespace EtoApp
{
	public partial class BarCodeScannerDialog : Dialog
	{
		private BarCodeScanner _scanner;
		private string _loggerPath = string.Empty;
		private ImageCapture _capture = new ImageCapture();
		private ImageView _preview;
		private string _path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),".tmp", "InventBox", "Images", "Barcode.png");

		public BarCodeScannerDialog(ImageCapture capture, string path)
		{
			_loggerPath = path;
			_scanner = new BarCodeScanner(_loggerPath);
			_capture = capture;
			_preview = CreatePreview();
			Task task = new Task(async () => await StartCapture());
			task.RunSynchronously();
			Content = CreateLayout();
		}

		private ImageView CreatePreview()
		{
			return new ImageView()
			{
				Width = 1000,
				Height = 500,
				BackgroundColor = Colors.DimGray,
			};
		}

		private async Task StartCapture()
		{
			await _capture.StartCapture(onFrameCaptured);			
		}

		private DynamicLayout CreateLayout()
		{
			var layout = new DynamicLayout
			{
				Padding = 10,
			};
			layout.BeginVertical();
			layout.Add(_preview, yscale: true);
			layout.AddSpace();
			layout.AddSeparateRow(null,AddButton("Test", 100,100, async () => await OnCapture()),null);
			layout.EndVertical();
			return layout;
		}

		private async Task OnCapture()
		{
			await _capture.StopCapture(_path);
			Close();
		}

		private Button AddButton(string text, int width, int height, Action eventHandler)
		{
			var command = new Command();
			command.Executed += (sender, eventArgs) => eventHandler();
			return new Button { Text = text, Width = width, Height = height, Command = command};
		}
		private void onFrameCaptured(byte[] frame)
		{
			var barcode = _scanner.ScanBarCode(frame);
			Application.Instance.AsyncInvoke(async () =>
			{
				var old = _preview.Image;
				_preview.Image = new Bitmap(frame);
				old?.Dispose();
				if (!string.IsNullOrEmpty(barcode))
					await OnCapture();
			});
		}
	}
}
