using System.ComponentModel.DataAnnotations;

namespace IsotopesStats.Models;

public record class Player : IEntity
{
    public int Id { get; set; }
    
    [Required(ErrorMessage = "Player name is required.")]
    [StringLength(100, ErrorMessage = "Player name cannot exceed 100 characters.")]
    public string Name { get; set; } = string.Empty;
    
    public bool IsDeleted { get; set; } = false;
}
