using Postgrest.Attributes;
using Postgrest.Models;
using IsotopesStats.Domain.Models;

namespace IsotopesStats.SupabaseRepository.Models;

[Table("v_team_stats_summary")]
internal class TeamStatsSummaryDTO : PlayerStatsSummaryDTO
{
}

