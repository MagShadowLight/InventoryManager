using InventBox.Core.Enums;

namespace InventBox.Core.Models;

/// <summary>
/// Represents a model for creating item instance.
/// </summary>
public class Items
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
    /// <summary>
    /// Gets or sets the quantity.
    /// </summary>
    public virtual int Quantity { get; set; }
    /// <summary>
    /// Gets or sets the serial number.
    /// </summary>
    public virtual string SerialNumber { get; set; } = string.Empty;
    /// <summary>
    /// Gets or sets the model number.
    /// </summary>
    public virtual string ModelNumber { get; set; } = string.Empty;
    /// <summary>
    /// Gets or sets the manufacturer.
    /// </summary>
    public virtual string Manufacturer { get; set; } = string.Empty;
    /// <summary>
    /// Gets or sets the notes.
    /// </summary>
    public virtual string Notes { get; set; } = string.Empty;
    /// <summary>
    /// Gets or sets the date time for created at.
    /// </summary>
    public virtual DateTime CreatedAt { get; set; } = DateTime.Now;
    /// <summary>
    /// Gets or sets the date time for updated at.
    /// </summary>
    public virtual DateTime UpdatedAt { get; set; } = DateTime.Now;
    /// <summary>
    /// Gets or sets the condition.
    /// </summary>
    public virtual Conditions Conditions { get; set; }
    /// <summary>
    /// Gets or sets the category.
    /// </summary>
    public virtual Category? Category {get; set; }
    /// <summary>
    /// Gets or sets the location.
    /// </summary>
    public virtual Locations? Locations {get; set; }
    /// <summary>
    /// Gets or sets the warrantly.
    /// </summary>
    public virtual Warrantly? Warrantly { get; set; }
    /// <summary>
    /// Gets or sets the insurance.
    /// </summary>
    public virtual Insurance? Insurance { get; set; }
}
