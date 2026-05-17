using System.ComponentModel.DataAnnotations;

namespace InventBox.Core.Models;

public class Receipt
{
    public int Id { get; set; }
    public string StoreName { get; set; } = string.Empty;
    public double Price { get; set; }
}
