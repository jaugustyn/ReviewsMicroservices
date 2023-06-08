using CommentsAPI.Dto.Comment;
using Core.Entities.Models;
using Core.Repositories;

namespace CommentsAPI.Services;

public class CommentService : ICommentService
{
    private readonly ICommentRepository _commentRepository;

    public CommentService(ICommentRepository commentRepository)
    {
        _commentRepository = commentRepository;
    }

    public async Task<IEnumerable<CommentDto>> GetAllByUserIdAsync(Guid userId)
    {
        var comments = await _commentRepository.GetAllAsync();
        var userComments = comments.Where(x => x.UserId.Equals(userId)).Select(CommentDto.CommentToDto)
            .OrderByDescending(x => x.CreatedDate);

        return userComments;
    }

    public async Task<IEnumerable<CommentDto>> GetAllByReviewIdAsync(Guid reviewId)
    {
        var comments = await _commentRepository.GetAllAsync();
        var userComments = comments.Where(x => x.ReviewId.Equals(reviewId)).Select(CommentDto.CommentToDto)
            .OrderBy(x => x.CreatedDate);

        return userComments;
    }

    public async Task<IEnumerable<CommentDto>> GetAllAsync()
    {
        var comments = await _commentRepository.GetAllAsync();
        return comments is null ? null : comments.Select(CommentDto.CommentToDto);
    }

    public async Task<CommentDto> GetByIdAsync(Guid id)
    {
        var comment = await _commentRepository.GetByIdAsync(id);
        return comment is null ? null : CommentDto.CommentToDto(comment);
    }

    public async Task<CommentDto> CreateAsync(Guid userId, CommentCreateDto entity)
    {
        var comment = new Comment
        {
            Id = Guid.NewGuid(),
            Text = entity.Text,
            UserId = userId,
            ReviewId = entity.ReviewId,
            CreatedAt = DateTimeOffset.UtcNow
        };

        await _commentRepository.CreateAsync(comment);
        var newComment = await _commentRepository.GetByIdAsync(comment.Id);

        return CommentDto.CommentToDto(newComment);
    }

    public async Task UpdateAsync(Guid id, CommentUpdateDto entity)
    {
        var oldComment = await _commentRepository.GetByIdAsync(id);
        var comment = new Comment
        {
            Id = oldComment.Id,
            Text = entity.Text,
            UserId = oldComment.UserId,
            CreatedAt = oldComment.CreatedAt,
            ReviewId = oldComment.ReviewId
        };

        await _commentRepository.UpdateAsync(id, comment);
    }

    public async Task DeleteAsync(Guid id)
    {
        await _commentRepository.DeleteAsync(id);
    }
}