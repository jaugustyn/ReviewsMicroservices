using System.Security.Claims;
using Core.Entities.Models;

namespace Core.Services;

public interface IJwtService
{
    string GenerateAccessToken(User user);
    RefreshToken GenerateRefreshToken(User user);
    bool Verify(string token);
}