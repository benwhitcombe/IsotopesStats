namespace IsotopesStats.Domain.Models;

public record RolePermission
{
    public int RoleId { get; set; }

    public int PermissionId { get; set; }

    public Permission? Permission { get; set; }
}
