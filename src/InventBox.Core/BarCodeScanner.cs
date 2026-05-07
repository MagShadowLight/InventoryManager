
using SixLabors.ImageSharp.PixelFormats;
using ZXing.ImageSharp;
using ZXing.Common;
using ZXing;
using SixLabors.ImageSharp;
using ZXing.OneD;
using ZXing.ImageSharp.Rendering;

namespace InventBox.Core;

public class BarCodeScanner
{
    public string DecodeBarCode(string path, BarcodeFormat format = BarcodeFormat.CODE_128, bool tryHarder = true, bool tryInverted = true)
    {
        
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
}
