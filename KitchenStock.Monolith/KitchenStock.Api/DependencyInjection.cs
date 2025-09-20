using KitchenStock.Application.Modules.Auth.Abstractions;
using KitchenStock.Application.Modules.Ingredients.Abstractions;
using KitchenStock.Application.Modules.Kitchen.Abstractions;
using KitchenStock.Application.Modules.Recipe.Abstractions;
using KitchenStock.Application.Modules.Stock.Abstractions;
using KitchenStock.Application.Modules.Supplier.Abstractions;
using KitchenStock.Application.Modules.UnitOfMeasure.Abstractions;
using KitchenStock.Application.Modules.User.Abstractions;
using KitchenStock.Application.Security.Vault.Abstractions;
using KitchenStock.Infrastructure.Configuration;
using KitchenStock.Infrastructure.Modules.Auth.Repositories;
using KitchenStock.Infrastructure.Modules.Auth.Services;
using KitchenStock.Infrastructure.Modules.Ingredients.Repositories;
using KitchenStock.Infrastructure.Modules.Ingredients.Services;
using KitchenStock.Infrastructure.Modules.Kitchen.Repositories;
using KitchenStock.Infrastructure.Modules.Kitchen.Services;
using KitchenStock.Infrastructure.Modules.Recipe.Repositories;
using KitchenStock.Infrastructure.Modules.Recipe.Services;
using KitchenStock.Infrastructure.Modules.Stock.Repositories;
using KitchenStock.Infrastructure.Modules.Stock.Services;
using KitchenStock.Infrastructure.Modules.Supplier.Repositories;
using KitchenStock.Infrastructure.Modules.Supplier.Services;
using KitchenStock.Infrastructure.Modules.UnitOfMeasure.Repositories;
using KitchenStock.Infrastructure.Modules.UnitOfMeasure.Services;
using KitchenStock.Infrastructure.Modules.User.Repositories;
using KitchenStock.Infrastructure.Modules.User.Services;
using KitchenStock.Infrastructure.Persistence;
using KitchenStock.Infrastructure.Security.Vault;
using KitchenStock.Infrastructure.Security.Vault.Configuration;
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
            var config = sp.GetRequiredService<IOptions<KitchenStockSettings>>();
            var dbSecretPath = config.Value.VaultSecretPaths.Database.ConnectionString;

            var dbConnectionString = vault.ReadSecretValueAsync(dbSecretPath)
                .GetAwaiter()
                .GetResult();

            if (string.IsNullOrEmpty(dbConnectionString))
                throw new InvalidOperationException("Database connection string is missing in Vault");

            options.UseNpgsql(dbConnectionString);
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

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IKitchenRepository, KitchenRepository>();
        services.AddScoped<IIngredientRepository, IngredientRepository>();
        services.AddScoped<IStockEntryRepository, StockEntryRepository>();
        services.AddScoped<IUnitOfMeasureRepository, UnitOfMeasureRepository>();
        services.AddScoped<ISupplierRepository, SupplierRepository>();
        services.AddScoped<IRecipeRepository, RecipeRepository>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();

        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IKitchenService, KitchenService>();
        services.AddScoped<IIngredientService, IngredientService>();
        services.AddScoped<IStockEntryService, StockEntryService>();
        services.AddScoped<IUnitOfMeasureService, UnitOfMeasureService>();
        services.AddScoped<ISupplierService, SupplierService>();
        services.AddScoped<IRecipeService, RecipeService>();

        return services;
    }
}
