using System.ComponentModel.DataAnnotations;

namespace InventBox.Core.Models;

public class Warrantly
{
    [Key]
    public int Id { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public Status Status { get; set; }
    public string Provider { get; set; } = string.Empty;
    public string ContactNumber { get; set; } = string.Empty;
}
