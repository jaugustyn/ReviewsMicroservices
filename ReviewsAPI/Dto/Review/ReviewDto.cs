using Core.Entities;

namespace ReviewsAPI.Dto.Review;

public class ReviewDto : ReviewCreateDto, IEntityBase
{
    public Guid UserId { get; init; }
    public DateTimeOffset CreatedDate { get; init; }
    public Guid Id { get; init; }

    public static ReviewDto ReviewToDto(Core.Entities.Models.Review review)
    {
        return new ReviewDto
        {
            Id = review.Id,
            Title = review.Title,
            Text = review.Text,
            UserId = review.UserId,
            CreatedDate = review.CreatedDate
        };
    }
}