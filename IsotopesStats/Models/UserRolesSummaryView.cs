using Postgrest.Attributes;
using Postgrest.Models;

namespace IsotopesStats.Models;

[Table("v_user_roles_summary")]
public class UserRolesSummaryView : BaseModel, IEntity
{
    [PrimaryKey("userid", false)]
    public int Id { get; set; } // Mapping userid to Id for IEntity compatibility

    [Column("email")]
    public string Email { get; set; } = string.Empty;

    [Column("createdat")]
    public DateTime CreatedAt { get; set; }

    [Column("rolenames")]
    public string RoleNames { get; set; } = string.Empty;
}
