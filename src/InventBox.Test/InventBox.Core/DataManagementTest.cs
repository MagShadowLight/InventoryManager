using InventBox.Core;
using InventBox.Core.Models;
using InventBox.Test.Utils;
using Moq;

namespace InventBox.Test.InventBox.Core;

public class DataTransferTest
{
    private string[] _paths = ["DataTransfer.log","ItemExportTest.csv"];
    private List<Items>? _items;
    private string _path = "ItemTest.csv";
    [Fact]
    public void WhenUserAreImportingTheDataFromTheCSVFileThenItShouldBeStoredInAnObject()
    {
        // Arrange
        var csv = new DataManagement<Items>("DataTransfer.log");
        // Act
        _items = csv.Load(_path);
        // Assert
        Assert.Equal(3, _items.Count);
        Assert.Contains("Item 1", _items.Where(x => x.Name == "Item 1").First().Name);
        Assert.Contains("Item 2", _items.Where(x => x.Name == "Item 2").First().Name);
        Assert.Contains("Item 3", _items.Where(x => x.Name == "Item 3").First().Name);
        // Clean up
        FileUtils.CleanUpFile(_paths);
    }
    [Fact]
    public void WhenUserAreExportingTheDataIntoCSVFileThenItShouldBeStoredInAnCSVFile()
    {
        // Arrange
        List<Items> items = new List<Items>()
        {
            new Items { Id = 1, Name = "Test 1", Description = "This is a test", Quantity = 1, SerialNumber = "1bhabhahnaBHHBI", ModelNumber = "abhahbbhaba", Manufacturer = "Manufacturer 1", Insured = true, Notes = "This is a test note 1", CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now, Conditions = Conditions.Good},
            new Items { Id = 2, Name = "Test 2", Description = "This is another test", Quantity = 10, SerialNumber = "nhnanjNJMIHUVWGYUICOIJHUY", ModelNumber = "HQACBACSINSHUCBHYUISUBHS", Manufacturer = "Manufacturer 2", Insured = false, Notes = "This is a test note 2", CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now, Conditions = Conditions.Acceptable},
            new Items { Id = 3, Name = "Test 3", Description = "This is third test", Quantity = 3, SerialNumber = "hqwbnujwikJAUSHBWEVFDASJICHUDBEAUD", ModelNumber = "nqiubucuiaiuch", Manufacturer = "Manufacturer 3", Insured = false, Notes = "This is a test note 3", CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now, Conditions = Conditions.Excellent}
        };
        var csvParser = new DataManagement<Items>("DataTransfer.log");
        // Act
        csvParser.Save(items, "ItemExportTest.csv");
        _items = csvParser.Load("ItemExportTest.csv");
        // Assert
        Assert.Equal(3, _items.Count);
        Assert.Equal(items[0].Name, _items[0].Name);
        Assert.Equal(items[1].Name, _items[1].Name);
        Assert.Equal(items[2].Name, _items[2].Name);
        // Clean up
        FileUtils.CleanUpFile(_paths);
    }
    [Fact]
    public void WhenUserTriedToExportAnEmptyDataIntoCSVFileThenItShouldThrowExceptionInsteadOfStoringInCSVFile()
    {
        // Arrange
        List<Locations> locations = new List<Locations>();
        var csvParser = new DataManagement<Locations>("DataTransfer.log");
        string result = string.Empty;
        // Act
        csvParser.Save(locations, "LocationFailed.csv");
        result = File.ReadAllText("DataTransfer.log");
        // Assert
        Assert.Contains("[ERROR] data is empty", result);
        FileUtils.CleanUpFile(_paths);
    }
        [Fact]
    public void WhenUserTriedToImportANonExistentCSVFileIntoADataThenItShouldThrowException()
    {
        // Arrange
        List<Receipt> receipts = new List<Receipt>();
        var csvParser = new DataManagement<Receipt>(_paths[0]);
        string result = string.Empty;
        // Act
        csvParser.Load("NonExistentData.csv");
        result = File.ReadAllText(_paths[0]);
        // Assert
        Assert.Contains("[ERROR] Could not find file", result);
        FileUtils.CleanUpFile(_paths);
    }
}
