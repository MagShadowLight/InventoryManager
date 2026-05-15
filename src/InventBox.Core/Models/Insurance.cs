using System.ComponentModel.DataAnnotations;

namespace InventBox.Core.Models;

public class Insurance
{
    public virtual int Id { get; set; }
    public virtual DateTime StartDate { get; set; }
    public virtual DateTime EndDate { get; set; }
    public virtual string Provider { get; set; } = string.Empty;
    public virtual string ContactNumber { get; set; } = string.Empty;
    public virtual Status Insured { get; set; }
}
