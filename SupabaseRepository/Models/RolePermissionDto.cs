using Postgrest.Attributes;
using Postgrest.Models;
using IsotopesStats.Models;

namespace SupabaseRepository.Models;

[Table("rolepermissions")]
public class RolePermissionDto : BaseModel
{
    [PrimaryKey("roleid", false)]
    [Column("roleid")]
    public long RoleId { get; set; }

    [PrimaryKey("permissionid", false)]
    [Column("permissionid")]
    public long PermissionId { get; set; }

    [Reference(typeof(PermissionDto), useInnerJoin: false)]
    public PermissionDto? Permission { get; set; }
}
