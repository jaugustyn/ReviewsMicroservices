namespace Core.Entities.Models;

public record Review : IEntityBase
{
    public Guid Id { get; init; }
    public string Title { get; init; }
    public string Text { get; init; }
    public DateTimeOffset CreatedAt { get; init; }
    public Guid UserId { get; init; }
}