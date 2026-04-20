using System.ComponentModel.DataAnnotations;
namespace IsotopesStats.Models;

public record Player : IEntity
{
    public int Id { get; set; }
    
    [Required(ErrorMessage = "Player name is required.")]
    public string Name { get; set; } = string.Empty;
    
    public bool IsDeleted { get; set; } = false;
}
