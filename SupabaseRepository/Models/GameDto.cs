using Postgrest.Attributes;
using Postgrest.Models;
using IsotopesStats.Models;

namespace SupabaseRepository.Models;

[Table("games")]
public class GameDto : BaseModel
{
    [PrimaryKey("id", false)]
    public int Id { get; set; }

    [Column("seasonid")]
    public int SeasonId { get; set; }

    [Column("gamenumber")]
    public int GameNumber { get; set; }

    [Column("date")]
    public DateTime Date { get; set; } = DateTime.Now;

    [Column("diamond")]
    public string Diamond { get; set; } = string.Empty;

    [Column("ishome")]
    public bool IsHome { get; set; } = true;

    [Column("opponentid")]
    public int OpponentId { get; set; }

    [Column("type")]
    public GameType Type { get; set; } = GameType.League;

    [Column("isdeleted")]
    public bool IsDeleted { get; set; } = false;

    [Reference(typeof(OpponentDto))]
    public OpponentDto? Opponent { get; set; }
}
