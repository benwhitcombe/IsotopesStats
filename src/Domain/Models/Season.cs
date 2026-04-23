using System.ComponentModel.DataAnnotations;
namespace IsotopesStats.Domain.Models;

public record Season : IEntity
{
    public int Id { get; set; }
    
    [Required(ErrorMessage = "Season name is required.")]
    [StringLength(100, ErrorMessage = "Season name cannot exceed 100 characters.")]
    public string Name { get; set; } = string.Empty;
    
    public bool IsDeleted { get; set; } = false;
}
