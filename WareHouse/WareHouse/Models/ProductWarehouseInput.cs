using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace DefaultNamespace;

public class ProductWarehouseInput
{
    [Required]
    public int IdProduct { get; set; }
    [Required]
    public int IdWarehouse { get; set; }
    [Required]
    
    public int Amount { get; set; }
    [Required]
    public DateTime CreatedAt { get; set; }
}