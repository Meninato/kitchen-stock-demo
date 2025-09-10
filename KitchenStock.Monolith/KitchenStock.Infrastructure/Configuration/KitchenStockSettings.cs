namespace KitchenStock.Infrastructure.Configuration;

public class KitchenStockSettings
{
    public const string KITCHENSTOCK_SECTION = "KitchenStock";

    public VaultSecretPathSettings VaultSecretPaths { get; set; } = new();
}