namespace InventBox.Core.Models;

public class Receipt
{
    public int Id { get; set; }
    public string StoreName { get; set; } = string.Empty;
    public double Price { get; set; }
    public double Taxes { get; set; }
    public double TotalAmount { get; set; }
    public string InvoicePath { get; set; } = string.Empty;
}
