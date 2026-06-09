namespace InventBox.Core.Models;

/// <summary>
/// Represents a model for creating location instance.
/// </summary>
public class Locations
{
    /// <summary>
    /// Gets or sets the id.
    /// </summary>
    public virtual int Id { get; set; }
    /// <summary>
    /// Gets or sets the floor number.
    /// </summary>
    public virtual string Floor { get; set; } = string.Empty;
    /// <summary>
    /// Gets or sets the room.
    /// </summary>
    public virtual string Room { get; set; } = string.Empty;
    /// <summary>
    /// Gets or sets the container.
    /// </summary>
    public virtual string Container { get; set; } = string.Empty;
    /// <summary>
    /// Gets or sets the first part of coordinate, X.
    /// </summary>
    public virtual int X { get; set; }
    /// <summary>
    /// Gets or sets the second part of coordinate, Y.
    /// </summary>
    public virtual int Y { get; set; }
}
