using Postgrest.Attributes;
using Postgrest.Models;
using IsotopesStats.Domain.Models;

namespace IsotopesStats.SupabaseRepository.Models;

[Table("seasonopponents")]
internal class SeasonOpponentsDTO : BaseModel
{
    [PrimaryKey("seasonid", false)]
    public int SeasonId { get; set; }

    [PrimaryKey("opponentid", false)]
    public int OpponentId { get; set; }

    [Column("name")]
    public string Name { get; set; } = string.Empty;

    [Column("short_name")]
    public string ShortName { get; set; } = string.Empty;
}
