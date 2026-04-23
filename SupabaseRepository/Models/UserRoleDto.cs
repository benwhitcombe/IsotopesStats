using Postgrest.Attributes;
using Postgrest.Models;
using IsotopesStats.Models;

namespace SupabaseRepository.Models;

[Table("userroles")]
public class UserRoleDTO : BaseModel
{
    [PrimaryKey("id", false)]
    public long Id { get; set; }

    [Column("name")]
    public string Name { get; set; } = string.Empty;

    [Column("isdeleted")]
    public bool IsDeleted { get; set; } = false;

    [Reference(typeof(RolePermissionDTO), useInnerJoin: false)]
    public List<RolePermissionDTO> RolePermissions { get; set; } = new();
}
