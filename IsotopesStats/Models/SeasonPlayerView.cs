using Postgrest.Attributes;
using Postgrest.Models;

namespace IsotopesStats.Models;

[Table("v_season_players_list")]
public class SeasonPlayerView : BaseModel
{
    [Column("seasonid")]
    public int SeasonId { get; set; }

    [Column("playerid")]
    public int PlayerId { get; set; }

    [Column("playername")]
    public string PlayerName { get; set; } = string.Empty;
}
