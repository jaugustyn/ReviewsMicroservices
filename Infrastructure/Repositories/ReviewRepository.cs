using Core.Entities.Models;
using Core.Repositories;
using Infrastructure.Data;
using MongoDB.Driver;

namespace Infrastructure.Repositories;

public class ReviewRepository : GenericRepository<Review>, IReviewRepository
{
    private readonly FilterDefinitionBuilder<Review> _filterBuilder = Builders<Review>.Filter;
    private readonly IMongoCollection<Review> _collection;
    
    public ReviewRepository(IMongoDbContext mongoContext) : base(mongoContext)
    {
        _collection = mongoContext.Reviews;
    }
    
    public IEnumerable<Review> GetReviewsByUserIdSync(Guid userId)
    {
        var filter = _filterBuilder.Eq(x => x.UserId, userId);
        return _collection.Find(filter).ToList();
    }
}