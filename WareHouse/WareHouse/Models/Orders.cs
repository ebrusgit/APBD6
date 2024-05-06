namespace DefaultNamespace;
using System.ComponentModel.DataAnnotations;

public class Orders
{
    [Required]
    public int IdOrder { get; set; }
    [Required]
    public int IdProduct { get; set; }
    [Required]
    public int Amount { get; set; }
    [Required]
    public DateTime CreatedAt { get; set; }
    public DateTime FullfilledAt { get; set; }
    
}