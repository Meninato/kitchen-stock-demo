using KitchenStock.Application.Abstractions;
using KitchenStock.Domain.Entities;

namespace KitchenStock.Application.Modules.User.Abstractions;

public interface IUserRepository : IGenericRepository<UserEntity>
{
    Task<UserEntity?> GetByEmailAsync(string email);
    Task<bool> ExistsAsync(string email);
    Task<int> GetKitchenCountAsync(Guid userId);
}