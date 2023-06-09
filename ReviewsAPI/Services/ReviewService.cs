using Core.Entities.Models;
using Core.Interfaces.Repositories;
using ReviewsAPI.Dto.Review;

namespace ReviewsAPI.Services;

public class ReviewService : IReviewService
{
    private readonly IReviewRepository _reviewRepository;

    public ReviewService(IReviewRepository reviewRepository)
    {
        _reviewRepository = reviewRepository;
    }

    public async Task<IEnumerable<ReviewDto>> GetAllByUserIdAsync(Guid userId)
    {
        var reviews = await _reviewRepository.GetAllAsync();
        var userReviews = reviews.Where(x => x.UserId.Equals(userId)).Select(ReviewDto.ReviewToDto)
            .OrderByDescending(x => x.CreatedDate).ToList();

        return userReviews;
    }

    public async Task<ReviewDto> GetUserLastReviewAsync(Guid userId)
    {
        // Reviews are already ordered descending
        var reviews = await _reviewRepository.GetAllAsync();
        var lastReview = reviews.Where(x => x.UserId.Equals(userId)).Select(ReviewDto.ReviewToDto)
            .OrderByDescending(x => x.CreatedDate).Take(1).FirstOrDefault();

        return lastReview;
    }

    public async Task<IEnumerable<ReviewDto>> GetAllAsync()
    {
        var reviews = await _reviewRepository.GetAllAsync();
        var reviewsDto = reviews.Select(ReviewDto.ReviewToDto);

        return reviewsDto;
    }

    public async Task<ReviewDto> GetByIdAsync(Guid id)
    {
        var review = await _reviewRepository.GetByIdAsync(id);
        return review is null ? null : ReviewDto.ReviewToDto(review);
    }

    public async Task<ReviewDto> CreateAsync(Guid userId, ReviewCreateDto entity)
    {
        var review = new Review
        {
            Id = Guid.NewGuid(),
            Title = entity.Title,
            Text = entity.Text,
            UserId = userId,
            CreatedAt = DateTimeOffset.UtcNow
        };

        await _reviewRepository.CreateAsync(review);
        var newReview = await _reviewRepository.GetByIdAsync(review.Id);

        return ReviewDto.ReviewToDto(newReview);
    }

    public async Task UpdateAsync(Guid reviewId, ReviewUpdateDto entity)
    {
        var oldReview = await GetByIdAsync(reviewId);
        var review = new Review
        {
            Id = oldReview.Id,
            Title = entity.Title,
            Text = entity.Text,
            UserId = oldReview.UserId,
            CreatedAt = oldReview.CreatedDate
        };

        await _reviewRepository.UpdateAsync(reviewId, review);
    }

    public async Task DeleteAsync(Guid id)
    {
        await _reviewRepository.DeleteAsync(id);
    }
    
    public async Task<IEnumerable<ReviewDto>> SearchAsync(string keyPhrase)
    {
        var reviews = await GetAllAsync();
        IQueryable<ReviewDto>? query = null;
        
        if (!string.IsNullOrWhiteSpace(keyPhrase))
        {
            keyPhrase = keyPhrase.ToLower();
            query = reviews.AsQueryable().Where(x => x.Title.ToLower().Contains(keyPhrase)).OrderBy(x => x.CreatedDate);
        }

        return query;
    }
}