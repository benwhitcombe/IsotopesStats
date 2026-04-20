using Postgrest.Attributes;
using Postgrest.Models;
using IsotopesStats.Models;

namespace SupabaseRepository.Models;

[Table("v_team_stats_summary")]
public class TeamStatsSummaryDto : PlayerStatsSummaryDto
{
}
