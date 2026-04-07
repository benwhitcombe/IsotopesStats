using System.ComponentModel.DataAnnotations;

namespace IsotopesStats.Models;

public enum GameType
{
    RegularSeason,
    Playoffs,
    Exhibition
}

public record class Game
{
    public int Id { get; set; }
    
    [Range(1, int.MaxValue, ErrorMessage = "Please select a season.")]
    public int SeasonId { get; set; }
    
    [Range(1, 1000, ErrorMessage = "Game number must be between 1 and 1000.")]
    public int GameNumber { get; set; }
    
    [Required(ErrorMessage = "Date is required.")]
    public DateTime Date { get; set; }
    
    [StringLength(100, ErrorMessage = "Diamond name cannot exceed 100 characters.")]
    public string Diamond { get; set; } = string.Empty;
    
    [Range(1, int.MaxValue, ErrorMessage = "Please select an opponent.")]
    public int OpponentId { get; set; }
    
    public Opponent? Opponent { get; set; }
    public GameType Type { get; set; } = GameType.RegularSeason;
    public bool IsDeleted { get; set; } = false;
}
