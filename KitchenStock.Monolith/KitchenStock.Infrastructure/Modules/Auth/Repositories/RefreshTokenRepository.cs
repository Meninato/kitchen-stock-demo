using KitchenStock.Application.Modules.Auth.Abstractions;
using KitchenStock.Domain.Entities;
using KitchenStock.Infrastructure.Persistence;
using KitchenStock.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace KitchenStock.Infrastructure.Modules.Auth.Repositories;

public class RefreshTokenRepository : EfCoreGenericRepository<RefreshTokenEntity>, IRefreshTokenRepository
{
    public RefreshTokenRepository(KitchenStockDbContext context) : base(context) { }

    public async Task<RefreshTokenEntity?> GetByTokenHashAsync(string tokenHash)
    {
        return await _dbSet
            .Include(rt => rt.User)
            .FirstOrDefaultAsync(rt => rt.TokenHash == tokenHash);
    }

    public async Task RevokeTokenAsync(string tokenHash, string reason, string? replacedByToken = null)
    {
        var token = await GetByTokenHashAsync(tokenHash);
        if (token != null && token.IsActive)
        {
            token.RevokedAt = DateTime.UtcNow;
            token.RevokedReason = reason;
            token.ReplacedByToken = replacedByToken;
            await UpdateAsync(token);
        }
    }

    public async Task RevokeAllUserTokensAsync(Guid userId, string reason)
    {
        var activeTokens = await GetActiveTokensByUserIdAsync(userId);
        foreach (var token in activeTokens)
        {
            token.RevokedAt = DateTime.UtcNow;
            token.RevokedReason = reason;
        }
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<RefreshTokenEntity>> GetActiveTokensByUserIdAsync(Guid userId)
    {
        return await _dbSet
            .Where(rt => rt.UserId == userId && rt.RevokedAt == null && rt.ExpiresAt > DateTime.UtcNow)
            .OrderByDescending(rt => rt.CreatedAt)
            .ToListAsync();
    }

    public async Task CleanupExpiredTokensAsync()
    {
        var expiredTokens = await _dbSet
            .Where(rt => rt.ExpiresAt <= DateTime.UtcNow || rt.RevokedAt != null)
            .Where(rt => rt.CreatedAt <= DateTime.UtcNow.AddDays(-30)) // Keep for 30 days for audit
            .ToListAsync();

        _dbSet.RemoveRange(expiredTokens);
        await _context.SaveChangesAsync();
    }
}
