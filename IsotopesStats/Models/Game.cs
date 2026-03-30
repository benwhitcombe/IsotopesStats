namespace IsotopesStats.Models;

public enum GameType
{
    RegularSeason,
    Playoffs,
    Exhibition
}

public class Game
{
    public int Id { get; set; }
    public int SeasonId { get; set; }
    public int GameNumber { get; set; }
    public DateTime Date { get; set; }
    public string Diamond { get; set; } = string.Empty;
    public string Opponent { get; set; } = string.Empty;
    public GameType Type { get; set; } = GameType.RegularSeason;
}
