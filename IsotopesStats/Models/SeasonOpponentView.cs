using Postgrest.Attributes;
using Postgrest.Models;

namespace IsotopesStats.Models;

[Table("v_season_opponents_list")]
public class SeasonOpponentView : BaseModel
{
    [Column("seasonid")]
    public int SeasonId { get; set; }

    [Column("opponentid")]
    public int OpponentId { get; set; }

    [Column("opponentname")]
    public string OpponentName { get; set; } = string.Empty;
}
