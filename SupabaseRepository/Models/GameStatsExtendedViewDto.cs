using Postgrest.Attributes;
using Postgrest.Models;
using IsotopesStats.Models;
using System;

namespace SupabaseRepository.Models;

[Table("v_game_stats_extended")]
public class GameStatsExtendedViewDto : StatEntryDto
{
    [Column("playername")]
    public string PlayerName { get; set; } = string.Empty;

    [Column("seasonid")]
    public int SeasonId { get; set; }

    [Column("gamenumber")]
    public int GameNumber { get; set; }

    [Column("date")]
    public DateTime Date { get; set; }

    [Column("diamond")]
    public string Diamond { get; set; } = string.Empty;

    [Column("ishome")]
    public bool IsHome { get; set; }

    [Column("opponentid")]
    public int OpponentId { get; set; }

    [Column("gametype")]
    public GameType GameType { get; set; }

    [Column("gameisdeleted")]
    public bool GameIsDeleted { get; set; }

    [Column("opponentname")]
    public string OpponentName { get; set; } = string.Empty;

    [Column("opponentshortname")]
    public string OpponentShortName { get; set; } = string.Empty;
}
