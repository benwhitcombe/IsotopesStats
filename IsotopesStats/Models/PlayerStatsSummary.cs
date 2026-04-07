namespace IsotopesStats.Models;

public record class PlayerStatsSummary
{
    public string PlayerName { get; set; } = string.Empty;
    public int GamesPlayed { get; set; }
    
    public int H => H1B + H2B + H3B + H4B + HR;
    public int AB => H + FC + K + KF + GO + FO;
    public int PA => AB + BB + SF;

    public int H1B { get; set; }
    public int H2B { get; set; }
    public int H3B { get; set; }
    public int H4B { get; set; }
    public int HR { get; set; }
    
    public int FC { get; set; }
    public int BB { get; set; }
    public int SF { get; set; }
    
    public int K { get; set; }
    public int KF { get; set; }
    public int GO { get; set; }
    public int FO { get; set; }
    public int O => GO + FO;
    public int TB => H1B + (2 * H2B) + (3 * H3B) + (4 * (H4B + HR));
    
    public int R { get; set; }
    public int RBI { get; set; }

    // Calculated Stats
    public double AVG => AB > 0 ? (double)H / AB : 0;
    
    public double OBP => (AB + BB + SF) > 0 
        ? (double)(H + BB) / (AB + BB + SF) 
        : 0;

    public double SLG => AB > 0 
        ? (double)TB / AB 
        : 0;

    public double OPS => OBP + SLG;
}
