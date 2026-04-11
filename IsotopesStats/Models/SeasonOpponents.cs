using Postgrest.Attributes;
using Postgrest.Models;

namespace IsotopesStats.Models;

[Table("seasonopponents")]
public class SeasonOpponents : BaseModel
{
    [Column("seasonid")]
    public int SeasonId { get; set; }

    [Column("opponentid")]
    public int OpponentId { get; set; }
}
