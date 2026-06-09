namespace InventBox.Core.Models;

/// <summary>
/// Represents a model for creating insurance instance.
/// </summary>
public class Insurance
{
    /// <summary>
    /// Gets or sets the id.
    /// </summary>
    public virtual int Id { get; set; }
    /// <summary>
    /// Gets or sets the start date.
    /// </summary>
    public virtual DateTime StartDate { get; set; }
    /// <summary>
    /// Gets or sets the end date.
    /// </summary>
    public virtual DateTime EndDate { get; set; }
    /// <summary>
    /// Gets or sets the provider.
    /// </summary>
    public virtual string Provider { get; set; } = string.Empty;
    /// <summary>
    /// Gets or sets the contact number.
    /// </summary>
    public virtual string ContactNumber { get; set; } = string.Empty;
    /// <summary>
    /// Gets or sets the insured.
    /// </summary>
    public virtual Status Insured { get; set; }
}
