using System;
using Eto.Forms;
using Eto.Drawing;
using System.Threading.Tasks;
using InventBox.Core;
using System.IO;
using System.Threading;
using System.ComponentModel;

namespace InventBox.Desktop.Component.BarCodeComponents
{
	/// <summary>
	/// Represents the dialog for bar code scanner.
	/// </summary>
	public partial class BarCodeScannerDialog : Dialog
	{
		private BarCodeScanner _scanner;
		private static string _loggerPath = string.Empty;
		private ImageCapture _capture = new ImageCapture(_loggerPath);
		private FileLogger _logger;
		private ImageView _preview;
		private string _path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),".tmp", "InventBox", "Images", "Barcode.png");
		private readonly CancellationTokenSource _cancellation = new CancellationTokenSource();

		/// <summary>
		/// Initialize a new instance for bar code scanner dialog.
		/// </summary>
		/// <param name="capture">The capture for the image.</param>
		/// <param name="path">The path for the logger.</param>
		public BarCodeScannerDialog(ImageCapture capture, string path, FileLogger logger)
		{
			_loggerPath = path;
			_logger = logger;
			_logger.Logs("Opening bar code scanner.", _loggerPath);
			_scanner = new BarCodeScanner(_loggerPath);
			_capture = capture;
			_preview = CreatePreview();
			Task.Run(async () => await StartCapture());
			Content = CreateLayout();
		}
        /// <summary>
        /// Occurs when the dialog is closed.
        /// </summary>
        /// <param name="e">The argument for event.</param>\
        protected override void OnClosing(CancelEventArgs e)
        {
            _logger.Logs("Closing bar code scanner", _loggerPath);
            _cancellation.Cancel();
            Application.Instance.Invoke(async () => await _capture.CloseCapture());
            _cancellation.Dispose();
            base.OnClosing(e);
        }
		/// <summary>
		/// Create an Image preview.
		/// </summary>
		/// <returns>View of the image.</returns>
		private ImageView CreatePreview()
		{
			return new ImageView()
			{
				Width = 1000,
				Height = 500,
				BackgroundColor = Colors.DimGray,
			};
		}
		/// <summary>
		/// Occurs when the dialog is opened.
		/// </summary>
		/// <returns>Task operation.</returns>
		private async Task StartCapture()
		{
			await _capture.StartCapture(onFrameCaptured);			
		}
		/// <summary>
		/// Create the layout for the dialog.
		/// </summary>
		/// <returns>Dynamic layout.</returns>
		private DynamicLayout CreateLayout()
		{
			var layout = new DynamicLayout
			{
				Padding = 10,
			};
			layout.BeginVertical();
			layout.Add(_preview, yscale: true);
			layout.AddSpace();
			layout.EndVertical();
			return layout;
		}
		/// <summary>
		/// Capture the frame from the camera.
		/// </summary>
		/// <returns>Task operation.</returns>
		private async Task OnCapture()
		{
			await _capture.StopCapture(_path);
			Close();
		}
		/// <summary>
		/// Capture the frame from the camera.
		/// </summary>
		/// <param name="frame"></param>
		private void onFrameCaptured(byte[] frame)
		{
			if (_cancellation.IsCancellationRequested)
				return;
			var barcode = _scanner.TryScanBarCode(frame);
			try {
			Application.Instance.Invoke(async () =>
			{
				var old = _preview.Image;
				_preview.Image = new Bitmap(frame);
				old?.Dispose();
				if (!string.IsNullOrEmpty(barcode)) {
					await OnCapture();
					//Dispose();
				}
			});
			} catch(Exception ex)
			{
				Task.Run(async () => await _capture.CloseCapture());
				//Dispose();
			}
		}
	}
}
