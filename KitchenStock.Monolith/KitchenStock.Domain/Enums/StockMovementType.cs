namespace KitchenStock.Domain.Enums;

public enum StockMovementType
{
    Purchase = 1,    // Compra
    Usage = 2,       // Uso em receita
    Waste = 3,       // Desperdício/vencimento
    Adjustment = 4,  // Ajuste de inventário
    Return = 5       // Devolução
}
