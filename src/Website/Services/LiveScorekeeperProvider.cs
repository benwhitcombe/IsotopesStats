using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IsotopesStats.Domain.Interfaces;
using IsotopesStats.Domain.Models;
using IsotopesStats.Website.Shared.Models;
using Microsoft.AspNetCore.Components.Authorization;

namespace IsotopesStats.Website.Services
{
    public class LiveScorekeeperProvider : IScorekeeperDataProvider
    {
        private readonly IStatsRepository _statsRepository;
        private readonly IAuthRepository _authSvc;
        private readonly AuthenticationStateProvider _authStateProvider;
        private readonly int _gameId;
        
        private Game? _game;

        public LiveScorekeeperProvider(
            IStatsRepository statsRepository, 
            IAuthRepository authSvc,
            AuthenticationStateProvider authStateProvider,
            int gameId)
        {
            _statsRepository = statsRepository;
            _authSvc = authSvc;
            _authStateProvider = authStateProvider;
            _gameId = gameId;
        }

        public async Task<Game?> GetGameAsync()
        {
            _game = await _statsRepository.GetGameAsync(_gameId);
            return _game;
        }

        public async Task<List<Player>> GetRosterAsync()
        {
            if (_game == null) return new List<Player>();
            return await _statsRepository.GetPlayersAsync(_game.SeasonId);
        }

        public async Task<List<int>> GetBattingOrderAsync()
        {
            List<StatEntry> gameStats = await _statsRepository.GetGameStatsAsync(_gameId);
            return gameStats.OrderBy(s => s.BO).Select(s => s.PlayerId).Distinct().ToList();
        }

        public async Task SaveBattingOrderAsync(List<int> playerIds)
        {
            if (_game == null) return;
            
            var newStats = new List<StatEntry>();
            for (int i = 0; i < playerIds.Count; i++)
            {
                newStats.Add(new StatEntry
                {
                    GameId = _gameId,
                    PlayerId = playerIds[i],
                    BO = i + 1
                });
            }
            
            await _statsRepository.UpdateGameWithStatsAsync(_game, newStats);
            await LogActionAsync($"Set batting order with {playerIds.Count} players");
        }

        public async Task<List<PlateAppearance>> GetPlateAppearancesAsync()
        {
            return await _statsRepository.GetPlateAppearancesAsync(_gameId);
        }

        public async Task<PlateAppearance> SavePlateAppearanceAsync(PlateAppearance pa)
        {
            var saved = await _statsRepository.AddPlateAppearanceAsync(pa);
            await LogActionAsync($"Recorded {pa.Result} for Player {pa.PlayerId}");
            return saved;
        }

        public async Task UpdatePlateAppearanceAsync(PlateAppearance pa)
        {
            await _statsRepository.UpdatePlateAppearanceAsync(pa);
        }

        public async Task DeletePlateAppearanceAsync(PlateAppearance pa)
        {
            await _statsRepository.DeletePlateAppearanceAsync(pa.Id);
            await LogActionAsync($"Deleted play for Player {pa.PlayerId}");
        }

        public Task ClearGameDataAsync()
        {
            // Not implemented for live - we do not allow bulk delete from UI
            return Task.CompletedTask;
        }

        public async Task SyncGameStatsAsync()
        {
            await _statsRepository.SyncGameStatsFromPlateAppearancesAsync(_gameId);
        }

        public async Task LogActionAsync(string description)
        {
            var authState = await _authStateProvider.GetAuthenticationStateAsync();
            if (authState.User.Identity?.IsAuthenticated == true)
            {
                var userEmail = authState.User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value ?? "unknown";
                await _authSvc.AddLogAsync(new UserLog
                {
                    UserEmail = userEmail,
                    Action = UserLogAction.Updated,
                    EntityType = "Game",
                    EntityId = _gameId.ToString(),
                    Description = description,
                    Timestamp = DateTime.UtcNow
                });
            }
        }
    }
}
