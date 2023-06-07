using Core.Entities.Models;
using Core.Repositories;
using Infrastructure.Data;

namespace Infrastructure.Repositories;

public class ReviewRepository : GenericRepository<Review>, IReviewRepository
{
    private readonly AppDbContext _context;

    public ReviewRepository(AppDbContext context) : base(context)
    {
        _context = context;
    }

    public IEnumerable<Review> GetReviewsByUserIdSync(Guid userId)
    {
        var reviews = _context.Reviews.Where(x => x.UserId.Equals(userId)).ToList();
        return reviews;
    }
}