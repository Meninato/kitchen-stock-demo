using Shared.Common.Persistence.Abstractions;

namespace Kitchen.Domain.Entities;

public class KitchenEntity : BaseEntity
{
    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    // Owner ID - referência para User.Api
    public Guid OwnerId { get; set; }

    // Configurações Premium
    public bool AllowSharedIngredients { get; set; } = false;

    // Configurações operacionais
    public bool IsActive { get; set; } = true;

    // Estatísticas (calculadas)
    public int TotalIngredients { get; set; } = 0;
    public int TotalRecipes { get; set; } = 0;
    public int TotalSuppliers { get; set; } = 0;

    // Configurações de notificação
    public bool NotifyLowStock { get; set; } = true;
    public bool NotifyPriceChanges { get; set; } = true;

    // Data da última atividade
    public DateTime? LastActivityAt { get; set; }
}
