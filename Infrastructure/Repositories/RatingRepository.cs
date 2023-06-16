using Core.Entities.Models;
using Core.Interfaces.Repositories;
using Infrastructure.Data;
using MongoDB.Driver;

namespace Infrastructure.Repositories;

public class RatingRepository : GenericRepository<Rating>, IRatingRepository
{
    private readonly FilterDefinitionBuilder<Rating> _filterBuilder = Builders<Rating>.Filter;
    private readonly IMongoCollection<Rating> _collection;

    public RatingRepository(IMongoDbContext mongoContext) : base(mongoContext)
    {
        _collection = mongoContext.Ratings;
    }

    public async Task<IEnumerable<Rating>> GetRatingsByReviewIdAsync(Guid reviewId)
    {
        var filter = _filterBuilder.Eq(x => x.ReviewId, reviewId);
        return await _collection.Find(filter).ToListAsync();
    }

    public IEnumerable<Rating> GetRatingsByReviewIdSync(Guid reviewId)
    {
        var filter = _filterBuilder.Eq(x => x.ReviewId, reviewId);
        return _collection.Find(filter).ToList();
    }
    public IEnumerable<Rating> GetRatingsByUserIdSync(Guid userId)
    {
        var filter = _filterBuilder.Eq(x => x.UserId, userId);
        return _collection.Find(filter).ToList();
    }
}