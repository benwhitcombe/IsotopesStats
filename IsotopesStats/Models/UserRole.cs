namespace IsotopesStats.Models;

public class UserRole
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public List<Permission> Permissions { get; set; } = new();
    public bool IsActive { get; set; } = true;
}
