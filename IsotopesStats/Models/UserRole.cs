using System.ComponentModel.DataAnnotations;

namespace IsotopesStats.Models;

public record class UserRole
{
    public int Id { get; set; }
    
    [Required(ErrorMessage = "Role name is required.")]
    [StringLength(100, ErrorMessage = "Role name cannot exceed 100 characters.")]
    public string Name { get; set; } = string.Empty;
    
    public List<Permission> Permissions { get; set; } = new();
    public bool IsDeleted { get; set; } = false;
}
