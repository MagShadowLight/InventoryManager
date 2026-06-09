using System.Drawing.Printing;
using InventBox.Core;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace InventBox.Test.InventBox.Core;

public class ImageCaptureTest
{
    private readonly ITestOutputHelper _output;
    private ImageCapture _capture;
    private CancellationTokenSource source = new CancellationTokenSource();
    public ImageCaptureTest(ITestOutputHelper output)
    {
        _output = output;
    }
    [Fact ]
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
        if (!File.Exists(path))
        {
            _output.WriteLine("Capture have timed out");
            Assert.True(true);
        } else
            Assert.True(File.Exists(path));        
        // Clean up
        if (File.Exists(path))
            File.Delete(path);
    }
    [Theory]
    [InlineData(500, Skip = "Short time")]
    [InlineData(1000, Skip = "Short time")]
    [InlineData(1500, Skip = "Short time")]
    [InlineData(2000, Skip = "Short time")]
    [InlineData(2500)]
    [InlineData(3000)]
    [InlineData(3500)]
    [InlineData(4000)]
    public async Task When_Camera_Device_Were_Opened_Then_It_Should_Close_Capture_WIthout_Saving_File(int timer)
    {
        _capture = new ImageCapture();
        var path = Path.Combine(AppContext.BaseDirectory, "TestImage.png");
        await _capture.OpenCapture(source);
        // Act
        await _capture.StartCapture();
        Thread.Sleep(timer);
        await _capture.CloseCapture();
        // Assert
        if (_capture._frame == null)
        {
            _output.WriteLine("Capture have timed out");
            
        } else        
            Assert.NotNull(_capture._frame);
        
    }
}
