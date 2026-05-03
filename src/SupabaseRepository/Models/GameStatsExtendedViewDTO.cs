using Postgrest.Attributes;
using Postgrest.Models;
using IsotopesStats.Domain.Models;
using System;

namespace IsotopesStats.SupabaseRepository.Models;

[Table("v_game_stats_extended")]
internal class GameStatsExtendedViewDTO : StatEntryDTO
{
    [Column("playername")]
    public string PlayerName { get; set; } = string.Empty;

    [Column("seasonid")]
    public int SeasonId { get; set; }

    [Column("seasonname")]
    public string SeasonName { get; set; } = string.Empty;

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

    [Column("visitingteamscore")]
    public int? VisitingTeamScore { get; set; }

    [Column("hometeamscore")]
    public int? HomeTeamScore { get; set; }
}
