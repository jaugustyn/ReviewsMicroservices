namespace Core.Entities.Models;

public class RefreshToken : IEntityBase
{
    public string Token { get; set; }
    public DateTimeOffset Expires { get; set; }
    public DateTimeOffset CreatedDate { get; set; }
    public DateTimeOffset? RevokedDate { get; set; }

    public Guid UserId { get; init; }
    public User User { get; init; }
    public bool IsExpired => DateTime.UtcNow >= Expires;
    public bool IsActive => RevokedDate == null && !IsExpired;
    public Guid Id { get; init; }
}