using Postgrest.Attributes;
using Postgrest.Models;
using IsotopesStats.Domain.Models;

namespace IsotopesStats.SupabaseRepository.Models;

[Table("players")]
internal class PlayerDTO : BaseModel
{
    [PrimaryKey("id", false)]
    public int Id { get; set; }

    [Column("name")]
    public string Name { get; set; } = string.Empty;

    [Column("isdeleted")]
    public bool IsDeleted { get; set; } = false;
}

