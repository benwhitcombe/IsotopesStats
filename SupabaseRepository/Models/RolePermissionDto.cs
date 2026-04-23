using Postgrest.Attributes;
using Postgrest.Models;
using IsotopesStats.Models;

namespace SupabaseRepository.Models;

[Table("rolepermissions")]
public class RolePermissionDTO : BaseModel
{
    [PrimaryKey("roleid", false)]
    [Column("roleid")]
    public int RoleId { get; set; }

    [PrimaryKey("permissionid", false)]
    [Column("permissionid")]
    public int PermissionId { get; set; }

    [Reference(typeof(PermissionDTO), useInnerJoin: false)]
    public PermissionDTO? Permission { get; set; }
}
