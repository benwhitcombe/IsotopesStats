using Postgrest.Attributes;
using Postgrest.Models;
using IsotopesStats.Domain.Models;
using Newtonsoft.Json;

namespace IsotopesStats.SupabaseRepository.Models;

[Table("stats")]
internal class StatEntryDTO : BaseModel
{
    [PrimaryKey("id", false)]
    [JsonProperty("id")]
    public int Id { get; set; }

    [Column("playerid")]
    [JsonProperty("playerid")]
    public int PlayerId { get; set; }

    [Column("gameid")]
    [JsonProperty("gameid")]
    public int GameId { get; set; }

    [Column("bo")]
    [JsonProperty("bo")]
    public int BO { get; set; }

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

    [Reference(typeof(PlayerDTO))]
    public PlayerDTO? Player { get; set; }

    [Reference(typeof(GameDTO))]
    public GameDTO? Game { get; set; }
}

