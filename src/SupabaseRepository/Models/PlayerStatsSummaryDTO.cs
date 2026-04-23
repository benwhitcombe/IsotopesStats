using Postgrest.Attributes;
using Postgrest.Models;
using IsotopesStats.Domain.Models;

namespace IsotopesStats.SupabaseRepository.Models;

[Table("v_player_stats_summary")]
internal class PlayerStatsSummaryDTO : BaseModel
{
    [Column("playername")]
    public string PlayerName { get; set; } = string.Empty;

    [Column("gamesplayed")]
    public int GamesPlayed { get; set; }

    [Column("h1b")]
    public int H1B { get; set; }

    [Column("h2b")]
    public int H2B { get; set; }

    [Column("h3b")]
    public int H3B { get; set; }

    [Column("h4b")]
    public int H4B { get; set; }

    [Column("hr")]
    public int HR { get; set; }

    [Column("fc")]
    public int FC { get; set; }

    [Column("bb")]
    public int BB { get; set; }

    [Column("sf")]
    public int SF { get; set; }

    [Column("k")]
    public int K { get; set; }

    [Column("kf")]
    public int KF { get; set; }

    [Column("go")]
    public int GO { get; set; }

    [Column("fo")]
    public int FO { get; set; }

    [Column("r")]
    public int R { get; set; }

    [Column("rbi")]
    public int RBI { get; set; }
}

