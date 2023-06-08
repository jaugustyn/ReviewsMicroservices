using Core.Entities.Models;

namespace Core.Repositories;

public interface ITokenRepository
{
    Task<RefreshToken> GetByIdAsync(Guid id);
    Task<RefreshToken> GetByTokenAsync(string token);
    Task<RefreshToken> GetByUserIdAsync(Guid userId);
    Task<RefreshToken> AddTokenAsync(RefreshToken token);
    Task UpdateTokenAsync(RefreshToken token);
}