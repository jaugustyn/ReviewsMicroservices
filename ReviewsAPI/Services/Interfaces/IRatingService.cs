using ReviewsAPI.Dto.Rating;

namespace ReviewsAPI.Services;

public interface IRatingService
{
    Task<RatingDto> GetByIdAsync(Guid ratingId);
    Task<RatingDto> CreateAsync(Guid userId, RatingCreateDto entity);
    Task UpdateAsync(Guid ratingId, RatingUpdateDto entity);
    Task DeleteAsync(Guid ratingId);
    
    Task<double?> GetRatingsByReviewIdAsync(Guid reviewId);
}