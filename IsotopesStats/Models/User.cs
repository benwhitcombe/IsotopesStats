using Postgrest.Attributes;
using Postgrest.Models;

namespace IsotopesStats.Models;

[Table("users")]
public class User : BaseModel
{
    [PrimaryKey("id", false)]
    public int Id { get; set; }
    
    [Column("email")]
    public string Email { get; set; } = string.Empty;
    
    [Column("passwordhash")]
    public string PasswordHash { get; set; } = string.Empty;
    
    [Column("createdat")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    [Column("isdeleted")]
    public bool IsDeleted { get; set; } = false;

    public List<UserRole> Roles { get; set; } = new();
}
