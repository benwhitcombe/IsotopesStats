namespace IsotopesStats.Models;

public record class Permission
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
}
