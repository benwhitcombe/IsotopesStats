using IsotopesStats.Models;
using SupabaseRepository.Models;
using SupabaseRepository.Mappings;
using Supabase;
using Postgrest;
using Postgrest.Responses;
using Newtonsoft.Json;
using IsotopesStats.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SupabaseRepository.Repositories;

public class SupabaseStatsRepository : IStatsRepository
{
    private readonly Supabase.Client _supabase;

    public SupabaseStatsRepository(Supabase.Client supabase)
    {
        _supabase = supabase;
    }

    // --- SEASONS ---
    public async Task<List<Season>> GetSeasonsAsync()
    {
        ModeledResponse<SeasonDto> response = await _supabase.From<SeasonDto>()
            .Where(x => x.IsDeleted == false)
            .Order("name", Postgrest.Constants.Ordering.Descending)
            .Get();
        return response.Models.Select(x => x.ToModel()).ToList();
    }

    public async Task<int> AddSeasonAsync(Season season)
    {
        ModeledResponse<SeasonDto> response = await _supabase.From<SeasonDto>().Insert(season.ToDto());
        return response.Model?.Id ?? 0;
    }

    public async Task UpdateSeasonAsync(Season season)
    {
        Season update = new Season { Id = season.Id, Name = season.Name, IsDeleted = season.IsDeleted };
        await _supabase.From<SeasonDto>().Update(update.ToDto());
    }

    public async Task DeleteSeasonAsync(int seasonId)
    {
        Season season = new Season { Id = seasonId, IsDeleted = true };
        await _supabase.From<SeasonDto>().Update(season.ToDto());
    }

    public async Task<bool> IsSeasonNameUniqueAsync(string name, int excludeId = 0)
    {
        ModeledResponse<SeasonDto> response = await _supabase.From<SeasonDto>()
            .Where(x => x.Name == name)
            .Where(x => x.Id != excludeId)
            .Where(x => x.IsDeleted == false)
            .Get();
        return response.Models.Count == 0;
    }

    // --- PLAYERS ---
    public async Task<List<Player>> GetAllPlayersAsync()
    {
        ModeledResponse<PlayerDto> response = await _supabase.From<PlayerDto>()
            .Where(x => x.IsDeleted == false)
            .Order("name", Postgrest.Constants.Ordering.Ascending)
            .Get();
        return response.Models.Select(x => x.ToModel()).ToList();
    }

    public async Task<List<Player>> GetPlayersAsync(int seasonId)
    {
        ModeledResponse<SeasonPlayerViewDto> response = await _supabase.From<SeasonPlayerViewDto>()
            .Filter("seasonid", Postgrest.Constants.Operator.Equals, seasonId)
            .Get();
        
        List<Player> players = new List<Player>();
        foreach (SeasonPlayerViewDto sp in response.Models)
        {
            players.Add(new Player { Id = sp.PlayerId, Name = sp.PlayerName });
        }
        return players;
    }

    public async Task<int> AddPlayerAsync(Player player, int seasonId)
    {
        if (seasonId == 0)
        {
            ModeledResponse<PlayerDto> insertResponse = await _supabase.From<PlayerDto>().Insert(player.ToDto());
            return insertResponse.Model?.Id ?? 0;
        }

        Dictionary<string, object> parameters = new Dictionary<string, object>
        {
            { "p_name", player.Name },
            { "p_season_id", seasonId }
        };

        BaseResponse rpcResponse = await _supabase.Rpc("add_player_to_season", parameters);
        return string.IsNullOrEmpty(rpcResponse.Content) ? 0 : JsonConvert.DeserializeObject<int>(rpcResponse.Content);
    }

    public async Task UpdatePlayerAsync(Player player)
    {
        Player update = new Player { Id = player.Id, Name = player.Name, IsDeleted = player.IsDeleted };
        await _supabase.From<PlayerDto>().Update(update.ToDto());
    }

    public async Task DeletePlayerAsync(int playerId)
    {
        Player player = new Player { Id = playerId, IsDeleted = true };
        await _supabase.From<PlayerDto>().Update(player.ToDto());
    }

    public async Task AddPlayerToSeasonAsync(int playerId, int seasonId)
    {
        SeasonPlayers link = new SeasonPlayers { PlayerId = playerId, SeasonId = seasonId };
        await _supabase.From<SeasonPlayersDto>().Insert(link.ToDto());
    }

    public async Task DeletePlayerFromSeasonAsync(int playerId, int seasonId)
    {
        await _supabase.From<SeasonPlayersDto>()
            .Filter("playerid", Postgrest.Constants.Operator.Equals, playerId)
            .Filter("seasonid", Postgrest.Constants.Operator.Equals, seasonId)
            .Delete();
    }

    public async Task<bool> IsPlayerNameUniqueAsync(string name, int excludeId = 0)
    {
        ModeledResponse<PlayerDto> response = await _supabase.From<PlayerDto>()
            .Where(x => x.Name == name)
            .Where(x => x.Id != excludeId)
            .Where(x => x.IsDeleted == false)
            .Get();
        return response.Models.Count == 0;
    }

    public async Task<List<Season>> GetSeasonsForPlayerAsync(int playerId)
    {
        return await GetSeasonsAsync();
    }

    // --- OPPONENTS ---
    public async Task<List<Opponent>> GetAllOpponentsAsync()
    {
        ModeledResponse<OpponentDto> response = await _supabase.From<OpponentDto>()
            .Where(x => x.IsDeleted == false)
            .Order("name", Postgrest.Constants.Ordering.Ascending)
            .Get();
        return response.Models.Select(x => x.ToModel()).ToList();
    }

    public async Task<List<Opponent>> GetOpponentsAsync(int seasonId)
    {
        ModeledResponse<SeasonOpponentViewDto> response = await _supabase.From<SeasonOpponentViewDto>()
            .Filter("seasonid", Postgrest.Constants.Operator.Equals, seasonId)
            .Get();
        
        List<Opponent> opponents = new List<Opponent>();
        foreach (SeasonOpponentViewDto sov in response.Models)
        {
            opponents.Add(new Opponent { 
                Id = sov.OpponentId, 
                Name = sov.OpponentName 
            });
        }
        return opponents.OrderBy(o => o.Name).ToList();
    }

    public async Task<int> AddOpponentAsync(Opponent opponent, int seasonId)
    {
        if (seasonId == 0)
        {
            ModeledResponse<OpponentDto> insertResponse = await _supabase.From<OpponentDto>().Insert(opponent.ToDto());
            return insertResponse.Model?.Id ?? 0;
        }

        Dictionary<string, object> parameters = new Dictionary<string, object>
        {
            { "p_name", opponent.Name },
            { "p_season_id", seasonId }
        };

        BaseResponse rpcResponse = await _supabase.Rpc("add_opponent_to_season", parameters);
        return string.IsNullOrEmpty(rpcResponse.Content) ? 0 : JsonConvert.DeserializeObject<int>(rpcResponse.Content);
    }

    public async Task UpdateOpponentAsync(Opponent opponent)
    {
        Opponent update = new Opponent { Id = opponent.Id, Name = opponent.Name, IsDeleted = opponent.IsDeleted };
        await _supabase.From<OpponentDto>().Update(update.ToDto());
    }

    public async Task DeleteOpponentAsync(int opponentId)
    {
        Opponent opponent = new Opponent { Id = opponentId, IsDeleted = true };
        await _supabase.From<OpponentDto>().Update(opponent.ToDto());
    }

    public async Task<bool> IsOpponentNameUniqueAsync(string name, int excludeId = 0)
    {
        ModeledResponse<OpponentDto> response = await _supabase.From<OpponentDto>()
            .Where(x => x.Name == name)
            .Where(x => x.Id != excludeId)
            .Where(x => x.IsDeleted == false)
            .Get();
        return response.Models.Count == 0;
    }

    public async Task<List<SeasonOpponents>> GetSeasonsForOpponentAsync(int opponentId)
    {
        ModeledResponse<SeasonOpponentsDto> response = await _supabase.From<SeasonOpponentsDto>()
            .Filter("opponentid", Postgrest.Constants.Operator.Equals, opponentId)
            .Get();
        return response.Models.Select(x => x.ToModel()).ToList();
    }

    public async Task AddOpponentToSeasonAsync(int opponentId, int seasonId, string? name = null, string? shortName = null)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            OpponentDto? master = await _supabase.From<OpponentDto>().Where(x => x.Id == opponentId).Single();
            name = master?.Name;
        }
        SeasonOpponents link = new SeasonOpponents { SeasonId = seasonId, OpponentId = opponentId, Name = name, ShortName = shortName };
        await _supabase.From<SeasonOpponentsDto>().Insert(link.ToDto());
    }

    public async Task DeleteOpponentFromSeasonAsync(int opponentId, int seasonId)
    {
        await _supabase.From<SeasonOpponentsDto>()
            .Filter("opponentid", Postgrest.Constants.Operator.Equals, opponentId)
            .Filter("seasonid", Postgrest.Constants.Operator.Equals, seasonId)
            .Delete();
    }

    // --- GAMES & STATS ---
    public async Task<Game?> GetGameAsync(int gameId)
    {
        GameDto? gameDto = await _supabase.From<GameDto>()
            .Filter("id", Postgrest.Constants.Operator.Equals, gameId)
            .Single();
        return gameDto?.ToModel();
    }

    public async Task<List<Game>> GetGamesBySeasonAsync(int seasonId)
    {
        ModeledResponse<GameManagementViewDto> response = await _supabase.From<GameManagementViewDto>()
            .Filter("seasonid", Postgrest.Constants.Operator.Equals, seasonId)
            .Order("date", Postgrest.Constants.Ordering.Descending)
            .Get();
        
        List<Game> games = new List<Game>();
        foreach (GameManagementViewDto v in response.Models)
        {
            games.Add(new Game {
                Id = v.Id,
                SeasonId = v.SeasonId,
                GameNumber = v.GameNumber,
                Date = v.Date,
                Diamond = v.Diamond,
                IsHome = v.IsHome,
                OpponentId = v.OpponentId,
                Type = v.GameType,
                IsDeleted = v.IsDeleted,
                Opponent = new Opponent { Id = v.OpponentId, Name = v.OpponentName }
            });
        }
        return games;
    }

    public async Task<List<StatEntry>> GetGameStatsAsync(int gameId)
    {
        ModeledResponse<StatEntryDto> response = await _supabase.From<StatEntryDto>()
            .Select("*, Player:players(*)")
            .Where(x => x.GameId == gameId)
            .Order("bo", Postgrest.Constants.Ordering.Ascending)
            .Get();
        return response.Models.Select(x => x.ToModel()).ToList();
    }

    public async Task<List<PlayerStatsSummary>> GetStatsSummaryAsync(int seasonId)
    {
        if (seasonId == -1) return await GetAllStatsSummaryAsync();
        
        List<Player> players = await GetPlayersAsync(seasonId);
        
        ModeledResponse<PlayerStatsSummaryDto> response = await _supabase.From<PlayerStatsSummaryDto>()
            .Filter("seasonid", Postgrest.Constants.Operator.Equals, seasonId)
            .Get();
            
        List<PlayerStatsSummary> stats = response.Models.Select(x => x.ToModel()).ToList();
        
        return players.Where(p => p.Name != "Spare")
            .Select(p => stats.FirstOrDefault(s => s.PlayerName == p.Name) ?? new PlayerStatsSummary { PlayerName = p.Name })
            .ToList();
    }

    private async Task<List<PlayerStatsSummary>> GetAllStatsSummaryAsync()
    {
        List<Player> players = await GetAllPlayersAsync();
        
        ModeledResponse<PlayerStatsSummaryDto> response = await _supabase.From<PlayerStatsSummaryDto>().Get();
        
        List<PlayerStatsSummary> aggregatedStats = response.Models.GroupBy(p => p.PlayerName)
            .Select(g => new PlayerStatsSummary
            {
                PlayerName = g.Key,
                GamesPlayed = g.Sum(x => x.GamesPlayed),
                H1B = g.Sum(x => x.H1B),
                H2B = g.Sum(x => x.H2B),
                H3B = g.Sum(x => x.H3B),
                H4B = g.Sum(x => x.H4B),
                HR = g.Sum(x => x.HR),
                FC = g.Sum(x => x.FC),
                BB = g.Sum(x => x.BB),
                SF = g.Sum(x => x.SF),
                K = g.Sum(x => x.K),
                KF = g.Sum(x => x.KF),
                GO = g.Sum(x => x.GO),
                FO = g.Sum(x => x.FO),
                R = g.Sum(x => x.R),
                RBI = g.Sum(x => x.RBI)
            }).ToList();
            
        return players.Where(p => p.Name != "Spare")
            .Select(p => aggregatedStats.FirstOrDefault(s => s.PlayerName == p.Name) ?? new PlayerStatsSummary { PlayerName = p.Name })
            .ToList();
    }

    public async Task<PlayerStatsSummary> GetTeamTotalsAsync(int seasonId)
    {
        if (seasonId == -1) return await GetAllTeamTotalsAsync();

        TeamStatsSummaryDto? result = await _supabase.From<TeamStatsSummaryDto>()
            .Filter("seasonid", Postgrest.Constants.Operator.Equals, seasonId)
            .Single();
        return result?.ToModel() ?? new TeamStatsSummary { PlayerName = "TEAM TOTALS" };
    }

    private async Task<PlayerStatsSummary> GetAllTeamTotalsAsync()
    {
        ModeledResponse<TeamStatsSummaryDto> response = await _supabase.From<TeamStatsSummaryDto>().Get();
        
        return new PlayerStatsSummary
        {
            PlayerName = "TEAM TOTALS",
            GamesPlayed = response.Models.Sum(x => x.GamesPlayed),
            H1B = response.Models.Sum(x => x.H1B),
            H2B = response.Models.Sum(x => x.H2B),
            H3B = response.Models.Sum(x => x.H3B),
            H4B = response.Models.Sum(x => x.H4B),
            HR = response.Models.Sum(x => x.HR),
            FC = response.Models.Sum(x => x.FC),
            BB = response.Models.Sum(x => x.BB),
            SF = response.Models.Sum(x => x.SF),
            K = response.Models.Sum(x => x.K),
            KF = response.Models.Sum(x => x.KF),
            GO = response.Models.Sum(x => x.GO),
            FO = response.Models.Sum(x => x.FO),
            R = response.Models.Sum(x => x.R),
            RBI = response.Models.Sum(x => x.RBI)
        };
    }

    public async Task<int> GetMostRecentStatsSeasonIdAsync()
    {
        ModeledResponse<GameStatsExtendedViewDto> response = await _supabase.From<GameStatsExtendedViewDto>()
            .Order("gameid", Postgrest.Constants.Ordering.Descending)
            .Limit(1)
            .Get();
        
        return response.Model?.SeasonId ?? 0;
    }

    public async Task AddGameWithStatsAsync(Game game, List<StatEntry> stats)
    {
        Dictionary<string, object> parameters = new Dictionary<string, object>
        {
            { "p_season_id", game.SeasonId },
            { "p_game_number", game.GameNumber },
            { "p_date", game.Date },
            { "p_diamond", game.Diamond ?? (object)DBNull.Value },
            { "p_ishome", game.IsHome },
            { "p_opponent_id", game.OpponentId },
            { "p_type", (int)game.Type },
            { "p_stats", stats.Select(s => s.ToDto()).ToList() }
        };

        await _supabase.Rpc("create_game_with_stats", parameters);
    }

    public async Task UpdateGameWithStatsAsync(Game game, List<StatEntry> stats)
    {
        Dictionary<string, object> parameters = new Dictionary<string, object>
        {
            { "p_game_id", game.Id },
            { "p_season_id", game.SeasonId },
            { "p_game_number", game.GameNumber },
            { "p_date", game.Date },
            { "p_diamond", game.Diamond ?? (object)DBNull.Value },
            { "p_ishome", game.IsHome },
            { "p_opponent_id", game.OpponentId },
            { "p_type", (int)game.Type },
            { "p_is_deleted", game.IsDeleted },
            { "p_stats", stats.Select(s => s.ToDto()).ToList() }
        };

        await _supabase.Rpc("update_game_with_stats", parameters);
    }

    public async Task DeleteGameAsync(int gameId)
    {
        Game game = new Game { Id = gameId, IsDeleted = true };
        await _supabase.From<GameDto>().Update(game.ToDto());
    }

    public async Task<List<GameStatsExtendedView>> GetAllGameStatsAsync(int seasonId)
    {
        ModeledResponse<GameStatsExtendedViewDto> response = await _supabase.From<GameStatsExtendedViewDto>()
            .Filter("seasonid", Postgrest.Constants.Operator.Equals, seasonId)
            .Where(x => x.GameIsDeleted == false)
            .Order("date", Postgrest.Constants.Ordering.Descending)
            .Order("bo", Postgrest.Constants.Ordering.Ascending)
            .Get();
        return response.Models.Select(x => x.ToModel()).ToList();
    }

    public async Task<List<GameStatsExtendedView>> GetPlayerGameLogAsync(string playerName, int seasonId)
    {
        ModeledResponse<GameStatsExtendedViewDto> response = await _supabase.From<GameStatsExtendedViewDto>()
            .Filter("playername", Postgrest.Constants.Operator.Equals, playerName)
            .Filter("seasonid", Postgrest.Constants.Operator.Equals, seasonId)
            .Where(x => x.GameIsDeleted == false)
            .Order("date", Postgrest.Constants.Ordering.Descending)
            .Get();
        return response.Models.Select(x => x.ToModel()).ToList();
    }

    public async Task<int> GetNextGameNumberAsync(int seasonId)
    {
        ModeledResponse<GameDto> response = await _supabase.From<GameDto>()
            .Filter("seasonid", Postgrest.Constants.Operator.Equals, seasonId)
            .Where(x => x.IsDeleted == false)
            .Order("gamenumber", Postgrest.Constants.Ordering.Descending)
            .Limit(1)
            .Get();
        
        return (response.Model?.GameNumber ?? 0) + 1;
    }

    public async Task<List<GameSummaryView>> GetGameSummariesAsync(int seasonId)
    {
        ModeledResponse<GameSummaryViewDto> response = await _supabase.From<GameSummaryViewDto>()
            .Filter("seasonid", Postgrest.Constants.Operator.Equals, seasonId)
            .Order("date", Postgrest.Constants.Ordering.Descending)
            .Get();
        return response.Models.Select(x => x.ToModel()).ToList();
    }

    public async Task<List<GameStatsExtendedView>> GetExtendedGameStatsAsync(int gameId)
    {
        ModeledResponse<GameStatsExtendedViewDto> response = await _supabase.From<GameStatsExtendedViewDto>()
            .Filter("gameid", Postgrest.Constants.Operator.Equals, gameId)
            .Order("bo", Postgrest.Constants.Ordering.Ascending)
            .Get();
        return response.Models.Select(x => x.ToModel()).ToList();
    }
}
