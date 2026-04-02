namespace IsotopesStats.Models;

public class Permission
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
}

public class UserRole
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public List<Permission> Permissions { get; set; } = new();
}

public class User
{
    public int Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public int RoleId { get; set; }
    public UserRole? Role { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
