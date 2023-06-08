using ReviewsAPI.Dto.Review;

namespace ReviewsAPI.Services;

public interface IReviewService
{
    Task<IEnumerable<ReviewDto>> GetAllByUserIdAsync(Guid userId);
    Task<ReviewDto> GetUserLastReviewAsync(Guid userId);
    Task<IEnumerable<ReviewDto>> GetAllAsync();
    Task<ReviewDto> GetByIdAsync(Guid postId);
    Task<ReviewDto> CreateAsync(Guid userId, ReviewCreateDto entity);
    Task UpdateAsync(Guid reviewId, ReviewUpdateDto entity);
    Task DeleteAsync(Guid id);
    
    Task<IEnumerable<ReviewDto>> SearchAsync(string keyPhrase);
}