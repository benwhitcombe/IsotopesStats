namespace IsotopesStats.Domain.Models;

public enum UserLogAction
{
    Created = 0,
    Updated = 1,
    Deleted = 2
}

public record UserLog : IEntity
{
    public int Id { get; set; }
    
    public string? UserId { get; set; }
    
    public string UserEmail { get; set; } = string.Empty;
    
    public UserLogAction Action { get; set; }
    
    public string EntityType { get; set; } = string.Empty;
    
    public string EntityId { get; set; } = string.Empty;
    
    public string Description { get; set; } = string.Empty;
    
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}
