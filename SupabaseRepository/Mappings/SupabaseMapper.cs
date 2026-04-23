using Riok.Mapperly.Abstractions;
using IsotopesStats.Models;
using SupabaseRepository.Models;
using IsotopesStats.Domain.Services;
using Postgrest.Models;

namespace SupabaseRepository.Mappings;

[Mapper(PropertyNameMappingStrategy = PropertyNameMappingStrategy.CaseInsensitive)]
public partial class SupabaseMapper
{
    // --- Custom Configuration ---
    private DateTime MapToWhitbyTime(DateTime source) => DateTimeService.ToWhitbyTime(source);

    // --- Entity Mappings (To Model) ---
    [MapperRequiredMapping(RequiredMappingStrategy.None)]
    public partial Game ToModel(GameDTO dto);

    [MapperRequiredMapping(RequiredMappingStrategy.None)]
    public partial Player ToModel(PlayerDTO dto);

    [MapperRequiredMapping(RequiredMappingStrategy.None)]
    public partial Season ToModel(SeasonDTO dto);

    [MapperRequiredMapping(RequiredMappingStrategy.None)]
    public partial Opponent ToModel(OpponentDTO dto);

    [MapperRequiredMapping(RequiredMappingStrategy.None)]
    public partial StatEntry ToModel(StatEntryDTO dto);

    [MapperRequiredMapping(RequiredMappingStrategy.None)]
    public partial User ToModel(UserDTO dto);

    [MapperRequiredMapping(RequiredMappingStrategy.None)]
    public partial UserLog ToModel(UserLogDTO dto);

    [MapperRequiredMapping(RequiredMappingStrategy.None)]
    public partial UserRole ToModel(UserRoleDTO dto);

    [MapperRequiredMapping(RequiredMappingStrategy.None)]
    public partial Permission ToModel(PermissionDTO dto);

    [MapperRequiredMapping(RequiredMappingStrategy.None)]
    public partial SeasonPlayers ToModel(SeasonPlayersDTO dto);

    [MapperRequiredMapping(RequiredMappingStrategy.None)]
    public partial SeasonOpponents ToModel(SeasonOpponentsDTO dto);

    // --- View Mappings (To Model) ---
    [MapperRequiredMapping(RequiredMappingStrategy.None)]
    public partial GameManagementView ToModel(GameManagementViewDTO dto);

    [MapperRequiredMapping(RequiredMappingStrategy.None)]
    public partial GameSummaryView ToModel(GameSummaryViewDTO dto);

    [MapperRequiredMapping(RequiredMappingStrategy.None)]
    public partial GameStatsExtendedView ToModel(GameStatsExtendedViewDTO dto);

    [MapperRequiredMapping(RequiredMappingStrategy.None)]
    public partial PlayerStatsSummary ToModel(PlayerStatsSummaryDTO dto);

    [MapperRequiredMapping(RequiredMappingStrategy.None)]
    public partial TeamStatsSummary ToModel(TeamStatsSummaryDTO dto);

    [MapperRequiredMapping(RequiredMappingStrategy.None)]
    public partial UserRolesSummaryView ToModel(UserRolesSummaryViewDTO dto);

    [MapperRequiredMapping(RequiredMappingStrategy.None)]
    public partial SeasonPlayerView ToModel(SeasonPlayerViewDTO dto);

    [MapperRequiredMapping(RequiredMappingStrategy.None)]
    public partial SeasonOpponentView ToModel(SeasonOpponentViewDTO dto);

    [MapperRequiredMapping(RequiredMappingStrategy.None)]
    public partial UserUserRoles ToModel(UserUserRolesDTO dto);

    // --- Entity Mappings (To DTO) ---
    [MapperRequiredMapping(RequiredMappingStrategy.None)]
    public partial GameDTO ToDTO(Game model);

    [MapperRequiredMapping(RequiredMappingStrategy.None)]
    public partial PlayerDTO ToDTO(Player model);

    [MapperRequiredMapping(RequiredMappingStrategy.None)]
    public partial SeasonDTO ToDTO(Season model);

    [MapperRequiredMapping(RequiredMappingStrategy.None)]
    public partial OpponentDTO ToDTO(Opponent model);

    [MapperRequiredMapping(RequiredMappingStrategy.None)]
    public partial StatEntryDTO ToDTO(StatEntry model);

    [MapperRequiredMapping(RequiredMappingStrategy.None)]
    public partial UserDTO ToDTO(User model);

    [MapperRequiredMapping(RequiredMappingStrategy.None)]
    public partial UserLogDTO ToDTO(UserLog model);

    [MapperRequiredMapping(RequiredMappingStrategy.None)]
    public partial UserRoleDTO ToDTO(UserRole model);

    [MapperRequiredMapping(RequiredMappingStrategy.None)]
    public partial PermissionDTO ToDTO(Permission model);

    [MapperRequiredMapping(RequiredMappingStrategy.None)]
    public partial SeasonPlayersDTO ToDTO(SeasonPlayers model);

    [MapperRequiredMapping(RequiredMappingStrategy.None)]
    public partial SeasonOpponentsDTO ToDTO(SeasonOpponents model);

    [MapperRequiredMapping(RequiredMappingStrategy.None)]
    public partial UserUserRolesDTO ToDTO(UserUserRoles model);

    [MapperRequiredMapping(RequiredMappingStrategy.None)]
    public partial RolePermissionDTO ToDTO(RolePermission model);
}
