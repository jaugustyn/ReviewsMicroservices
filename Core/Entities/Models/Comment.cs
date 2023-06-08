namespace Core.Entities.Models;

public record Comment : IEntityBase
{
    public Guid Id { get; init; }
    public string? Text { get; init; }
    public DateTimeOffset CreatedAt { get; init; }
    public DateTimeOffset ModifiedAt { get; init; }
    public Guid ReviewId { get; init; }
    public Guid UserId { get; init; }
}