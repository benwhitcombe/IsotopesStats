namespace IsotopesStats.Models;

public record class StatEntry
{
    public int Id { get; set; }
    public int PlayerId { get; set; }
    public int GameId { get; set; }
    public int BO { get; set; }

    // Navigation Properties
    public Player? Player { get; set; }
    public Game? Game { get; set; }
    
    // Plate Appearance Stats
    public int H => H1B + H2B + H3B + H4B + HR;
    public int AB => H + FC + K + KF + GO + FO;
    public int PA => AB + BB + SF;

    public int H1B { get; set; }
    public int H2B { get; set; }
    public int H3B { get; set; }
    public int H4B { get; set; }
    public int HR { get; set; }
    
    // Other Plate Outcomes
    public int FC { get; set; }
    public int BB { get; set; }
    public int SF { get; set; }
    
    // Outs
    public int K { get; set; }
    public int KF { get; set; }
    public int GO { get; set; }
    public int FO { get; set; }
    public int O => GO + FO;
    public int TB => H1B + (2 * H2B) + (3 * H3B) + (4 * (H4B + HR));
    
    // Run Stats
    public int R { get; set; }
    public int RBI { get; set; }

    public double AVG => AB > 0 ? (double)H / AB : 0;
    public double OBP => PA > 0 ? (double)(H + BB) / PA : 0;
}
