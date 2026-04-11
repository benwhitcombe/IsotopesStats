using Postgrest.Attributes;
using Postgrest.Models;

namespace IsotopesStats.Models;

[Table("userroles")]
public class UserRole : BaseModel, IEntity
{
    [PrimaryKey("id", false)]
    public int Id { get; set; }
    
    [Column("name")]
    public string Name { get; set; } = string.Empty;
    
    [Column("isdeleted")]
    public bool IsDeleted { get; set; } = false;

    public List<Permission> Permissions { get; set; } = new();

    public UserRole Clone() => (UserRole)this.MemberwiseClone();
}
