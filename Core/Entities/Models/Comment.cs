namespace Core.Entities.Models;

public record Comment : IEntityBase
{
    public string? Text { get; init; }
    public Guid ReviewId { get; init; }
    public Guid UserId { get; init; }
    public DateTimeOffset CreatedDate { get; init; }
    public Guid Id { get; init; }
}