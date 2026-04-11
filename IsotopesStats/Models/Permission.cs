using Postgrest.Attributes;
using Postgrest.Models;

namespace IsotopesStats.Models;

[Table("permissions")]
public class Permission : BaseModel
{
    [PrimaryKey("id", false)]
    public int Id { get; set; }
    
    [Column("name")]
    public string Name { get; set; } = string.Empty;
}
