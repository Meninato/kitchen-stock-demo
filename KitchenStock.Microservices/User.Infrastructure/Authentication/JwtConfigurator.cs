using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Shared.Common.Vault.Abstractions;
using System.Text;
using User.Infrastructure.Vault;

namespace User.Infrastructure.Authentication;

public class JwtOptionsConfigurator : IConfigureOptions<JwtBearerOptions>
{
    private readonly IVaultService _vault;

    public JwtOptionsConfigurator(IVaultService vault)
    {
        _vault = vault;
    }

    public void Configure(JwtBearerOptions options)
    {
        var vaultJwt = _vault.GetAsync<VaultJwt>("kitchen/services/jwt").GetAwaiter().GetResult();

        if (vaultJwt is null || string.IsNullOrEmpty(vaultJwt.Secret))
            throw new InvalidOperationException("Jwt is missing in Vault");

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(vaultJwt.Secret)),
            ValidateIssuer = true,
            ValidIssuer = "myapp.com",
            ValidateAudience = true,
            ValidAudience = "myapp-clients",
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    }
}
