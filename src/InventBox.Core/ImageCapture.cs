using Eto.Drawing;
using Eto.Forms;
using FlashCap;

namespace InventBox.Core;

/// <summary>
/// Represents the image to be captured.
/// </summary>
public class ImageCapture
{
    private FileLogger _logger = new FileLogger();
    private string _path = string.Empty;
    CancellationToken token;
    CaptureDevices? devices;
    public byte[]? _frame;
    CaptureDeviceDescriptor? descriptor1;
    VideoCharacteristics? characteristic1;
    CaptureDevice? device;
    public bool IsCaptureOpen = false;

    public ImageCapture(string path)
    {
        _path = path;
    }
    /// <summary>
    /// Open the capture device.
    /// </summary>
    /// <param name="source">The source for cancellation token.</param>
    /// <returns>Task Operation.</returns>
    public async Task OpenCapture(CancellationTokenSource source)
    {
        _logger.Logs("Opening camera", _path);
        token = source.Token;
        
        devices = new CaptureDevices();

        try {
            foreach (var descriptor in devices.EnumerateDescriptors())
            {
                if (descriptor == null)
                {
                    _logger.Error("Could not detect camera device");
                    continue;
                }
                descriptor1 = descriptor;
                var characteristic = descriptor1.Characteristics
                .Where(c => c.PixelFormat != PixelFormats.Unknown)!.ToList();
                if (descriptor!.Characteristics.Count() == 0)
                    continue;
                
                characteristic1 = characteristic.First();
                break;
            }
            IsCaptureOpen = true;
            _logger.Logs("camera opened", _path);
        } catch (Exception ex)
        {
            _logger.Error($"camera have failed to open. Message: {ex.Message}", _path);
            IsCaptureOpen = false;
        }
    }
    /// <summary>
    /// Starting the image capture from the camera.
    /// </summary>
    /// <param name="onFrame">The frame from the capture.</param>
    /// <returns>Task operation.</returns>
    public async Task StartCapture(Action<byte[]>? onFrame = null)
    {
        try {
            _logger.Logs("Starting camera", _path);
            device = await descriptor1!.OpenAsync(
            characteristic1!,
            BufferScope =>
                {                
                    var image = BufferScope.Buffer.CopyImage();
                    _frame = image;
                    onFrame?.Invoke(image);
                },
                token
            );
            await device.StartAsync(token).ConfigureAwait(false);
            _logger.Logs("Camera started", _path);
        } catch (Exception ex)
        {
            _logger.Error("Failed to start capture", _path);            
        }
    }
    /// <summary>
    /// Stop the capture and save into the file.
    /// </summary>
    /// <param name="imagePath">The path for the image.</param>
    /// <returns>Task operation.</returns>
    public async Task StopCapture(string imagePath)
    {
        try {
            _logger.Logs("Stopping capture.", _path);
            if (device == null)
                return;

            if (_frame == null)
                return;

            await device.StopAsync(token);
            _logger.Logs("Capture stopped. Saving image to file.", _path);
            using var fileStream = new FileStream(
                imagePath,
                FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite
            );
            await fileStream.WriteAsync(_frame, 0, _frame.Length, token);
            await fileStream.FlushAsync(token);
            _logger.Logs("Image saved to file.", _path);
        } catch (Exception ex)
        {
            _logger.Error($"Failed to stop capture or save file. {ex.Message}", _path);
            MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxType.Error);
        }
    }
    /// <summary>
    /// Close the capture from the image without saving.
    /// </summary>
    /// <returns>Task operation.</returns>
    public async Task CloseCapture()
    {
        try
        {
            _logger.Logs("Closing capture.", _path);
            if (device == null)
                return;
            await device.StopAsync(token);
            _logger.Logs("Capture closed.", _path);
        } catch (Exception ex)
        {
            _logger.Error($"Failed to close capture. {ex.Message}", _path);
        }
    }
}
