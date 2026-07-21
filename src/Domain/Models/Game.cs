using System;
using System.Linq;
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

    public bool ScorecardStatsSync { get; set; }

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

    public string? IncompleteInnings { get; set; }

    public System.Collections.Generic.List<int> GetIncompleteInningsList()
    {
        if (string.IsNullOrWhiteSpace(IncompleteInnings)) return new System.Collections.Generic.List<int>();
        try
        {
            return IncompleteInnings.Split(',', StringSplitOptions.RemoveEmptyEntries)
                                    .Select(s => int.TryParse(s.Trim(), out int val) ? val : 0)
                                    .Where(v => v > 0)
                                    .ToList();
        }
        catch
        {
            return new System.Collections.Generic.List<int>();
        }
    }

    public void ToggleIncompleteInning(int inning)
    {
        var list = GetIncompleteInningsList();
        if (list.Contains(inning))
        {
            list.Remove(inning);
        }
        else
        {
            list.Add(inning);
        }
        
        if (list.Count == 0)
        {
            IncompleteInnings = null;
        }
        else
        {
            list.Sort();
            IncompleteInnings = string.Join(",", list);
        }
    }
}
