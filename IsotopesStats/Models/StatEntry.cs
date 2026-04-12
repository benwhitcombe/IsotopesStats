using Postgrest.Attributes;
using Postgrest.Models;

namespace IsotopesStats.Models;

[Table("stats")]
public class StatEntry : BaseModel, IEntity
{
    [PrimaryKey("id", false)]
    public int Id { get; set; }
    
    [Column("playerid")]
    public int PlayerId { get; set; }
    
    [Column("gameid")]
    public int GameId { get; set; }
    
    [Column("bo")]
    public int BO { get; set; }
    
    [Column("h1b")]
    public int H1B { get; set; }
    
    [Column("h2b")]
    public int H2B { get; set; }
    
    [Column("h3b")]
    public int H3B { get; set; }
    
    [Column("h4b")]
    public int H4B { get; set; }
    
    [Column("hr")]
    public int HR { get; set; }
    
    [Column("fc")]
    public int FC { get; set; }
    
    [Column("bb")]
    public int BB { get; set; }
    
    [Column("sf")]
    public int SF { get; set; }
    
    [Column("k")]
    public int K { get; set; }
    
    [Column("kf")]
    public int KF { get; set; }
    
    [Column("go")]
    public int GO { get; set; }
    
    [Column("fo")]
    public int FO { get; set; }
    
    [Column("r")]
    public int R { get; set; }
    
    [Column("rbi")]
    public int RBI { get; set; }

    [Reference(typeof(Player))]
    public Player? Player { get; set; }

    [Reference(typeof(Game))]
    public Game? Game { get; set; }

    // Calculated properties restored for UI
    public string PlayerName => Player?.Name ?? string.Empty;
    public int H => H1B + H2B + H3B + H4B + HR;
    public int AB => H + FC + K + KF + GO + FO;
    public int PA => AB + BB + SF;
    public int TB => H1B + (2 * H2B) + (3 * H3B) + (4 * (H4B + HR));
    
    // Calculated Stats
    public double AVG => AB > 0 ? (double)H / AB : 0;
    
    public double OBP => (AB + BB + SF) > 0 
        ? (double)(H + BB) / (AB + BB + SF) 
        : 0;

    public double SLG => AB > 0 
        ? (double)TB / AB 
        : 0;

    public double OPS => OBP + SLG;

    public StatEntry Clone() => (StatEntry)this.MemberwiseClone();
}
