namespace InventBox.Core.Models;

public class Locations
{
    public virtual int Id { get; set; }
    public virtual string Floor { get; set; } = string.Empty;
    public virtual string Room { get; set; } = string.Empty;
    public virtual string Container { get; set; } = string.Empty;
    public virtual int X { get; set; }
    public virtual int Y { get; set; }
}
