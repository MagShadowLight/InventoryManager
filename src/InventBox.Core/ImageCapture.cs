using FlashCap;

namespace InventBox.Core;

public class ImageCapture
{
    private ConsoleLogger logger = new ConsoleLogger();
    CancellationToken token;
    CaptureDevices devices;
    CaptureDeviceDescriptor? descriptor1;
    VideoCharacteristics? characteristic;
    TaskCompletionSource<byte[]>? taskSource;
    CaptureDevice device;

    public async Task OpenCapture()
    {
        CancellationTokenSource source = new CancellationTokenSource();
        token = source.Token;
        
        devices = new CaptureDevices();

        descriptor1 = devices.EnumerateDescriptors().ElementAt(1)!;
        if (descriptor1 == null)
        {
            logger.Error("Could not detect camera device");
        }

        characteristic = descriptor1.Characteristics
        .FirstOrDefault(c => c.PixelFormat != PixelFormats.Unknown)!;

        taskSource = new TaskCompletionSource<byte[]>();
        device = await descriptor1.OpenAsync(
            characteristic,
            BufferScope =>
            {
                
                var image = BufferScope.Buffer.CopyImage();

                taskSource.TrySetResult(image);
            },
            token
        );
    }
    public async Task StartCapture()
    {
        await device.StartAsync(token);
    }
    public async Task StopCapture()
    {
        var imageData = await taskSource.Task;

        await device.StopAsync(token);

        var path = "Test.png";
        using var fileStream = new FileStream(
            path,
            FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite
        );
        await fileStream.WriteAsync(imageData, 0, imageData.Length, token);
        await fileStream.FlushAsync(token);
    }
}
