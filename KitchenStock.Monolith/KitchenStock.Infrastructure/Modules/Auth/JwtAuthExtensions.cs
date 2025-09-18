using KitchenStock.Application.Modules.Auth.Dtos;
using KitchenStock.Application.Security.Vault.Abstractions;
using KitchenStock.Infrastructure.Configuration;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace KitchenStock.Infrastructure.Modules.Auth;

public static class JwtAuthExtensions
{
    public static async Task<IServiceCollection> AddJwtAuthenticationAsync(this IServiceCollection services)
    {
        using var sp = services.BuildServiceProvider();
        var vault = sp.GetRequiredService<IVaultService>();
        var config = sp.GetRequiredService<IOptions<KitchenStockSettings>>();

        var vaultJwt = await vault.ReadSecretAsync<VaultJwtTokenDto>(
            config.Value.VaultSecretPaths.JwtToken.Config);

        if (vaultJwt is null || string.IsNullOrEmpty(vaultJwt.Secret))
            throw new InvalidOperationException("JWT configuration is missing in Vault");

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(vaultJwt.Secret)),
                    ValidateIssuer = true,
                    ValidIssuer = vaultJwt.Issuer,
                    ValidateAudience = true,
                    ValidAudience = vaultJwt.Audience,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };
            });

        return services;
    }
}
