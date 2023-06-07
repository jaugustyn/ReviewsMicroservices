using CommentsAPI.Dto.Comment;

namespace CommentsAPI.Services;

public interface ICommentService
{
    Task<IEnumerable<CommentDto>> GetAllByUserIdAsync(Guid userId);
    Task<IEnumerable<CommentDto>> GetAllByReviewIdAsync(Guid reviewId);
    Task<IEnumerable<CommentDto>> GetAllAsync();
    Task<CommentDto> GetByIdAsync(Guid reviewId);
    Task<CommentDto> CreateAsync(Guid userId, CommentCreateDto entity);
    Task UpdateAsync(Guid postId, CommentUpdateDto entity);
    Task DeleteAsync(Guid postId);
}