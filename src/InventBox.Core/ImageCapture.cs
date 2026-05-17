using Eto.Drawing;
using Eto.Forms;
using FlashCap;

namespace InventBox.Core;

public class ImageCapture
{
    private ConsoleLogger logger = new ConsoleLogger();
    CancellationToken token;
    CaptureDevices? devices;
    byte[]? _frame;
    CaptureDeviceDescriptor? descriptor1;
    VideoCharacteristics? characteristic;
    CaptureDevice? device;
    

    public async Task OpenCapture(CancellationTokenSource source)
    {
        token = source.Token;
        
        devices = new CaptureDevices();


        foreach (var descriptor in devices.EnumerateDescriptors())
        {
            if (descriptor == null)
            {
                logger.Error("Could not detect camera device");
            }
            if (descriptor.Characteristics.Count() == 0)
                 continue;
            descriptor1 = descriptor;
            characteristic = descriptor1.Characteristics
            .FirstOrDefault(c => c.PixelFormat != PixelFormats.Unknown)!;
        }

    }
    public async Task StartCapture(Action<byte[]>? onFrame = null)
    {
        device = await descriptor1.OpenAsync(
            characteristic,
            BufferScope =>
            {                
                var image = BufferScope.Buffer.CopyImage();
                _frame = image;
                onFrame?.Invoke(image);
            },
            token
        );
        await device.StartAsync(token).ConfigureAwait(false);
    }
    public async Task StopCapture(string imagePath)
    {
        if (_frame == null)
            return;

        await device.StopAsync(token);
        using var fileStream = new FileStream(
            imagePath,
            FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite
        );
        await fileStream.WriteAsync(_frame, 0, _frame.Length, token);
        await fileStream.FlushAsync(token);
    }


}
