using Shared.Common.Persistence.Abstractions;
using User.Domain.Entities;

namespace User.Application.Services.Abstractions;

public interface IUserRepository : IGenericRepository<UserEntity>
{
    Task<UserEntity?> GetByEmailAsync(string email);
    Task<bool> ExistsAsync(string email);
}
