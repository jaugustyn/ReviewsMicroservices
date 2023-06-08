namespace Core.Entities.Models;

public class RefreshToken : IEntityBase
{
    public Guid Id { get; init; }
    public string Token { get; init; }
    public DateTimeOffset Expires { get; init; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? RevokedAt { get; set; }
    public Guid UserId { get; init; }
    public bool IsExpired => DateTime.UtcNow >= Expires;
    public bool IsActive => RevokedAt == null && !IsExpired;
}