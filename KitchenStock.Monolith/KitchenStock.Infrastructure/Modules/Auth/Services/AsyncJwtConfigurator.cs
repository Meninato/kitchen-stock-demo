using KitchenStock.Application.Modules.Auth.Dtos;
using KitchenStock.Application.Security.Vault.Abstractions;
using KitchenStock.Infrastructure.Configuration;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace KitchenStock.Infrastructure.Modules.Auth.Services;

public interface IAsyncJwtConfigurator
{
    Task ConfigureAsync(JwtBearerOptions options);
}

public class AsyncJwtConfigurator : IAsyncJwtConfigurator
{
    private readonly IVaultService _vault;
    private readonly JwtTokenVaultPathSettings _jwtTokenVaultPath;

    public AsyncJwtConfigurator(IVaultService vault, IOptions<KitchenStockSettings> config)
    {
        _vault = vault;
        _jwtTokenVaultPath = config.Value.VaultSecretPaths.JwtToken;
    }

    public async Task ConfigureAsync(JwtBearerOptions options)
    {
        var vaultJwt = await _vault.ReadSecretAsync<VaultJwtTokenDto>(_jwtTokenVaultPath.Config);

        if (vaultJwt is null || string.IsNullOrEmpty(vaultJwt.Secret))
            throw new InvalidOperationException("Jwt is missing in Vault");

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
    }
}
