using InventBox.Core;

namespace InventBox.Test.InventBox.Core;

public class ImageCaptureTest
{
    private ImageCapture _capture;
    private CancellationTokenSource source = new CancellationTokenSource();
    [Fact]
    public async Task When_Camera_Device_Were_Opened_Then_It_Should_Start_Capture()
    {
        // Arrange
        _capture = new ImageCapture();
        var path = Path.Combine(AppContext.BaseDirectory, "TestImage.png");
        await _capture.OpenCapture(source);
        // Act
        await _capture.StartCapture();
        Thread.Sleep(1500);
        await _capture.StopCapture(path);
        // Assert
        Assert.True(File.Exists(path));
        // Clean up
        File.Delete(path);
    }
    [Fact]
    public async Task When_Camera_Device_Were_Opened_Then_It_Should_Close_Capture_WIthout_Saving_File()
    {
        _capture = new ImageCapture();
        var path = Path.Combine(AppContext.BaseDirectory, "TestImage.png");
        await _capture.OpenCapture(source);
        // Act
        await _capture.StartCapture();
        Thread.Sleep(1500);
        await _capture.CloseCapture();
        // Assert
        Assert.True(_capture._frame != null);
    }
}
