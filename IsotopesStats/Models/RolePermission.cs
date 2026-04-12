using Postgrest.Attributes;
using Postgrest.Models;

namespace IsotopesStats.Models;

[Table("rolepermissions")]
public class RolePermission : BaseModel
{
    [Column("roleid")]
    public int RoleId { get; set; }

    [Column("permissionid")]
    public int PermissionId { get; set; }
}
