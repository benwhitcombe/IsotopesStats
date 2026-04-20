using Postgrest.Attributes;
using Postgrest.Models;
using IsotopesStats.Models;

namespace SupabaseRepository.Models;

[Table("v_season_opponents_list")]
public class SeasonOpponentViewDto : BaseModel
{
    [Column("seasonid")]
    public int SeasonId { get; set; }

    [Column("opponentid")]
    public int OpponentId { get; set; }

    [Column("opponentname")]
    public string OpponentName { get; set; } = string.Empty;
}
