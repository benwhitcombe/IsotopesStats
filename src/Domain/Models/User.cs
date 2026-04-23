namespace IsotopesStats.Domain.Models;

public record User : IEntity<string>
{
    public string Id { get; set; } = string.Empty;
    
    public string Email { get; set; } = string.Empty;
    
    public string PasswordHash { get; set; } = string.Empty;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public bool IsDeleted { get; set; } = false;

    public List<UserRole> Roles { get; set; } = new();

    // Added Clone method to support shallow copies used in the UI refactors
}
