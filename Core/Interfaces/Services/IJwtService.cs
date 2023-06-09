using Core.Entities.Models;

namespace Core.Interfaces.Services;

public interface IJwtService
{
    string GenerateAccessToken(User user);
    RefreshToken GenerateRefreshToken(User user);
    bool Verify(string token);
}