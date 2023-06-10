using Core.Entities.Models;
using Core.Interfaces.Repositories;
using ReviewsAPI.Dto.Rating;
using ReviewsAPI.Dto.Review;

namespace ReviewsAPI.Services;

public class RatingService: IRatingService
{
    private readonly IRatingRepository _ratingRepository;

    public RatingService(IRatingRepository ratingRepository)
    {
        _ratingRepository = ratingRepository;
    }

    public async Task<RatingDto> GetByIdAsync(Guid ratingId)
    {
        var rating = await _ratingRepository.GetByIdAsync(ratingId);
        return rating is null ? null : RatingDto.RatingToDto(rating);
    }

    public async Task<RatingDto> CreateAsync(Guid userId, RatingCreateDto entity)
    {
        var actualRatings = await _ratingRepository.GetRatingsByReviewIdAsync(entity.ReviewId);

        if (actualRatings.Any(x => x.UserId.Equals(userId)))
        {
            return null;
        }
        
        var rating = new Rating
        {
            Id = Guid.NewGuid(),
            ReviewId = entity.ReviewId,
            UserId = userId,
            Value = entity.Value
        };

        await _ratingRepository.CreateAsync(rating);
        var newRating = await _ratingRepository.GetByIdAsync(rating.Id);

        return RatingDto.RatingToDto(newRating);
    }

    public async Task UpdateAsync(Guid ratingId, RatingUpdateDto entity)
    {
        var oldReview = await GetByIdAsync(ratingId);
        var rating = new Rating()
        {
            Id = oldReview.Id,
            ReviewId = oldReview.ReviewId,
            UserId = oldReview.UserId,
            Value = entity.Value
        };

        await _ratingRepository.UpdateAsync(ratingId, rating);
    }

    public async Task DeleteAsync(Guid ratingId)
    {
        await _ratingRepository.DeleteAsync(ratingId);
    }

    public async Task<double?> GetRatingsByReviewIdAsync(Guid reviewId)
    {
        var ratings = await _ratingRepository.GetRatingsByReviewIdAsync(reviewId);

        if (ratings is null || !ratings.Any()) return null;
        
        var avgRating = Math.Round(ratings.Average(x => x.Value), 2);
        
        return avgRating;
    }
}