namespace Core.Entities.Models;

public class Rating : IEntityBase
{
    public Guid Id { get; init; }
    public Guid ReviewId { get; init; }
    public Guid UserId { get; init; }
    public int Value { get; init; }
}