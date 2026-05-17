using System.ComponentModel.DataAnnotations;

namespace InventBox.Core.Models;

public class Warrantly
{
    public virtual int Id { get; set; }
    public virtual DateTime StartDate { get; set; }
    public virtual DateTime EndDate { get; set; }
    public virtual Status Status { get; set; }
    public virtual string Provider { get; set; } = string.Empty;
    public virtual string ContactNumber { get; set; } = string.Empty;
}
