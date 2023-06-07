namespace Core.Entities.Models;

public record Review : IEntityBase
{
    public string Title { get; init; }
    public string Text { get; init; }
    public Guid UserId { get; init; }
    public DateTimeOffset CreatedDate { get; init; }
    public Guid Id { get; init; }
}