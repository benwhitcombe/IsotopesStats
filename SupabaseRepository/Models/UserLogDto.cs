using Postgrest.Attributes;
using Postgrest.Models;
using IsotopesStats.Models;

namespace SupabaseRepository.Models;

[Table("userlogs")]
public class UserLogDTO : BaseModel
{
    [PrimaryKey("id", false)]
    public int Id { get; set; }

    [Column("userid")]
    public string? UserId { get; set; }

    [Column("useremail")]
    public string UserEmail { get; set; } = string.Empty;

    [Column("action")]
    public UserLogAction Action { get; set; }

    [Column("entitytype")]
    public string EntityType { get; set; } = string.Empty;

    [Column("entityid")]
    public string EntityId { get; set; } = string.Empty;

    [Column("description")]
    public string Description { get; set; } = string.Empty;

    [Column("timestamp")]
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}
