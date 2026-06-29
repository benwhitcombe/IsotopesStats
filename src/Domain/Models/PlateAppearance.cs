using System.Text.Json.Serialization;

namespace IsotopesStats.Domain.Models;

public record PlateAppearance : IEntity
{
    public int Id { get; set; }
    
    public int GameId { get; set; }
    
    public int PlayerId { get; set; }
    
    public int Inning { get; set; }
    
    public int OrderNumber { get; set; } // Position in the batting order for that inning/game
    
    public string Result { get; set; } = string.Empty; // 1B, 2B, 3B, HR, BB, K, GO, FO, FC, etc.
    
    // State BEFORE this plate appearance
    public int OutsBefore { get; set; }
    public int? RunnerOn1B { get; set; } // PlayerId of runner on 1st, or null
    public int? RunnerOn2B { get; set; } // PlayerId of runner on 2nd, or null
    public int? RunnerOn3B { get; set; } // PlayerId of runner on 3rd, or null
    
    // State AFTER this plate appearance
    public int OutsRecorded { get; set; } // Number of outs recorded on this play
    public int RunsScored { get; set; } // Number of runs scored on this play
    public int? RunnerOn1B_End { get; set; }
    public int? RunnerOn2B_End { get; set; }
    public int? RunnerOn3B_End { get; set; }
    
    // Outs that occurred on this play
    public int? OutAt1B { get; set; } // PlayerId of runner thrown out at 1B
    public int? OutAt2B { get; set; } // PlayerId of runner thrown out at 2B
    public int? OutAt3B { get; set; } // PlayerId of runner thrown out at 3B
    public int? OutAtHome { get; set; } // PlayerId of runner thrown out at Home
    
    // Navigation properties
    public Player? Player { get; set; }
    public Game? Game { get; set; }
    
    public bool EndsInning { get; set; }
}
