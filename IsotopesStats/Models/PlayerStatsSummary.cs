namespace IsotopesStats.Models;

public class PlayerStatsSummary
{
    public string PlayerName { get; set; } = string.Empty;
    public int GamesPlayed { get; set; }
    
    public int PA { get; set; }
    public int AB { get; set; }
    public int H { get; set; }
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
    public int B { get; set; }
    public int GO { get; set; }
    public int FO { get; set; }
    public int O { get; set; }
    
    public int R { get; set; }
    public int RBI { get; set; }

    // Calculated Stats
    public double AVG => AB > 0 ? (double)H / AB : 0;
    
    public double OBP => (AB + BB + SF) > 0 
        ? (double)(H + BB) / (AB + BB + SF) 
        : 0;

    public double SLG => AB > 0 
        ? (double)(H1B + 2 * H2B + 3 * H3B + 4 * (H4B + HR)) / AB 
        : 0;

    public double OPS => OBP + SLG;
}
