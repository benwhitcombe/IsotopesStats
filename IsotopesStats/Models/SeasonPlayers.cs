using Postgrest.Attributes;
using Postgrest.Models;

namespace IsotopesStats.Models;

[Table("seasonplayers")]
public class SeasonPlayers : BaseModel
{
    [Column("seasonid")]
    public int SeasonId { get; set; }

    [Column("playerid")]
    public int PlayerId { get; set; }
}
