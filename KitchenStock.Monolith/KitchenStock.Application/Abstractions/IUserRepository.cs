using KitchenStock.Domain.Entities;

namespace KitchenStock.Application.Abstractions;

public interface IUserRepository : IGenericRepository<UserEntity>
{
    Task<UserEntity?> GetByEmailAsync(string email);
    Task<bool> ExistsAsync(string email);
}