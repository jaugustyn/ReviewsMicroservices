using Core.Entities.Models;

namespace Core.Interfaces.Repositories;

public interface IReviewRepository : IGenericRepository<Review>
{
    IEnumerable<Review> GetReviewsByUserIdSync(Guid userId);
}