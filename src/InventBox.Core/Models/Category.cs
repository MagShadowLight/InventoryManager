namespace InventBox.Core.Models;

/// <summary>
/// Represents a model for creating category instance.
/// </summary>
public class Category
{
    /// <summary>
    /// Gets or sets the id.
    /// </summary>
    public virtual int Id { get; set; }
    /// <summary>
    /// Gets or sets the name.
    /// </summary>
    public virtual string Name { get; set; } = string.Empty;
    /// <summary>
    /// Gets or sets the description.
    /// </summary>
    public virtual string Description { get; set; } = string.Empty;
}
