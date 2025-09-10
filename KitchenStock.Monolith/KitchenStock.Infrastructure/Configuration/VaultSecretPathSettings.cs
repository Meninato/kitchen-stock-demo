namespace KitchenStock.Infrastructure.Configuration;

public class VaultSecretPathSettings
{
    public DatabaseVaultPathSettings Database { get; set; } = new();
    public JwtTokenVaultPathSettings JwtToken { get; set; } = new();
}

public class DatabaseVaultPathSettings
{
    public string ConnectionString { get; set; } = string.Empty;
}

public class JwtTokenVaultPathSettings
{
    public string Config { get; set; } = string.Empty;
}