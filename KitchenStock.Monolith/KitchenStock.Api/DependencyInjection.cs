using KitchenStock.Application.Abstractions;
using KitchenStock.Infrastructure.Persistence;
using KitchenStock.Infrastructure.Services;
using KitchenStock.Infrastructure.Services.Vault;
using KitchenStock.Infrastructure.Services.Vault.Dtos;
using Microsoft.EntityFrameworkCore;

namespace KitchenStock.Api;

public static class DependencyInjection
{
    public static IServiceCollection AddKitchenStockDbContext(this IServiceCollection services)
    {
        services.AddDbContext<KitchenStockDbContext>((sp, options) =>
        {
            var vault = sp.GetRequiredService<IVaultService>();
            var vaultConString = vault.ReadSecretAsync<VaultDbConnectionString>("kitchen/services/user/db/dev")
                .GetAwaiter()
                .GetResult();

            if (vaultConString is null || string.IsNullOrEmpty(vaultConString.ConnectionString))
                throw new InvalidOperationException("Database connection string is missing in Vault");

            options.UseNpgsql(vaultConString.ConnectionString);
        });
        return services;
    }

    public static IServiceCollection AddKitchenStockServices(this IServiceCollection services)
    {
        services.AddSingleton<IVaultService, VaultService>();

        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IAuthService, AuthService>();

        return services;
    }
}
