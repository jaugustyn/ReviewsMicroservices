using Core.Entities.Models;
using Core.Repositories;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class TokenRepository : ITokenRepository
{
    private readonly AppDbContext _context;

    public TokenRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<RefreshToken> GetByTokenAsync(string tokenStr)
    {
        var token = await _context.RefreshTokens.FirstOrDefaultAsync(x => x.Token.Equals(tokenStr));
        return token;
    }

    public async Task<RefreshToken> GetByUserIdAsync(Guid userId)
    {
        var token = await _context.RefreshTokens.FirstOrDefaultAsync(x => x.UserId.Equals(userId) && x.IsActive);
        return token;
    }

    public async Task<RefreshToken> AddTokenAsync(RefreshToken token)
    {
        await _context.RefreshTokens.AddAsync(token);
        await _context.SaveChangesAsync();
        var item = await GetByIdAsync(token.Id);
        return item;
    }

    public async Task UpdateTokenAsync(RefreshToken entity)
    {
        _context.RefreshTokens.Update(entity);
        await _context.SaveChangesAsync();
    }

    private async Task<RefreshToken> GetByIdAsync(Guid tokenId)
    {
        var token = await _context.RefreshTokens.FindAsync(tokenId);
        return token;
    }
}