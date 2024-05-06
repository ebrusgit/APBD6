using System.ComponentModel.DataAnnotations;

namespace DefaultNamespace;

public class Warehouse
{
    [Required]
    public int IdWarehouse { get; set; }
    [Required]
    [MaxLength(200)]
    public string Name { get; set; }
    [Required]
    [MaxLength(200)]
    public string Address { get; set; }
    [Required]
    [MaxLength(5)]
    public string test { get; set; }
    
}