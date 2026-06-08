using InventBox.Core.Models;
using InventBox.Core.Utils;

namespace InventBox.Test.InventBox.Core.Utils;

public class EnumUtilsTest
{
    private List<Conditions> conditions;

    [Fact]
    public void When_There_Is_Values_In_Enum_Then_It_Should_Add_To_List()
    {
        // Arrange
        conditions = new List<Conditions>();
        // Act
        conditions = EnumUtils<Conditions>.GetEnumList<Conditions>();
        // Assert
        Assert.Contains(Conditions.Good, conditions);
    }
}
