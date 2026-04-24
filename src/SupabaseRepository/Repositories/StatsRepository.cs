using IsotopesStats.Domain.Models;
using IsotopesStats.SupabaseRepository.Models;
using IsotopesStats.SupabaseRepository.Mappings;
using Supabase;
using Postgrest;
using Postgrest.Responses;
using Newtonsoft.Json;
using IsotopesStats.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IsotopesStats.SupabaseRepository.Repositories;

internal class StatsRepository : BaseRepository, IStatsRepository
{
    public StatsRepository(Supabase.Client supabase) : base(supabase, new SupabaseMapper())
    {
    }

    // --- SEASONS ---
    public Task<List<Season>> GetSeasonsAsync() => 
        GetListAsync<Season, SeasonDTO>(Mapper.ToModel, "name", Constants.Ordering.Descending);

    public Task<int> AddSeasonAsync(Season season) => 
        InsertAsync(season, Mapper.ToDTO);

    public Task UpdateSeasonAsync(Season season) => 
        UpdateAsync(season, Mapper.ToDTO);

    public Task DeleteSeasonAsync(int seasonId) => 
        SoftDeleteAsync<SeasonDTO>(seasonId);

    public Task<bool> IsSeasonNameUniqueAsync(string name, int excludeId = 0) => 
        IsUniqueAsync<SeasonDTO>("name", name, excludeId);

    // --- PLAYERS ---
    public Task<List<Player>> GetAllPlayersAsync() => 
        GetListAsync<Player, PlayerDTO>(Mapper.ToModel, "name");

    public async Task<List<Player>> GetPlayersAsync(int seasonId)
    {
        ModeledResponse<SeasonPlayerViewDTO> response = await Supabase.From<SeasonPlayerViewDTO>()
            .Filter("seasonid", Constants.Operator.Equals, seasonId)
            .Get();
            
        return response.Models.Select(sp => new Player { Id = sp.PlayerId, Name = sp.PlayerName }).OrderBy(p => p.Name).ToList();
    }

    public async Task<int> AddPlayerAsync(Player player, int seasonId)
    {
        int playerId = player.Id;
        if (player.Id == 0)
        {
            playerId = await InsertAsync(player, Mapper.ToDTO);
        }

        if (seasonId != 0 && playerId != 0)
        {
            SeasonPlayers link = new SeasonPlayers { SeasonId = seasonId, PlayerId = playerId };
            await Supabase.From<SeasonPlayersDTO>().Insert(Mapper.ToDTO(link));
        }

        return playerId;
    }

    public Task UpdatePlayerAsync(Player player) => 
        UpdateAsync(player, Mapper.ToDTO);

    public Task DeletePlayerAsync(int playerId) => 
        SoftDeleteAsync<PlayerDTO>(playerId);

    public async Task AddPlayerToSeasonAsync(int playerId, int seasonId)
    {
        SeasonPlayers link = new SeasonPlayers { SeasonId = seasonId, PlayerId = playerId };
        await Supabase.From<SeasonPlayersDTO>().Insert(Mapper.ToDTO(link));
    }

    public async Task DeletePlayerFromSeasonAsync(int playerId, int seasonId)
    {
        await Supabase.From<SeasonPlayersDTO>()
            .Filter("seasonid", Constants.Operator.Equals, seasonId)
            .Filter("playerid", Constants.Operator.Equals, playerId)
            .Delete();
    }

    public Task<bool> IsPlayerNameUniqueAsync(string name, int excludeId = 0) => 
        IsUniqueAsync<PlayerDTO>("name", name, excludeId);

    public Task<List<Season>> GetSeasonsForPlayerAsync(int playerId) => 
        GetSeasonsAsync();

    // --- OPPONENTS ---
    public Task<List<Opponent>> GetAllOpponentsAsync() => 
        GetListAsync<Opponent, OpponentDTO>(Mapper.ToModel, "name");

    public async Task<List<Opponent>> GetOpponentsAsync(int seasonId)
    {
        ModeledResponse<SeasonOpponentViewDTO> response = await Supabase.From<SeasonOpponentViewDTO>()
            .Filter("seasonid", Constants.Operator.Equals, seasonId)
            .Get();

        return response.Models.Select(sov => new Opponent { Id = sov.OpponentId, Name = sov.OpponentName, ShortName = sov.OpponentShortName }).OrderBy(o => o.Name).ToList();
    }

    public async Task<int> AddOpponentAsync(Opponent opponent, int seasonId)
    {
        int opponentId = opponent.Id;
        if (opponent.Id == 0)
        {
            opponentId = await InsertAsync(opponent, Mapper.ToDTO);
        }

        if (seasonId != 0 && opponentId != 0)
        {
            SeasonOpponents link = new SeasonOpponents { SeasonId = seasonId, OpponentId = opponentId, Name = opponent.Name, ShortName = opponent.ShortName };
            await Supabase.From<SeasonOpponentsDTO>().Insert(Mapper.ToDTO(link));
        }

        return opponentId;
    }

    public Task UpdateOpponentAsync(Opponent opponent) => 
        UpdateAsync(opponent, Mapper.ToDTO);

    public Task DeleteOpponentAsync(int opponentId) => 
        SoftDeleteAsync<OpponentDTO>(opponentId);

    public Task<bool> IsOpponentNameUniqueAsync(string name, int excludeId = 0) => 
        IsUniqueAsync<OpponentDTO>("name", name, excludeId);

    public async Task<List<SeasonOpponents>> GetSeasonsForOpponentAsync(int opponentId)
    {
        ModeledResponse<SeasonOpponentsDTO> response = await Supabase.From<SeasonOpponentsDTO>()
            .Filter("opponentid", Constants.Operator.Equals, opponentId)
            .Get();
        return response.Models.Select(x => Mapper.ToModel(x)).ToList();
    }

    public async Task AddOpponentToSeasonAsync(int opponentId, int seasonId, string? name = null, string? shortName = null)
    {
        if (string.IsNullOrEmpty(name))
        {
            OpponentDTO? master = await Supabase.From<OpponentDTO>().Where(x => x.Id == opponentId).Single();
            name = master?.Name ?? "Unknown";
            shortName = master?.ShortName;
        }
        SeasonOpponents link = new SeasonOpponents { SeasonId = seasonId, OpponentId = opponentId, Name = name, ShortName = shortName };
        await Supabase.From<SeasonOpponentsDTO>().Insert(Mapper.ToDTO(link));
    }

    public async Task DeleteOpponentFromSeasonAsync(int opponentId, int seasonId)
    {
        await Supabase.From<SeasonOpponentsDTO>()
            .Filter("seasonid", Constants.Operator.Equals, seasonId)
            .Filter("opponentid", Constants.Operator.Equals, opponentId)
            .Delete();
    }

    // --- GAMES & STATS ---
    public async Task<Game?> GetGameAsync(int gameId)
    {
        GameDTO? gameDTO = await Supabase.From<GameDTO>()
            .Filter("id", Constants.Operator.Equals, gameId)
            .Single();
        return gameDTO != null ? Mapper.ToModel(gameDTO) : null;
    }

    public async Task<List<Game>> GetGamesBySeasonAsync(int seasonId)
    {
        ModeledResponse<GameDTO> response = await Supabase.From<GameDTO>()
            .Filter("seasonid", Constants.Operator.Equals, seasonId)
            .Where(x => x.IsDeleted == false)
            .Order("date", Constants.Ordering.Descending)
            .Get();
        return response.Models.Select(x => Mapper.ToModel(x)).ToList();
    }

    public async Task<List<StatEntry>> GetGameStatsAsync(int gameId)
    {
        ModeledResponse<StatEntryDTO> response = await Supabase.From<StatEntryDTO>()
            .Filter("gameid", Constants.Operator.Equals, gameId)
            .Order("bo", Constants.Ordering.Ascending)
            .Get();
        return response.Models.Select(x => Mapper.ToModel(x)).ToList();
    }

    public async Task<List<PlayerStatsSummary>> GetStatsSummaryAsync(int seasonId)
    {
        List<Player> players = await GetPlayersAsync(seasonId);
        
        ModeledResponse<PlayerStatsSummaryDTO> response = await Supabase.From<PlayerStatsSummaryDTO>()
            .Filter("seasonid", Constants.Operator.Equals, seasonId)
            .Get();
            
        List<PlayerStatsSummary> stats = response.Models.Select(x => Mapper.ToModel(x)).ToList();
        
        return players.Where(p => p.Name != "Spare")
            .Select(p => stats.FirstOrDefault(s => s.PlayerName == p.Name) ?? new PlayerStatsSummary { PlayerName = p.Name })
            .ToList();
    }

    public async Task<List<PlayerStatsSummary>> GetAllStatsSummaryAsync()
    {
        List<Player> players = await GetAllPlayersAsync();
        
        BaseResponse response = await Supabase.Rpc("get_player_stats_summary_all", null);
        List<PlayerStatsSummaryDTO> dtos = string.IsNullOrEmpty(response.Content) 
            ? new List<PlayerStatsSummaryDTO>() 
            : JsonConvert.DeserializeObject<List<PlayerStatsSummaryDTO>>(response.Content) ?? new List<PlayerStatsSummaryDTO>();

        List<PlayerStatsSummary> aggregatedStats = dtos.Select(x => Mapper.ToModel(x)).ToList();
            
        return players.Where(p => p.Name != "Spare")
            .Select(p => aggregatedStats.FirstOrDefault(s => s.PlayerName == p.Name) ?? new PlayerStatsSummary { PlayerName = p.Name })
            .ToList();
    }

    public async Task<PlayerStatsSummary> GetTeamTotalsAsync(int seasonId)
    {
        TeamStatsSummaryDTO? result = await Supabase.From<TeamStatsSummaryDTO>()
            .Filter("seasonid", Constants.Operator.Equals, seasonId)
            .Single();
        return result != null ? Mapper.ToModel(result) : new TeamStatsSummary { PlayerName = "TEAM TOTALS" };
    }

    public async Task<PlayerStatsSummary> GetAllTeamTotalsAsync()
    {
        BaseResponse response = await Supabase.Rpc("get_team_stats_summary_all", null);
        List<PlayerStatsSummaryDTO> dtos = string.IsNullOrEmpty(response.Content) 
            ? new List<PlayerStatsSummaryDTO>() 
            : JsonConvert.DeserializeObject<List<PlayerStatsSummaryDTO>>(response.Content) ?? new List<PlayerStatsSummaryDTO>();
            
        return dtos.Any() ? Mapper.ToModel(dtos.First()) : new TeamStatsSummary { PlayerName = "TEAM TOTALS" };
    }

    public async Task<List<GameStatsExtendedView>> GetAllGameStatsAsync(int seasonId)
    {
        ModeledResponse<GameStatsExtendedViewDTO> response = await Supabase.From<GameStatsExtendedViewDTO>()
            .Filter("seasonid", Constants.Operator.Equals, seasonId)
            .Where(x => x.GameIsDeleted == false)
            .Order("date", Constants.Ordering.Descending)
            .Get();
        return response.Models.Select(x => Mapper.ToModel(x)).ToList();
    }

    public async Task<int> GetMostRecentStatsSeasonIdAsync()
    {
        ModeledResponse<GameSummaryViewDTO> response = await Supabase.From<GameSummaryViewDTO>()
            .Where(x => x.PlayerCount > 0)
            .Order("date", Constants.Ordering.Descending)
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
            { "p_type", (int)game.GameType },
            { "p_stats", stats.Select(s => Mapper.ToDTO(s)).ToList() }
        };

        await Supabase.Rpc("create_game_with_stats", parameters);
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
            { "p_type", (int)game.GameType },
            { "p_stats", stats.Select(s => Mapper.ToDTO(s)).ToList() }
        };

        await Supabase.Rpc("update_game_with_stats", parameters);
        await Supabase.From<GameDTO>().Update(Mapper.ToDTO(game));
    }

    public async Task DeleteGameAsync(int gameId)
    {
        Game game = new Game { Id = gameId, IsDeleted = true };
        await Supabase.From<GameDTO>().Update(Mapper.ToDTO(game));
    }

    public async Task<int> GetNextGameNumberAsync(int seasonId)
    {
        ModeledResponse<GameDTO> response = await Supabase.From<GameDTO>()
            .Filter("seasonid", Constants.Operator.Equals, seasonId)
            .Order("gamenumber", Constants.Ordering.Descending)
            .Limit(1)
            .Get();
        return (response.Model?.GameNumber ?? 0) + 1;
    }

    public async Task<List<GameStatsExtendedView>> GetPlayerGameLogAsync(string playerName, int seasonId)
    {
        ModeledResponse<GameStatsExtendedViewDTO> response = await Supabase.From<GameStatsExtendedViewDTO>()
            .Filter("playername", Constants.Operator.Equals, playerName)
            .Filter("seasonid", Constants.Operator.Equals, seasonId)
            .Where(x => x.GameIsDeleted == false)
            .Order("date", Constants.Ordering.Descending)
            .Get();
        return response.Models.Select(x => Mapper.ToModel(x)).ToList();
    }

    public async Task<List<GameStatsExtendedView>> GetAllPlayerGameLogAsync(string playerName)
    {
        ModeledResponse<GameStatsExtendedViewDTO> response = await Supabase.From<GameStatsExtendedViewDTO>()
            .Filter("playername", Constants.Operator.Equals, playerName)
            .Where(x => x.GameIsDeleted == false)
            .Order("date", Constants.Ordering.Descending)
            .Get();
        return response.Models.Select(x => Mapper.ToModel(x)).ToList();
    }

    public async Task<List<GameSummaryView>> GetGameSummariesAsync(int seasonId)
    {
        ModeledResponse<GameSummaryViewDTO> response = await Supabase.From<GameSummaryViewDTO>()
            .Filter("seasonid", Constants.Operator.Equals, seasonId)
            .Order("date", Constants.Ordering.Descending)
            .Get();
        return response.Models.Select(x => Mapper.ToModel(x)).ToList();
    }

    public async Task<List<GameSummaryView>> GetAllGameSummariesAsync()
    {
        ModeledResponse<GameSummaryViewDTO> response = await Supabase.From<GameSummaryViewDTO>()
            .Order("date", Constants.Ordering.Descending)
            .Get();
        return response.Models.Select(x => Mapper.ToModel(x)).ToList();
    }

    public async Task<List<GameStatsExtendedView>> GetExtendedGameStatsAsync(int gameId)
    {
        ModeledResponse<GameStatsExtendedViewDTO> response = await Supabase.From<GameStatsExtendedViewDTO>()
            .Filter("gameid", Constants.Operator.Equals, gameId)
            .Order("bo", Constants.Ordering.Ascending)
            .Get();
        return response.Models.Select(x => Mapper.ToModel(x)).ToList();
    }
}
