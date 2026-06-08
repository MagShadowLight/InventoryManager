using Eto.Drawing;
using Eto.Forms;
using FlashCap;

namespace InventBox.Core;

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
            MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxType.Error);
            IsCaptureOpen = false;
        }
    }
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
            MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxType.Error);
            
        }
    }
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
