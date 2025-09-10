using KitchenStock.Application.Abstractions;
using KitchenStock.Application.Modules.Auth.Abstractions;
using KitchenStock.Application.Modules.User.Abstractions;
using KitchenStock.Infrastructure.Modules.Auth.Services;
using KitchenStock.Infrastructure.Modules.User.Services;
using KitchenStock.Infrastructure.Persistence;
using KitchenStock.Infrastructure.Security.Vault;
using KitchenStock.Infrastructure.Security.Vault.Configuration;
using KitchenStock.Infrastructure.Security.Vault.Dtos;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

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

    public static IServiceCollection AddKitchenStockServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<VaultSettings>(configuration.GetSection("Vault"));
        services.AddHttpClient<IVaultService, VaultService>((sp, client) =>
        {
            var config = sp.GetRequiredService<IOptions<VaultSettings>>().Value;
            client.BaseAddress = new Uri(config.Url);
            client.DefaultRequestHeaders.Add("X-Vault-Token", config.Token);
        });

        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IAuthService, AuthService>();

        return services;
    }
}
