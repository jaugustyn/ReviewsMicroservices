using Core.Entities.Models;

namespace Core.Interfaces.Repositories;

public interface ICommentRepository : IGenericRepository<Comment>
{
    IEnumerable<Comment> GetCommentsByReviewIdSync(Guid postId);
    IEnumerable<Comment> GetCommentsByUserIdSync(Guid userId);
    void DeleteCommentSync(Guid id);
}