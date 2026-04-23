using Postgrest.Attributes;
using Postgrest.Models;
using IsotopesStats.Models;

namespace SupabaseRepository.Models;

[Table("useruserroles")]
public class UserUserRolesDTO : BaseModel
{
    [PrimaryKey("userid", false)]
    [Column("userid")]
    public Guid UserId { get; set; }

    [Column("roleid")]
    public long RoleId { get; set; }
}
