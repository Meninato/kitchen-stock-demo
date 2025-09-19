using KitchenStock.Application.Abstractions;
using KitchenStock.Domain.Entities;

namespace KitchenStock.Application.Modules.Auth.Abstractions;

public interface IRefreshTokenRepository : IGenericRepository<RefreshTokenEntity>
{
    Task<RefreshTokenEntity?> GetByTokenHashAsync(string tokenHash);
    Task RevokeTokenAsync(string tokenHash, string reason, string? replacedByToken = null);
    Task RevokeAllUserTokensAsync(Guid userId, string reason);
    Task<IEnumerable<RefreshTokenEntity>> GetActiveTokensByUserIdAsync(Guid userId);
    Task CleanupExpiredTokensAsync();
}