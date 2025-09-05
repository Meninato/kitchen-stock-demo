using Flurl.Http.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Shared.Common.Vault.Abstractions;
using Shared.Common.Vault.Implementations;
using Shared.Common.Vault.Settings;

namespace Shared.Common.Vault.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddVault(this IServiceCollection services, VaultSettings vaultSettings)
    {
        services.AddSingleton(vaultSettings);

        services.TryAddSingleton<IFlurlClientCache, FlurlClientCache>();
        services.AddSingleton<IVaultService, VaultService>();
        return services;
    }
}
