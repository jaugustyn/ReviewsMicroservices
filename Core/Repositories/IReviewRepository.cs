using Core.Entities.Models;

namespace Core.Repositories;

public interface IReviewRepository : IGenericRepository<Review>
{
    IEnumerable<Review> GetReviewsByUserIdSync(Guid userId);
}