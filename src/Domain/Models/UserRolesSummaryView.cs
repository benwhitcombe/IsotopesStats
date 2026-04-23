namespace IsotopesStats.Domain.Models;

public record UserRolesSummaryView : IEntity<string>
{
    public string Id { get; set; } = string.Empty; // Mapping userid to Id for IEntity compatibility

    public string Email { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }

    public string RoleNames { get; set; } = string.Empty;

    public bool IsDeleted { get; set; } = false;
}
