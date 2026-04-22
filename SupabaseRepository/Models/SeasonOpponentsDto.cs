using Postgrest.Attributes;
using Postgrest.Models;
using IsotopesStats.Models;

namespace SupabaseRepository.Models;

[Table("seasonopponents")]
public class SeasonOpponentsDto : BaseModel
{
    [Column("seasonid")]
    public int SeasonId { get; set; }

    [Column("opponentid")]
    public int OpponentId { get; set; }

    [Column("name")]
    public string? Name { get; set; }

    [Column("short_name")]
    public string? ShortName { get; set; }
}
