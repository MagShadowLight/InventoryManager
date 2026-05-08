
using SixLabors.ImageSharp.PixelFormats;
using ZXing.ImageSharp;
using ZXing.Common;
using ZXing;
using SixLabors.ImageSharp;
using ZXing.OneD;
using ZXing.ImageSharp.Rendering;
using Eto.Forms;

namespace InventBox.Core;

public class BarCodeScanner
{
    private FileLogger _logger = new FileLogger();
    private string _loggerPath = string.Empty;
    public BarCodeScanner(string path)
    {
        _loggerPath = path;
    }

    public string DecodeBarCode(string path, BarcodeFormat format = BarcodeFormat.CODE_128, bool tryHarder = true, bool tryInverted = true)
    {
        try {
            var reader = new ZXing.BarcodeReader<Image<Rgba32>>(image => new ImageSharpLuminanceSource<Rgba32>(image))
            {
                AutoRotate = true,
                Options = new DecodingOptions
                {
                    PossibleFormats = new[] { format },
                    TryHarder = tryHarder,
                    TryInverted = tryInverted
                }
            };

            using var image = Image.Load<Rgba32>(path);
            var result = reader.Decode(image);
            return result.Text;
        } catch (Exception ex)
        {
            _logger.Error($"Failed to read the bar code. Message: {ex.Message}", _loggerPath);
            return string.Empty;
        }
    }

     public string? ScanBarCode(byte[] data, BarcodeFormat format = BarcodeFormat.CODE_128, bool tryHarder = true, bool tryInverted = true)
    {
        if (data == null || data.Length == 0)
            return null;

        var reader = CreateReader(format, tryHarder, tryInverted);

        using var stream = new MemoryStream(data);
        using var image = Image.Load<Rgba32>(stream);
        var result = reader.Decode(image);
        return result?.Text;
    }

    public void EncodeBarCode(string text, string path, int height = 100, int width = 100, int margin = 10, string foreground = "000000", string background = "FFFFFF", BarcodeFormat format = BarcodeFormat.CODE_128)
    {
        var writer = new ZXing.ImageSharp.BarcodeWriter<Rgba32>()
        {
            Format = format,
            Options = new Code128EncodingOptions
            {
                Height = height,
                Width = width,
                Margin = margin
            },
            Renderer = new ImageSharpRenderer<Rgba32>
            {
                Foreground = Rgba32.ParseHex(foreground),
                Background = Rgba32.ParseHex(background)
            }
        };

        using var image = writer.Write(text);
        image.SaveAsPng(path);
    }

    public ZXing.BarcodeReader<Image<Rgba32>> CreateReader(BarcodeFormat format = BarcodeFormat.CODE_128, bool tryHarder = true, bool tryInverted = true)
    {        
        return new ZXing.BarcodeReader<Image<Rgba32>> (image => new ImageSharpLuminanceSource<Rgba32>(image))
        {
            AutoRotate = true,
            Options = new DecodingOptions
            {
                PossibleFormats = new[] { format },
                TryHarder = tryHarder,
                TryInverted = tryInverted
            }
        };
    }
}
