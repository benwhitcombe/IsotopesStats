namespace IsotopesStats.Domain.Models;

public record UserUserRoles
{
    public string UserId { get; set; } = string.Empty;

    public int RoleId { get; set; }
}
