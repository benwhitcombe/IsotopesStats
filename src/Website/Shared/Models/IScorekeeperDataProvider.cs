using System.Collections.Generic;
using System.Threading.Tasks;
using IsotopesStats.Domain.Models;

namespace IsotopesStats.Website.Shared.Models
{
    public interface IScorekeeperDataProvider
    {
        Task<Game?> GetGameAsync();
        Task<List<Player>> GetRosterAsync();
        Task<List<int>> GetBattingOrderAsync();
        Task SaveBattingOrderAsync(List<int> playerIds);
        Task<List<PlateAppearance>> GetPlateAppearancesAsync();
        Task<PlateAppearance> SavePlateAppearanceAsync(PlateAppearance pa);
        Task UpdatePlateAppearanceAsync(PlateAppearance pa);
        Task DeletePlateAppearanceAsync(PlateAppearance pa);
        Task ClearGameDataAsync();
        Task SyncGameStatsAsync();
        Task LogActionAsync(string description);
    }
}
