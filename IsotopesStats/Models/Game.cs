using System.ComponentModel.DataAnnotations;
using Postgrest.Attributes;
using Postgrest.Models;

namespace IsotopesStats.Models;

public enum GameType
{
    League = 0,
    Tournament = 1,
    Exhibition = 2
}

[Table("games")]
public class Game : BaseModel, IEntity
{
    [PrimaryKey("id", false)]
    public int Id { get; set; }
    
    [Column("seasonid")]
    public int SeasonId { get; set; }
    
    [Column("gamenumber")]
    public int GameNumber { get; set; }
    
    [Column("date")]
    public DateTime Date { get; set; } = DateTime.Now;
    
    [Column("diamond")]
    public string Diamond { get; set; } = string.Empty;
    
    [Column("opponentid")]
    public int OpponentId { get; set; }
    
    [Column("type")]
    public GameType Type { get; set; } = GameType.League;
    
    [Column("isdeleted")]
    public bool IsDeleted { get; set; } = false;

    [Reference(typeof(Opponent), shouldFilterTopLevel: false)]
    public Opponent? Opponent { get; set; }
}
