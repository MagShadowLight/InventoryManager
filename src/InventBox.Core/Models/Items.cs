namespace InventBox.Core.Models;

public class Items
{
    public virtual int Id { get; set; }
    public virtual string Name { get; set; } = string.Empty;
    public virtual string Description { get; set; } = string.Empty;
    public virtual int Quantity { get; set; }
    public virtual string SerialNumber { get; set; } = string.Empty;
    public virtual string ModelNumber { get; set; } = string.Empty;
    public virtual string Manufacturer { get; set; } = string.Empty;
    public virtual bool Insured { get; set; }
    public virtual string Notes { get; set; } = string.Empty;
    public virtual DateTime CreatedAt { get; set; } = DateTime.Now;
    public virtual DateTime UpdatedAt { get; set; } = DateTime.Now;
    public virtual Conditions Conditions { get; set; }
    public virtual Category? Category {get; set; }
    public virtual Locations? Locations {get; set; }
    public virtual Warrantly? Warrantly { get; set; }
}
