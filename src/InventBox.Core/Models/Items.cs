namespace InventBox.Core.Models;

public class Items
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public string SerialNumber { get; set; } = string.Empty;
    public string ModelNumber { get; set; } = string.Empty;
    public string Manufacturer { get; set; } = string.Empty;
    public bool Insured { get; set; }
    public string Notes { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
    public Conditions Conditions { get; set; }
}
