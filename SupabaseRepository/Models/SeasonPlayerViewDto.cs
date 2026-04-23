using Postgrest.Attributes;
using Postgrest.Models;
using IsotopesStats.Models;

namespace SupabaseRepository.Models;

[Table("v_season_players_list")]
internal class SeasonPlayerViewDTO : BaseModel
{
    [Column("seasonid")]
    public int SeasonId { get; set; }

    [Column("playerid")]
    public int PlayerId { get; set; }

    [Column("playername")]
    public string PlayerName { get; set; } = string.Empty;
}

