using Postgrest.Attributes;
using Postgrest.Models;
using IsotopesStats.Domain.Models;

namespace IsotopesStats.SupabaseRepository.Models;

[Table("v_season_opponents_list")]
internal class SeasonOpponentViewDTO : BaseModel
{
    [Column("seasonid")]
    public int SeasonId { get; set; }

    [Column("opponentid")]
    public int OpponentId { get; set; }

    [Column("opponentname")]
    public string OpponentName { get; set; } = string.Empty;

    [Column("opponentshortname")]
    public string OpponentShortName { get; set; } = string.Empty;
}

