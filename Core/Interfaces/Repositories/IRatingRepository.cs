using Core.Entities.Models;

namespace Core.Interfaces.Repositories;

public interface IRatingRepository : IGenericRepository<Rating>
{
    Task<IEnumerable<Rating>> GetRatingsByReviewIdAsync(Guid reviewId);
}