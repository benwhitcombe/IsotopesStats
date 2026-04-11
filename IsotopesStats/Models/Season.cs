using System.ComponentModel.DataAnnotations;
using Postgrest.Attributes;
using Postgrest.Models;

namespace IsotopesStats.Models;

[Table("seasons")]
public class Season : BaseModel, IEntity
{
    [PrimaryKey("id", false)]
    public int Id { get; set; }
    
    [Column("name")]
    [Required(ErrorMessage = "Season name is required.")]
    [StringLength(100, ErrorMessage = "Season name cannot exceed 100 characters.")]
    public string Name { get; set; } = string.Empty;
    
    [Column("isdeleted")]
    public bool IsDeleted { get; set; } = false;
}
