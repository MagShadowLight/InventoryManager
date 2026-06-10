using Eto.Drawing;
using Eto.Forms;
using FlashCap;

namespace InventBox.Core;

/// <summary>
/// Represents the image to be captured.
/// </summary>
public class ImageCapture
{
    private ConsoleLogger logger = new ConsoleLogger();
    CancellationToken token;
    CaptureDevices? devices;
    public byte[]? _frame;
    CaptureDeviceDescriptor? descriptor1;
    VideoCharacteristics? characteristic1;
    CaptureDevice? device;
    public bool IsCaptureOpen = false;
    /// <summary>
    /// Open the capture device.
    /// </summary>
    /// <param name="source">The source for cancellation token.</param>
    /// <returns>Task Operation.</returns>
    public async Task OpenCapture(CancellationTokenSource source)
    {
        token = source.Token;
        
        devices = new CaptureDevices();

        try {
            foreach (var descriptor in devices.EnumerateDescriptors())
            {
                if (descriptor == null)
                {
                    logger.Error("Could not detect camera device");
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
        } catch (Exception ex)
        {
            // MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxType.Error);
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
        } catch (Exception ex)
        {
            // MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxType.Error);
            
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
        if (device == null)
            return;

        if (_frame == null)
            return;

        await device.StopAsync(token);
        using var fileStream = new FileStream(
            imagePath,
            FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite
        );
        await fileStream.WriteAsync(_frame, 0, _frame.Length, token);
        await fileStream.FlushAsync(token);
        } catch (Exception ex)
        {
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
            if (device == null)
                return;
            await device.StopAsync(token);
        } catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Error:", MessageBoxButtons.OK, MessageBoxType.Error);
        }
    }
}
