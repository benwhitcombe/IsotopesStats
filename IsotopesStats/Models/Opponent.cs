using System.ComponentModel.DataAnnotations;
using Postgrest.Attributes;
using Postgrest.Models;

namespace IsotopesStats.Models;

[Table("opponents")]
public class Opponent : BaseModel, IEntity
{
    [PrimaryKey("id", false)]
    public int Id { get; set; }
    
    [Column("name")]
    [Required(ErrorMessage = "Opponent name is required.")]
    public string Name { get; set; } = string.Empty;
    
    [Column("isdeleted")]
    public bool IsDeleted { get; set; } = false;
}
