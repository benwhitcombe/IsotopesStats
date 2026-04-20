using Postgrest.Attributes;
using Postgrest.Models;
using IsotopesStats.Models;

namespace SupabaseRepository.Models;

[Table("rolepermissions")]
public class RolePermissionDto : BaseModel
{
    [Column("roleid")]
    public int RoleId { get; set; }

    [Column("permissionid")]
    public int PermissionId { get; set; }
}
