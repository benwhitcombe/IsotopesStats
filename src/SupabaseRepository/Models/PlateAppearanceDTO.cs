using Postgrest.Attributes;
using Postgrest.Models;

namespace IsotopesStats.SupabaseRepository.Models;

[Table("plate_appearances")]
internal class PlateAppearanceDTO : BaseModel
{
    [PrimaryKey("id", false)]
    public int Id { get; set; }

    [Column("gameid")]
    public int GameId { get; set; }

    [Column("playerid")]
    public int PlayerId { get; set; }

    [Column("inning")]
    public int Inning { get; set; }

    [Column("ordernumber")]
    public int OrderNumber { get; set; }

    [Column("result")]
    public string Result { get; set; } = string.Empty;

    [Column("outsbefore")]
    public int OutsBefore { get; set; }

    [Column("runneron1b")]
    public int? RunnerOn1B { get; set; }

    [Column("runneron2b")]
    public int? RunnerOn2B { get; set; }

    [Column("runneron3b")]
    public int? RunnerOn3B { get; set; }

    [Column("outsrecorded")]
    public int OutsRecorded { get; set; }

    [Column("runsscored")]
    public int RunsScored { get; set; }

    [Column("runneron1b_end")]
    public int? RunnerOn1B_End { get; set; }

    [Column("runneron2b_end")]
    public int? RunnerOn2B_End { get; set; }

    [Column("runneron3b_end")]
    public int? RunnerOn3B_End { get; set; }
}
