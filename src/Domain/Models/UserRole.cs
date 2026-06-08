using System.Collections.Generic;

namespace IsotopesStats.Domain.Models;

public record UserRole : IEntity
{
    public int Id { get; set; }
    
    public string Name { get; set; } = string.Empty;
    
    public bool IsDeleted { get; set; } = false;

    public List<Permission> Permissions { get; set; } = new();

    public UserRole DeepClone() => this with { Permissions = new List<Permission>(Permissions) };

    public virtual bool Equals(UserRole? other)
    {
        if (other == null) return false;
        return Id == other.Id && Name == other.Name && IsDeleted == other.IsDeleted;
    }

    public override int GetHashCode() => System.HashCode.Combine(Id, Name, IsDeleted);
}
