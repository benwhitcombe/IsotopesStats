using Postgrest.Attributes;
using Postgrest.Models;

namespace IsotopesStats.Models;

public enum UserLogAction
{
    Created = 0,
    Updated = 1,
    Deleted = 2
}

[Table("userlogs")]
public class UserLog : BaseModel, IEntity
{
    [PrimaryKey("id", false)]
    public int Id { get; set; }
    
    [Column("userid")]
    public int? UserId { get; set; }
    
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
