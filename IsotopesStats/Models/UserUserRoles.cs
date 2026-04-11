using Postgrest.Attributes;
using Postgrest.Models;

namespace IsotopesStats.Models;

[Table("useruserroles")]
public class UserUserRoles : BaseModel
{
    [Column("userid")]
    public string UserId { get; set; } = string.Empty;

    [Column("roleid")]
    public int RoleId { get; set; }
}
