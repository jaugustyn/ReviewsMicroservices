using System.ComponentModel.DataAnnotations;

namespace ReviewsAPI.Dto.Rating;

public class RatingDto
{
    public Guid Id { get; init; }
    public Guid ReviewId { get; init; }
    public Guid UserId { get; init; }
    public int Value { get; init; }

    public static RatingDto RatingToDto(Core.Entities.Models.Rating rating)
    {
        return new RatingDto
        {
            Id = rating.Id,
            ReviewId = rating.ReviewId,
            UserId = rating.UserId,
            Value = rating.Value
        };
    }
}