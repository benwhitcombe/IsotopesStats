namespace IsotopesStats.Models;

public enum UserRole
{
    Administrator,
    TeamRep,
    Scorekeeper,
    Player
}

public class User
{
    public int Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public UserRole Role { get; set; } = UserRole.Player;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
