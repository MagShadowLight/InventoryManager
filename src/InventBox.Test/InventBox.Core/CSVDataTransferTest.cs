using InventBox.Core;
using Xunit.Abstractions;

namespace InventBox.Test.InventBox.Core;

public class CSVDataTransferTest
{
    string path = "Test.csv";
    CSVDataTransfer csvparser = new CSVDataTransfer();
    private readonly ITestOutputHelper _output;
    public CSVDataTransferTest(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact]
    public void WhenUserImportTheDataFromTheCSVThenValuesShouldBeStoredFromTheCSV()
    {
        // Arrange
        List<string> result = new List<string>();
        csvparser = new CSVDataTransfer();
        // Act
        csvparser.Import(path);
        result = csvparser.textList;
        _output.WriteLine($"This should have value of: {string.Join(",", result)}");
        // Assert
        Assert.Equal(10, result.Count);
        Assert.Contains("Test1", result[0]);
        Assert.Contains("Test2", result[1]);
        Assert.Contains("Test3", result[2]);
        Assert.Contains("Test4", result[3]);
        Assert.Contains("Test5", result[4]);
        Assert.Contains("Test6", result[5]);
        Assert.Contains("Test7", result[6]);
        Assert.Contains("Test8", result[7]);
        Assert.Contains("Test9", result[8]);
        Assert.Contains("Test10", result[9]);
    }
    [Fact]
    public void WhenUserExportTheDataIntoACSVFileThenValuesShouldBeStoredInCSVFile()
    {
        // Arrange
        List<string> text = new List<string>() {"Test1", "Test2", "Test3", "Test4", "Test5", "Test6", "Test7", "Test8", "Test9", "Test10"};
        int count = text.Count;
        List<string> result = new List<string>();
        csvparser = new CSVDataTransfer();
        // Act
        csvparser.Export(text.ToArray(), "TestExport.csv");
        csvparser.Import("TestExport.csv");
        result = csvparser.textList;
        // Assert
        Assert.Equal(10, result.Count);
        CleanUpCSV("TestExport.csv");
    }
    private void CleanUpCSV(string path)
    {
        File.Delete(path);
    }
}
