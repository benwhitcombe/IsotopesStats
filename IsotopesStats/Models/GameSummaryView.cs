using Postgrest.Attributes;
using Postgrest.Models;

namespace IsotopesStats.Models;

[Table("v_game_summaries")]
public class GameSummaryView : BaseModel
{
    [PrimaryKey("gameid", false)]
    public int GameId { get; set; }

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

    [Column("playercount")]
    public int PlayerCount { get; set; }

    [Column("teamruns")]
    public int TeamRuns { get; set; }

    [Column("teamhits")]
    public int TeamHits { get; set; }

    [Column("teamhrs")]
    public int TeamHRs { get; set; }

    [Column("teambbs")]
    public int TeamBBs { get; set; }

    [Column("teamab")]
    public int TeamAB { get; set; }

    [Column("teampa")]
    public int TeamPA { get; set; }

    [Column("teamrbi")]
    public int TeamRBI { get; set; }
}
