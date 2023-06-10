using Core.Entities.Models;
using Core.Interfaces.Repositories;
using Infrastructure.Data;
using MongoDB.Driver;

namespace Infrastructure.Repositories;

public class CommentRepository : GenericRepository<Comment>, ICommentRepository
{
    private readonly FilterDefinitionBuilder<Comment> _filterBuilder = Builders<Comment>.Filter;
    private readonly IMongoCollection<Comment> _collection;
    
    public CommentRepository(IMongoDbContext context): base(context)
    {
        _collection = context.Comments;
    }
    
    public IEnumerable<Comment> GetCommentsByReviewIdSync(Guid reviewId)
    {
        var filter = _filterBuilder.Eq(x => x.ReviewId, reviewId);
        return _collection.Find(filter).ToList();
    }
    
    public IEnumerable<Comment> GetCommentsByUserIdSync(Guid userId)
    {
        var filter = _filterBuilder.Eq(x => x.UserId, userId);
        return _collection.Find(filter).ToList();
    }

    public void DeleteCommentSync(Guid commentId)
    {
        var filter = _filterBuilder.Eq(x => x.Id, commentId);
        _collection.DeleteOne(filter);
    }
}