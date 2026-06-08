using InventBox.Core;
using InventBox.Core.Models;
using Xunit.Abstractions;

namespace InventBox.Test.InventBox.Core;

public class JsonParserTest
{
    private readonly ITestOutputHelper _output;
    private JsonParser<Category> _parser;

    private string str_result = string.Empty;
    private Category category_result = new Category();

    public JsonParserTest(ITestOutputHelper output)
    {
        _output = output;
    }
    [Fact]
    public void When_There_Is_Values_In_The_Object_Then_It_Should_Return_As_A_Json()
    {
        // Arrange
        var category = new Category{Id = 1, Name = "Test", Description = "This is test that going to be parse as JSON!"};
        _parser = new JsonParser<Category>();
        // Act
        str_result = _parser.ParseJson(category);
        // Assert
        _output.WriteLine(str_result);
        Assert.Contains("Test", str_result);
    }
    [Fact]
    public void When_There_Is_Objects_In_JSON_Then_It_Should_Return_As_A_Object()
    {
        // Arrange
        var json = """
        {
            "Id": 1,
            "Name": "Test",
            "Description": "This is test that going to be deparse as Object!"
        }
        """;
        _parser = new JsonParser<Category>();
        // Act
        category_result = _parser.DeParseJson(json)!;
        // Arrange
        _output.WriteLine($"Id: {category_result.Id}\nName:{category_result.Name}\nDescription:{category_result.Description}");
        Assert.Contains("Test", category_result.Name);
    }
}
