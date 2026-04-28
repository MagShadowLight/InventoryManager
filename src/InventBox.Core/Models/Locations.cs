namespace InventBox.Core.Models;

public class Locations
{
    public int Id { get; set; }
    public string Floor { get; set; } = string.Empty;
    public string Room { get; set; } = string.Empty;
    public string Container { get; set; } = string.Empty;
    public int x { get; set; }
    public int y { get; set; }
}
