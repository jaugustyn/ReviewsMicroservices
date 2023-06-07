using Core.Entities.Models;
using Core.Repositories;
using Infrastructure.Data;

namespace Infrastructure.Repositories;

public class CommentRepository : GenericRepository<Comment>, ICommentRepository
{
    private readonly AppDbContext _context;

    public CommentRepository(AppDbContext context) : base(context)
    {
        _context = context;
    }

    public IEnumerable<Comment> GetCommentsByReviewIdSync(Guid reviewId)
    {
        var comments = _context.Comments.Where(x => x.ReviewId.Equals(reviewId)).ToList();
        return comments;
    }

    public IEnumerable<Comment> GetCommentsByUserIdSync(Guid userId)
    {
        var comments = _context.Comments.Where(x => x.UserId.Equals(userId)).ToList();
        return comments;
    }

    public void DeleteCommentSync(Guid id)
    {
        var comment = _context.Comments.FirstOrDefault(x => x.Id.Equals(id));

        _context.Comments.Remove(comment);
        _context.SaveChanges();
    }
}