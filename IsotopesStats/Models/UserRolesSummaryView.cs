using Postgrest.Attributes;
using Postgrest.Models;

namespace IsotopesStats.Models;

[Table("v_user_roles_summary")]
public class UserRolesSummaryView : BaseModel, IEntity<string>
{
    [PrimaryKey("userid", false)]
    public string Id { get; set; } = string.Empty; // Mapping userid to Id for IEntity compatibility

    [Column("email")]
    public string Email { get; set; } = string.Empty;

    [Column("createdat")]
    public DateTime CreatedAt { get; set; }

    [Column("rolenames")]
    public string RoleNames { get; set; } = string.Empty;

    [Column("isdeleted")]
    public bool IsDeleted { get; set; } = false;
}
