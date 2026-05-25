using System;
using System.ComponentModel.DataAnnotations;

namespace IsotopesStats.Domain.Models;

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

    public DateTime Date { get; set; }

    public string Diamond { get; set; } = string.Empty;

    public bool IsHome { get; set; }

    public bool IsScorekeptLive { get; set; }

    public int OpponentId { get; set; }

    public GameType GameType { get; set; }

    public bool IsDeleted { get; set; } = false;

    public int? VisitingTeamScore { get; set; }

    public int? HomeTeamScore { get; set; }

    // Calculated properties based on Isotopes perspective
    public int? OurScore => IsHome ? HomeTeamScore : VisitingTeamScore;
    public int? OpponentScore => IsHome ? VisitingTeamScore : HomeTeamScore;

    public bool? IsWin => (OurScore != null && OpponentScore != null) ? (OurScore > OpponentScore) : null;
    public bool? IsTie => (OurScore != null && OpponentScore != null) ? (OurScore == OpponentScore) : null;

    // Navigation property
    public Opponent? Opponent { get; set; }

    public string? OpponentInningScoresJson { get; set; }

    public System.Collections.Generic.Dictionary<int, int> GetOpponentInningScores()
    {
        if (string.IsNullOrWhiteSpace(OpponentInningScoresJson)) return new System.Collections.Generic.Dictionary<int, int>();
        try
        {
            return System.Text.Json.JsonSerializer.Deserialize<System.Collections.Generic.Dictionary<int, int>>(OpponentInningScoresJson) ?? new System.Collections.Generic.Dictionary<int, int>();
        }
        catch
        {
            return new System.Collections.Generic.Dictionary<int, int>();
        }
    }

    public void SetOpponentInningScores(System.Collections.Generic.Dictionary<int, int> scores)
    {
        OpponentInningScoresJson = System.Text.Json.JsonSerializer.Serialize(scores);
    }
}
