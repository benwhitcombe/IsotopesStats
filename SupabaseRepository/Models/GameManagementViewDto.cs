using Postgrest.Attributes;
using Postgrest.Models;
using IsotopesStats.Models;

namespace SupabaseRepository.Models;

[Table("v_games_management")]
public class GameManagementViewDto : BaseModel
{
    [PrimaryKey("id", false)]
    public int Id { get; set; }

    [Column("seasonid")]
    public int SeasonId { get; set; }

    [Column("gamenumber")]
    public int GameNumber { get; set; }

    [Column("date")]
    public DateTime Date { get; set; }

    [Column("diamond")]
    public string Diamond { get; set; } = string.Empty;

    [Column("ishome")]
    public bool IsHome { get; set; }

    [Column("opponentid")]
    public int OpponentId { get; set; }

    [Column("gametype")]
    public GameType GameType { get; set; }

    [Column("isdeleted")]
    public bool IsDeleted { get; set; }

    [Column("opponentname")]
    public string OpponentName { get; set; } = string.Empty;

    [Column("opponentshortname")]
    public string OpponentShortName { get; set; } = string.Empty;
}
