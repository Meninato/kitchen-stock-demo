using KitchenStock.Application.Modules.Auth.Dtos;
using KitchenStock.Application.Security.Vault.Abstractions;
using KitchenStock.Infrastructure.Configuration;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace KitchenStock.Infrastructure.Modules.Auth;

public class JwtOptionsConfigurator : IPostConfigureOptions<JwtBearerOptions>
{
    private readonly IVaultService _vault;
    private readonly JwtTokenVaultPathSettings _jwtTokenVaultPath;

    public JwtOptionsConfigurator(IVaultService vault, IOptions<KitchenStockSettings> config)
    {
        _vault = vault;
        _jwtTokenVaultPath = config.Value.VaultSecretPaths.JwtToken;
    }

    public async void PostConfigure(string? name, JwtBearerOptions options)
    {
        try
        {
            var vaultJwt = await _vault.ReadSecretAsync<VaultJwtTokenDto>(_jwtTokenVaultPath.Config);
                //.GetAwaiter()
                //.GetResult();

            if (vaultJwt is null || string.IsNullOrEmpty(vaultJwt.Secret))
                throw new InvalidOperationException("JWT configuration is missing in Vault");

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
        catch (Exception ex)
        {
            throw new InvalidOperationException("Failed to configure JWT options from Vault", ex);
        }
    }
}
