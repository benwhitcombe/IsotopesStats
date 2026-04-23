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
        ModeledResponse<SeasonDTO> response = await _supabase.From<SeasonDTO>()
            .Where(x => x.IsDeleted == false)
            .Order("name", Postgrest.Constants.Ordering.Descending)
            .Get();
        return response.Models.Select(x => x.ToModel()).ToList();
    }

    public async Task<int> AddSeasonAsync(Season season)
    {
        ModeledResponse<SeasonDTO> response = await _supabase.From<SeasonDTO>().Insert(season.ToDTO());
        return response.Model?.Id ?? 0;
    }

    public async Task UpdateSeasonAsync(Season season)
    {
        Season update = new Season { Id = season.Id, Name = season.Name, IsDeleted = season.IsDeleted };
        await _supabase.From<SeasonDTO>().Update(update.ToDTO());
    }

    public async Task DeleteSeasonAsync(int seasonId)
    {
        Season season = new Season { Id = seasonId, IsDeleted = true };
        await _supabase.From<SeasonDTO>().Update(season.ToDTO());
    }

    public async Task<bool> IsSeasonNameUniqueAsync(string name, int excludeId = 0)
    {
        ModeledResponse<SeasonDTO> response = await _supabase.From<SeasonDTO>()
            .Where(x => x.Name == name)
            .Where(x => x.Id != excludeId)
            .Where(x => x.IsDeleted == false)
            .Get();
        return response.Models.Count == 0;
    }

    // --- PLAYERS ---
    public async Task<List<Player>> GetAllPlayersAsync()
    {
        ModeledResponse<PlayerDTO> response = await _supabase.From<PlayerDTO>()
            .Where(x => x.IsDeleted == false)
            .Order("name", Postgrest.Constants.Ordering.Ascending)
            .Get();
        return response.Models.Select(x => x.ToModel()).ToList();
    }

    public async Task<List<Player>> GetPlayersAsync(int seasonId)
    {
        ModeledResponse<SeasonPlayerViewDTO> response = await _supabase.From<SeasonPlayerViewDTO>()
            .Filter("seasonid", Postgrest.Constants.Operator.Equals, seasonId)
            .Get();
        
        List<Player> players = new List<Player>();
        foreach (SeasonPlayerViewDTO sp in response.Models)
        {
            players.Add(new Player { Id = sp.PlayerId, Name = sp.PlayerName });
        }
        return players;
    }

    public async Task<int> AddPlayerAsync(Player player, int seasonId)
    {
        if (seasonId == 0)
        {
            ModeledResponse<PlayerDTO> insertResponse = await _supabase.From<PlayerDTO>().Insert(player.ToDTO());
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
        await _supabase.From<PlayerDTO>().Update(update.ToDTO());
    }

    public async Task DeletePlayerAsync(int playerId)
    {
        Player player = new Player { Id = playerId, IsDeleted = true };
        await _supabase.From<PlayerDTO>().Update(player.ToDTO());
    }

    public async Task AddPlayerToSeasonAsync(int playerId, int seasonId)
    {
        SeasonPlayers link = new SeasonPlayers { PlayerId = playerId, SeasonId = seasonId };
        await _supabase.From<SeasonPlayersDTO>().Insert(link.ToDTO());
    }

    public async Task DeletePlayerFromSeasonAsync(int playerId, int seasonId)
    {
        await _supabase.From<SeasonPlayersDTO>()
            .Filter("playerid", Postgrest.Constants.Operator.Equals, playerId)
            .Filter("seasonid", Postgrest.Constants.Operator.Equals, seasonId)
            .Delete();
    }

    public async Task<bool> IsPlayerNameUniqueAsync(string name, int excludeId = 0)
    {
        ModeledResponse<PlayerDTO> response = await _supabase.From<PlayerDTO>()
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
        ModeledResponse<OpponentDTO> response = await _supabase.From<OpponentDTO>()
            .Where(x => x.IsDeleted == false)
            .Order("name", Postgrest.Constants.Ordering.Ascending)
            .Get();
        return response.Models.Select(x => x.ToModel()).ToList();
    }

    public async Task<List<Opponent>> GetOpponentsAsync(int seasonId)
    {
        ModeledResponse<SeasonOpponentViewDTO> response = await _supabase.From<SeasonOpponentViewDTO>()
            .Filter("seasonid", Postgrest.Constants.Operator.Equals, seasonId)
            .Get();
        
        List<Opponent> opponents = new List<Opponent>();
        foreach (SeasonOpponentViewDTO sov in response.Models)
        {
            opponents.Add(new Opponent { 
                Id = sov.OpponentId, 
                Name = sov.OpponentName,
                ShortName = sov.OpponentShortName
            });
        }
        return opponents.OrderBy(o => o.Name).ToList();
    }

    public async Task<int> AddOpponentAsync(Opponent opponent, int seasonId)
    {
        if (seasonId == 0)
        {
            ModeledResponse<OpponentDTO> insertResponse = await _supabase.From<OpponentDTO>().Insert(opponent.ToDTO());
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
        Opponent update = new Opponent { Id = opponent.Id, Name = opponent.Name, ShortName = opponent.ShortName, IsDeleted = opponent.IsDeleted };
        await _supabase.From<OpponentDTO>().Update(update.ToDTO());
    }

    public async Task DeleteOpponentAsync(int opponentId)
    {
        OpponentDTO? existing = await _supabase.From<OpponentDTO>().Where(x => x.Id == opponentId).Single();
        if (existing != null)
        {
            existing.IsDeleted = true;
            await _supabase.From<OpponentDTO>().Update(existing);
        }
    }

    public async Task<bool> IsOpponentNameUniqueAsync(string name, int excludeId = 0)
    {
        ModeledResponse<OpponentDTO> response = await _supabase.From<OpponentDTO>()
            .Where(x => x.Name == name)
            .Where(x => x.Id != excludeId)
            .Where(x => x.IsDeleted == false)
            .Get();
        return response.Models.Count == 0;
    }

    public async Task<List<SeasonOpponents>> GetSeasonsForOpponentAsync(int opponentId)
    {
        ModeledResponse<SeasonOpponentsDTO> response = await _supabase.From<SeasonOpponentsDTO>()
            .Filter("opponentid", Postgrest.Constants.Operator.Equals, opponentId)
            .Get();
        return response.Models.Select(x => x.ToModel()).ToList();
    }

    public async Task AddOpponentToSeasonAsync(int opponentId, int seasonId, string? name = null, string? shortName = null)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            OpponentDTO? master = await _supabase.From<OpponentDTO>().Where(x => x.Id == opponentId).Single();
            name = master?.Name;
        }
        SeasonOpponents link = new SeasonOpponents { SeasonId = seasonId, OpponentId = opponentId, Name = name, ShortName = shortName };
        await _supabase.From<SeasonOpponentsDTO>().Insert(link.ToDTO());
    }

    public async Task DeleteOpponentFromSeasonAsync(int opponentId, int seasonId)
    {
        await _supabase.From<SeasonOpponentsDTO>()
            .Filter("opponentid", Postgrest.Constants.Operator.Equals, opponentId)
            .Filter("seasonid", Postgrest.Constants.Operator.Equals, seasonId)
            .Delete();
    }

    // --- GAMES & STATS ---
    public async Task<Game?> GetGameAsync(int gameId)
    {
        GameDTO? gameDTO = await _supabase.From<GameDTO>()
            .Filter("id", Postgrest.Constants.Operator.Equals, gameId)
            .Single();
        return gameDTO?.ToModel();
    }

    public async Task<List<Game>> GetGamesBySeasonAsync(int seasonId)
    {
        ModeledResponse<GameManagementViewDTO> response = await _supabase.From<GameManagementViewDTO>()
            .Filter("seasonid", Postgrest.Constants.Operator.Equals, seasonId)
            .Order("date", Postgrest.Constants.Ordering.Descending)
            .Get();
        
        List<Game> games = new List<Game>();
        foreach (GameManagementViewDTO v in response.Models)
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
        ModeledResponse<StatEntryDTO> response = await _supabase.From<StatEntryDTO>()
            .Select("*, Player:players(*)")
            .Where(x => x.GameId == gameId)
            .Order("bo", Postgrest.Constants.Ordering.Ascending)
            .Get();
        return response.Models.Select(x => x.ToModel()).ToList();
    }

    public async Task<List<PlayerStatsSummary>> GetStatsSummaryAsync(int seasonId)
    {
        List<Player> players = await GetPlayersAsync(seasonId);
        
        ModeledResponse<PlayerStatsSummaryDTO> response = await _supabase.From<PlayerStatsSummaryDTO>()
            .Filter("seasonid", Postgrest.Constants.Operator.Equals, seasonId)
            .Get();
            
        List<PlayerStatsSummary> stats = response.Models.Select(x => x.ToModel()).ToList();
        
        return players.Where(p => p.Name != "Spare")
            .Select(p => stats.FirstOrDefault(s => s.PlayerName == p.Name) ?? new PlayerStatsSummary { PlayerName = p.Name })
            .ToList();
    }

    public async Task<List<PlayerStatsSummary>> GetAllStatsSummaryAsync()
    {
        List<Player> players = await GetAllPlayersAsync();
        
        BaseResponse response = await _supabase.Rpc("get_player_stats_summary_all", null);
        List<PlayerStatsSummaryDTO> DTOs = string.IsNullOrEmpty(response.Content) 
            ? new List<PlayerStatsSummaryDTO>() 
            : JsonConvert.DeserializeObject<List<PlayerStatsSummaryDTO>>(response.Content) ?? new List<PlayerStatsSummaryDTO>();

        List<PlayerStatsSummary> aggregatedStats = DTOs.Select(x => x.ToModel()).ToList();
            
        return players.Where(p => p.Name != "Spare")
            .Select(p => aggregatedStats.FirstOrDefault(s => s.PlayerName == p.Name) ?? new PlayerStatsSummary { PlayerName = p.Name })
            .ToList();
    }

    public async Task<PlayerStatsSummary> GetTeamTotalsAsync(int seasonId)
    {
        TeamStatsSummaryDTO? result = await _supabase.From<TeamStatsSummaryDTO>()
            .Filter("seasonid", Postgrest.Constants.Operator.Equals, seasonId)
            .Single();
        return result?.ToModel() ?? new TeamStatsSummary { PlayerName = "TEAM TOTALS" };
    }

    public async Task<PlayerStatsSummary> GetAllTeamTotalsAsync()
    {
        BaseResponse response = await _supabase.Rpc("get_team_stats_summary_all", null);
        List<PlayerStatsSummaryDTO> DTOs = string.IsNullOrEmpty(response.Content) 
            ? new List<PlayerStatsSummaryDTO>() 
            : JsonConvert.DeserializeObject<List<PlayerStatsSummaryDTO>>(response.Content) ?? new List<PlayerStatsSummaryDTO>();
            
        return DTOs.FirstOrDefault()?.ToModel() ?? new TeamStatsSummary { PlayerName = "TEAM TOTALS" };
    }

    public async Task<int> GetMostRecentStatsSeasonIdAsync()
    {
        ModeledResponse<GameStatsExtendedViewDTO> response = await _supabase.From<GameStatsExtendedViewDTO>()
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
            { "p_stats", stats.Select(s => s.ToDTO()).ToList() }
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
            { "p_stats", stats.Select(s => s.ToDTO()).ToList() }
        };

        await _supabase.Rpc("update_game_with_stats", parameters);
    }

    public async Task DeleteGameAsync(int gameId)
    {
        Game game = new Game { Id = gameId, IsDeleted = true };
        await _supabase.From<GameDTO>().Update(game.ToDTO());
    }

    public async Task<List<GameStatsExtendedView>> GetAllGameStatsAsync(int seasonId)
    {
        ModeledResponse<GameStatsExtendedViewDTO> response = await _supabase.From<GameStatsExtendedViewDTO>()
            .Filter("seasonid", Postgrest.Constants.Operator.Equals, seasonId)
            .Where(x => x.GameIsDeleted == false)
            .Order("date", Postgrest.Constants.Ordering.Descending)
            .Order("bo", Postgrest.Constants.Ordering.Ascending)
            .Get();
        return response.Models.Select(x => x.ToModel()).ToList();
    }

    public async Task<List<GameStatsExtendedView>> GetPlayerGameLogAsync(string playerName, int seasonId)
    {
        ModeledResponse<GameStatsExtendedViewDTO> response = await _supabase.From<GameStatsExtendedViewDTO>()
            .Filter("playername", Postgrest.Constants.Operator.Equals, playerName)
            .Filter("seasonid", Postgrest.Constants.Operator.Equals, seasonId)
            .Where(x => x.GameIsDeleted == false)
            .Order("date", Postgrest.Constants.Ordering.Descending)
            .Get();
        return response.Models.Select(x => x.ToModel()).ToList();
    }

    public async Task<List<GameStatsExtendedView>> GetAllPlayerGameLogAsync(string playerName)
    {
        ModeledResponse<GameStatsExtendedViewDTO> response = await _supabase.From<GameStatsExtendedViewDTO>()
            .Filter("playername", Postgrest.Constants.Operator.Equals, playerName)
            .Where(x => x.GameIsDeleted == false)
            .Order("date", Postgrest.Constants.Ordering.Descending)
            .Get();
        return response.Models.Select(x => x.ToModel()).ToList();
    }

    public async Task<int> GetNextGameNumberAsync(int seasonId)
    {
        ModeledResponse<GameDTO> response = await _supabase.From<GameDTO>()
            .Filter("seasonid", Postgrest.Constants.Operator.Equals, seasonId)
            .Where(x => x.IsDeleted == false)
            .Order("gamenumber", Postgrest.Constants.Ordering.Descending)
            .Limit(1)
            .Get();
        
        return (response.Model?.GameNumber ?? 0) + 1;
    }

    public async Task<List<GameSummaryView>> GetGameSummariesAsync(int seasonId)
    {
        ModeledResponse<GameSummaryViewDTO> response = await _supabase.From<GameSummaryViewDTO>()
            .Filter("seasonid", Postgrest.Constants.Operator.Equals, seasonId)
            .Order("date", Postgrest.Constants.Ordering.Descending)
            .Get();
        return response.Models.Select(x => x.ToModel()).ToList();
    }

    public async Task<List<GameSummaryView>> GetAllGameSummariesAsync()
    {
        ModeledResponse<GameSummaryViewDTO> response = await _supabase.From<GameSummaryViewDTO>()
            .Order("date", Postgrest.Constants.Ordering.Descending)
            .Get();
        return response.Models.Select(x => x.ToModel()).ToList();
    }

    public async Task<List<GameStatsExtendedView>> GetExtendedGameStatsAsync(int gameId)
    {
        ModeledResponse<GameStatsExtendedViewDTO> response = await _supabase.From<GameStatsExtendedViewDTO>()
            .Filter("gameid", Postgrest.Constants.Operator.Equals, gameId)
            .Order("bo", Postgrest.Constants.Ordering.Ascending)
            .Get();
        return response.Models.Select(x => x.ToModel()).ToList();
    }
}
