using Postgrest.Attributes;
using Postgrest.Models;
using IsotopesStats.Models;

namespace SupabaseRepository.Models;

[Table("seasonplayers")]
internal class SeasonPlayersDTO : BaseModel
{
    [Column("seasonid")]
    public int SeasonId { get; set; }

    [Column("playerid")]
    public int PlayerId { get; set; }
}

