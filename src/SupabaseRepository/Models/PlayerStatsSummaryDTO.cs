using Postgrest.Attributes;
using Postgrest.Models;
using IsotopesStats.Domain.Models;
using Newtonsoft.Json;

namespace IsotopesStats.SupabaseRepository.Models;

[Table("v_player_stats_summary")]
internal class PlayerStatsSummaryDTO : BaseModel
{
    [Column("playername")]
    [JsonProperty("playername")]
    public string PlayerName { get; set; } = string.Empty;

    [Column("gamesplayed")]
    [JsonProperty("gamesplayed")]
    public int GamesPlayed { get; set; }

    [Column("h1b")]
    [JsonProperty("h1b")]
    public int H1B { get; set; }

    [Column("h2b")]
    [JsonProperty("h2b")]
    public int H2B { get; set; }

    [Column("h3b")]
    [JsonProperty("h3b")]
    public int H3B { get; set; }

    [Column("iphr")]
    [JsonProperty("iphr")]
    public int IPHR { get; set; }

    [Column("hr")]
    [JsonProperty("hr")]
    public int HR { get; set; }

    [Column("fc")]
    [JsonProperty("fc")]
    public int FC { get; set; }

    [Column("bb")]
    [JsonProperty("bb")]
    public int BB { get; set; }

    [Column("sf")]
    [JsonProperty("sf")]
    public int SF { get; set; }

    [Column("k")]
    [JsonProperty("k")]
    public int K { get; set; }

    [Column("kf")]
    [JsonProperty("kf")]
    public int KF { get; set; }

    [Column("go")]
    [JsonProperty("go")]
    public int GO { get; set; }

    [Column("fo")]
    [JsonProperty("fo")]
    public int FO { get; set; }

    [Column("r")]
    [JsonProperty("r")]
    public int R { get; set; }

    [Column("rbi")]
    [JsonProperty("rbi")]
    public int RBI { get; set; }
}

