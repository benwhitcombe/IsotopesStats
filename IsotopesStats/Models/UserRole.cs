namespace IsotopesStats.Models;

public record class UserRole
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public List<Permission> Permissions { get; set; } = new();
    public bool IsDeleted { get; set; } = false;
}
