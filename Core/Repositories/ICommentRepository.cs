using Core.Entities.Models;

namespace Core.Repositories;

public interface ICommentRepository : IGenericRepository<Comment>
{
    IEnumerable<Comment> GetCommentsByReviewIdSync(Guid postId);
    void DeleteCommentSync(Guid id);
    IEnumerable<Comment> GetCommentsByUserIdSync(Guid userId);
}