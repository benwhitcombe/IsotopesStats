namespace IsotopesStats.Models;

public class StatEntry
{
    public int Id { get; set; }
    public int PlayerId { get; set; }
    public int GameId { get; set; }
    
    // Plate Appearance Stats
    public int PA { get; set; }
    public int AB { get; set; }
    public int H { get; set; }
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
    public int B { get; set; }
    public int GO { get; set; }
    public int FO { get; set; }
    public int O { get; set; }
    
    // Run Stats
    public int R { get; set; }
    public int RBI { get; set; }
}
