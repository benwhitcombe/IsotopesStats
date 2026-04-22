using System.ComponentModel.DataAnnotations;
namespace IsotopesStats.Models;

public record Opponent : IEntity
{
    public int Id { get; set; }
    
    [Required(ErrorMessage = "Opponent name is required.")]
    public string Name { get; set; } = string.Empty;

    public string ShortName { get; set; } = string.Empty;
    
    public bool IsDeleted { get; set; } = false;
}
