using System.ComponentModel.DataAnnotations;
namespace IsotopesStats.Models;

public enum GameType
{
    League = 0,
    Tournament = 1,
    Exhibition = 2
}

public record Game : IEntity
{
    public int Id { get; set; }
    
    public int SeasonId { get; set; }
    
    public int GameNumber { get; set; }
    
    public DateTime Date { get; set; } = DateTime.Now;
    
    public string Diamond { get; set; } = string.Empty;
    
    public bool IsHome { get; set; } = true;
    
    public int OpponentId { get; set; }
    
    public GameType Type { get; set; } = GameType.League;
    
    public bool IsDeleted { get; set; } = false;

    public Opponent? Opponent { get; set; }
}
