using InventBox.Core.DataTransfer;
using InventBox.Core.Models;
using InventBox.Test.Utils;

namespace InventBox.Test.InventBox.Core.DataTransfer;

public class CategoryDataTransferTest
{
    private List<Category>? _category;
    private string _path = "CategoryTest.csv";
    [Fact]
    public void WhenUserAreImportingTheCategoryDataFromCSVFileThenItShouldBeStoredInAnCategoryList()
    {
        // Arrange
        var csv = new CategoryDataTransfer();
        // Act
        _category = csv.Import(_path);
        // Assert
        Assert.Equal(3, _category.Count);
        Assert.Contains("Category 1", _category.Where(x => x.Name == "Category 1").First().Name);
        Assert.Contains("Category 2", _category.Where(x => x.Name == "Category 2").First().Name);
        Assert.Contains("Category 3", _category.Where(x => x.Name == "Category 3").First().Name);
    }
    [Fact]
    public void WhenUserAreExportingTheCategoriesDataIntoCSVFileThenItShouldBeStoredInAnCSVFile()
    {
        // Arrange
        List<Category> categories = new List<Category>()
        {
            new Category {Id = 1, Name = "Category 1", Description = "This is Test category 1"},
            new Category {Id = 2, Name = "Category 2", Description = "This is Test category 2"}, 
            new Category {Id = 3, Name = "Category 3", Description = "This is Test category 3"} 
        };
        var csv = new CategoryDataTransfer();
        // Act
        csv.Export(categories, "CategoryExportTest.csv");
        _category = csv.Import("CategoryExportTest.csv");
        // Assert
        Assert.Equal(3, _category.Count);
        Assert.Equal(_category[0].Name, categories[0].Name);
        Assert.Equal(_category[1].Name, categories[1].Name);
        Assert.Equal(_category[2].Name, categories[2].Name);
        // Clean up
        FileUtils.CleanUpFile("CategoryExportTest.csv");
    }
}
