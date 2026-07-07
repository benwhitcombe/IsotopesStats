using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IsotopesStats.Domain.Interfaces;
using IsotopesStats.Domain.Models;
using IsotopesStats.Website.Shared.Models;

namespace IsotopesStats.Website.Services
{
    public class SandboxScorekeeperProvider : IScorekeeperDataProvider
    {
        private readonly IStatsRepository _statsRepository;
        private readonly SessionStorageService _sessionStorage;
        private readonly int _gameId = 1;

        public SandboxScorekeeperProvider(IStatsRepository statsRepository, SessionStorageService sessionStorage)
        {
            _statsRepository = statsRepository;
            _sessionStorage = sessionStorage;
        }

        public async Task<Game?> GetGameAsync()
        {
            var savedGame = await _sessionStorage.GetItemAsync<Game>("Sandbox_Game");
            if (savedGame != null) return savedGame;
            return new Game 
            { 
                Id = _gameId, 
                OpponentId = 1, 
                IsHome = true, 
                Date = DateTime.Now,
                Opponent = new Opponent { Id = 1, Name = "Opponent", ShortName = "Opp" }
            };
        }

        public async Task<List<Player>> GetRosterAsync()
        {
            // Sandbox uses the current season's roster or a mock roster
            // For simplicity, fetch players from season 1 or current season
            // Alternatively, fetch all active players
            return await _statsRepository.GetPlayersAsync(1); // Assuming 1 is a valid season for the sandbox
        }

        public async Task<List<int>> GetBattingOrderAsync()
        {
            var lineupNames = await _sessionStorage.GetItemAsync<string[]>("Sandbox_Lineup") ?? Array.Empty<string>();
            var roster = await GetRosterAsync();
            var lineupIds = new List<int>();
            
            foreach (var name in lineupNames)
            {
                var player = roster.FirstOrDefault(p => p.Name == name);
                if (player != null)
                {
                    lineupIds.Add(player.Id);
                }
            }
            return lineupIds;
        }

        public async Task SaveBattingOrderAsync(List<int> playerIds)
        {
            var roster = await GetRosterAsync();
            var names = playerIds.Select(id => roster.FirstOrDefault(p => p.Id == id)?.Name).Where(n => n != null).ToArray();
            await _sessionStorage.SetItemAsync("Sandbox_Lineup", names);
        }

        private List<SandboxRecordedPlay>? _cachedPlays = null;

        public async Task<List<PlateAppearance>> GetPlateAppearancesAsync()
        {
            if (_cachedPlays == null)
            {
                _cachedPlays = await _sessionStorage.GetItemAsync<List<SandboxRecordedPlay>>("Sandbox_RecordedPlays") ?? new List<SandboxRecordedPlay>();
            }
            return _cachedPlays.Where(p => p.PA != null).Select(p => p.PA!).ToList();
        }

        public async Task<PlateAppearance> SavePlateAppearanceAsync(PlateAppearance pa)
        {
            pa.Id = (int)DateTime.UtcNow.Ticks; // Generate dummy ID
            
            if (_cachedPlays == null)
            {
                _cachedPlays = await _sessionStorage.GetItemAsync<List<SandboxRecordedPlay>>("Sandbox_RecordedPlays") ?? new List<SandboxRecordedPlay>();
            }
            
            _cachedPlays.Add(new SandboxRecordedPlay { PA = pa });
            await _sessionStorage.SetItemAsync("Sandbox_RecordedPlays", _cachedPlays);
            return pa;
        }

        public async Task UpdatePlateAppearanceAsync(PlateAppearance pa)
        {
            if (_cachedPlays == null)
            {
                _cachedPlays = await _sessionStorage.GetItemAsync<List<SandboxRecordedPlay>>("Sandbox_RecordedPlays") ?? new List<SandboxRecordedPlay>();
            }
            
            var playToUpdate = _cachedPlays.FirstOrDefault(p => p.PA != null && p.PA.Id == pa.Id);
            if (playToUpdate != null)
            {
                playToUpdate.PA = pa;
                await _sessionStorage.SetItemAsync("Sandbox_RecordedPlays", _cachedPlays);
            }
        }

        public async Task DeletePlateAppearanceAsync(PlateAppearance pa)
        {
            if (_cachedPlays == null)
            {
                _cachedPlays = await _sessionStorage.GetItemAsync<List<SandboxRecordedPlay>>("Sandbox_RecordedPlays") ?? new List<SandboxRecordedPlay>();
            }
            
            var playToRemove = _cachedPlays.LastOrDefault(p => p.PA != null && p.PA.Id == pa.Id);
            if (playToRemove != null)
            {
                _cachedPlays.Remove(playToRemove);
                await _sessionStorage.SetItemAsync("Sandbox_RecordedPlays", _cachedPlays);
            }
        }

        public async Task UpdateGameAsync(Game game)
        {
            await _sessionStorage.SetItemAsync("Sandbox_Game", game);
        }

        public async Task ClearGameDataAsync()
        {
            _cachedPlays = null;
            await _sessionStorage.RemoveItemAsync("Sandbox_RecordedPlays");
            await _sessionStorage.RemoveItemAsync("Sandbox_Lineup");
        }

        public Task SyncGameStatsAsync()
        {
            return Task.CompletedTask;
        }

        public Task LogActionAsync(string description)
        {
            // Do not log sandbox actions
            return Task.CompletedTask;
        }
    }
}
