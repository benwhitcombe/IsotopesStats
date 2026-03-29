namespace IsotopesStats.Models;

public class GameStatDetail
{
    public int GameNumber { get; set; }
    public DateTime Date { get; set; }
    public string Diamond { get; set; } = string.Empty;
    public string Opponent { get; set; } = string.Empty;
    public string PlayerName { get; set; } = string.Empty;
    public int BO { get; set; }
    
    // Plate Appearance Stats (Calculated)
    public int H1B { get; set; }
    public int H2B { get; set; }
    public int H3B { get; set; }
    public int H4B { get; set; }
    public int HR { get; set; }
    
    public int H => H1B + H2B + H3B + H4B + HR;
    
    // Other Plate Outcomes
    public int FC { get; set; }
    public int BB { get; set; }
    public int SF { get; set; }
    
    // Outs
    public int K { get; set; }
    public int KF { get; set; }
    public int GO { get; set; }
    public int FO { get; set; }
    
    public int AB => H + FC + K + KF + GO + FO;
    public int PA => AB + BB + SF;
    
    // Run Stats
    public int R { get; set; }
    public int RBI { get; set; }
}
