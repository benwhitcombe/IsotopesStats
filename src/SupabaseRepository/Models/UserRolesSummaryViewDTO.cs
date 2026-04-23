using Postgrest.Attributes;
using Postgrest.Models;
using IsotopesStats.Domain.Models;

namespace IsotopesStats.SupabaseRepository.Models;

[Table("v_user_roles_summary")]
internal class UserRolesSummaryViewDTO : BaseModel
{
    [PrimaryKey("userid", false)]
    public string Id { get; set; } = string.Empty;

    [Column("email")]
    public string Email { get; set; } = string.Empty;

    [Column("createdat")]
    public DateTime CreatedAt { get; set; }

    [Column("rolenames")]
    public string RoleNames { get; set; } = string.Empty;

    [Column("isdeleted")]
    public bool IsDeleted { get; set; } = false;
}

