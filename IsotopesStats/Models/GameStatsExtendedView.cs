using Postgrest.Attributes;
using Postgrest.Models;

namespace IsotopesStats.Models;

[Table("v_game_stats_extended")]
public class GameStatsExtendedView : StatEntry
{
    [Column("playername")]
    public string ExtendedPlayerName { get; set; } = string.Empty;

    [Column("seasonid")]
    public int SeasonId { get; set; }

    [Column("gamenumber")]
    public int GameNumber { get; set; }

    [Column("date")]
    public DateTime Date { get; set; }

    [Column("diamond")]
    public string Diamond { get; set; } = string.Empty;

    [Column("opponentid")]
    public int OpponentId { get; set; }

    [Column("gametype")]
    public GameType GameType { get; set; }

    [Column("gameisdeleted")]
    public bool GameIsDeleted { get; set; }

    [Column("opponentname")]
    public string OpponentName { get; set; } = string.Empty;
}
