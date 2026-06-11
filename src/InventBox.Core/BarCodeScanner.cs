
using SixLabors.ImageSharp.PixelFormats;
using ZXing.ImageSharp;
using ZXing.Common;
using ZXing;
using SixLabors.ImageSharp;
using ZXing.OneD;
using ZXing.ImageSharp.Rendering;

namespace InventBox.Core;

/// <summary>
/// Represents the scanner for bar code.
/// </summary>
public class BarCodeScanner
{
    /// <summary>
    /// Initalize the logger for file.
    /// </summary>
    private FileLogger _logger = new FileLogger();
    /// <summary>
    /// The path for the logger.
    /// </summary>
    private string _loggerPath = string.Empty;
    /// <summary>
    /// Initalize a new instance of the <see cref="BarCodeScanner"/>
    /// </summary>
    /// <param name="path">The path for the logger.</param>
    public BarCodeScanner(string path)
    {
        _loggerPath = path;
    }

    /// <summary>
    /// Decode the bar code from the file into a string.
    /// </summary>
    /// <param name="path">The path for the bar code image.</param>
    /// <param name="format">The format for the bar code.</param>
    /// <param name="tryHarder"></param>
    /// <param name="tryInverted"></param>
    /// <returns></returns>
    public string DecodeBarCode(string path, BarcodeFormat format = BarcodeFormat.CODE_128, bool tryHarder = true, bool tryInverted = true)
    {
        try {
            _logger.Logs("Scanning the bar code", _loggerPath);
            var reader = CreateReader(format, tryHarder, tryInverted);

            using var image = Image.Load<Rgba32>(path);
            var result = reader.Decode(image);
            _logger.Logs("Bar code scanned successfully.", _loggerPath);
            return result.Text;
        } catch (Exception ex)
        {
            _logger.Error($"Failed to read the bar code. Message: {ex.Message}", _loggerPath);
            return string.Empty;
        }
    }

    /// <summary>
    /// Try to scan the bar code from the byte array.
    /// </summary>
    /// <param name="data">the byte array from the image.</param>
    /// <param name="format">The format for the bar code.</param>
    /// <param name="tryHarder"></param>
    /// <param name="tryInverted"></param>
    /// <returns></returns>
     public string? TryScanBarCode(byte[] data, BarcodeFormat format = BarcodeFormat.CODE_128, bool tryHarder = true, bool tryInverted = true)
    {
        try {
            if (data.Length == 0)
                return string.Empty;

            _logger.Logs("Trying to scan bar code.", _loggerPath);
            var reader = CreateReader(format, tryHarder, tryInverted);

            using var stream = new MemoryStream(data);
            using var image = Image.Load<Rgba32>(stream);
            var result = reader.Decode(image);
            _logger.Logs("Bar code scanned successfully.", _loggerPath);
            return result?.Text;
        }   catch (Exception ex)
        {
            _logger.Error($"Failed to read the bar code. Message: {ex.Message}", _loggerPath);
            return string.Empty;
        }
    }

    /// <summary>
    /// Encode the message into a bar code and save it into images.
    /// </summary>
    /// <param name="text">The message for the barcode.</param>
    /// <param name="path">The path to be place for barcode.</param>
    /// <param name="height">The height of the barcode.</param>
    /// <param name="width">The width of the barcode.</param>
    /// <param name="margin"><The margin for the barcode./param>
    /// <param name="foreground">The foreground color for the barcode.</param>
    /// <param name="background">The background color for the barcode.</param>
    /// <param name="format">The format for the barcode.</param>
    public void EncodeBarCode(string text, string path, int height = 100, int width = 100, int margin = 10, string foreground = "000000", string background = "FFFFFF", BarcodeFormat format = BarcodeFormat.CODE_128)
    {
        try {
            _logger.Logs("Generating a new bar code.", _loggerPath);
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
            _logger.Logs("Bar code generated successfully.", _loggerPath);
        } catch (Exception ex)
        {
            _logger.Error($"Failed to write the bar code. Message: {ex.Message}", _loggerPath);
        }
    }

    /// <summary>
    /// Initialize the reader for the bar code.
    /// </summary>
    /// <param name="format">The format for the bar code.</param>
    /// <param name="tryHarder"></param>
    /// <param name="tryInverted"></param>
    /// <returns></returns>
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
