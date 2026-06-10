using InventBox.Core;
using InventBox.Core.Models;
using Xunit.Abstractions;

namespace InventBox.Tests.InventBox.Core;

public class BarCodeScannerTest
{
    private readonly ITestOutputHelper _output;
    private BarCodeScanner _scanner = new BarCodeScanner(string.Empty);
    private string str_result = string.Empty;
    private static Category category = new Category(){Id = 1, Name = "Test", Description = "This is test"};
    [Theory]
    [InlineData("./Data/barcodeTest")]
    public void Given_There_Is_BarCode_In_The_File_When_It_Decode_The_Barcode_Then_It_Should_Return_String(string path)
    {
        // Arrange
        string fullpath = Path.GetFullPath(path);
        _scanner = new BarCodeScanner(fullpath);
        // Act
        str_result = _scanner.DecodeBarCode(Path.Combine(AppContext.BaseDirectory, "Data", "barcodeTest"));
        // Assert
        Assert.Contains("Test 1", str_result);
    }
    [Theory]
    [InlineData("This is message that going to be encoded")]
    [InlineData("Test 1")]
    [InlineData("")]
    public void Given_There_Is_String_Message_When_It_Encode_Into_The_Barcode_Then_It_Should_Return_The_Value_From_The_Barcode(string message)
    {
        // Arrange
        string path = "BarCodeExport.png";
        _scanner = new BarCodeScanner(Path.Combine("Logs", "InventBox.log"));
        // Act
        _scanner.EncodeBarCode(message, path);
        byte[]? barcode = new byte[]{};
        if (File.Exists(path))
            barcode = File.ReadAllBytes(path);
        str_result = _scanner.TryScanBarCode(barcode)!;
        // Assert
        Assert.Contains(message, str_result);
        File.Delete(path);
    }
    [Fact]
    public void Given_There_Is_Invalid_File_When_It_Tried_To_Decode_It_Then_It_Should_Return_Null_Or_Empty()
    {
        // Arrange
        _scanner = new BarCodeScanner(Path.Combine("Logs", "InventBox.log"));
        byte[] bytes = new byte[] {1,2,3,4,5,6,7};
        byte[] emptyBytes = new byte[]{};
        // Act
        string result1 = _scanner.DecodeBarCode("InvalidBarCode.png");
        string? result2 = _scanner.TryScanBarCode(bytes);
        string? result3 = _scanner.TryScanBarCode(emptyBytes);
        // Assert
        Assert.True(string.IsNullOrEmpty(result1));
        Assert.True(string.IsNullOrEmpty(result2));
        Assert.True(string.IsNullOrEmpty(result3));
    }
}
