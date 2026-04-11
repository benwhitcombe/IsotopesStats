using System.ComponentModel.DataAnnotations;
using Postgrest.Attributes;
using Postgrest.Models;

namespace IsotopesStats.Models;

[Table("players")]
public class Player : BaseModel, IEntity
{
    [PrimaryKey("id", false)]
    public int Id { get; set; }
    
    [Column("name")]
    [Required(ErrorMessage = "Player name is required.")]
    public string Name { get; set; } = string.Empty;
    
    [Column("isdeleted")]
    public bool IsDeleted { get; set; } = false;
}
