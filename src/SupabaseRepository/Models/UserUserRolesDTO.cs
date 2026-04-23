using Postgrest.Attributes;
using Postgrest.Models;
using IsotopesStats.Domain.Models;

namespace IsotopesStats.SupabaseRepository.Models;

[Table("useruserroles")]
internal class UserUserRolesDTO : BaseModel
{
    [PrimaryKey("userid", false)]
    [Column("userid")]
    public string UserId { get; set; } = string.Empty;

    [Column("roleid")]
    public int RoleId { get; set; }
}

