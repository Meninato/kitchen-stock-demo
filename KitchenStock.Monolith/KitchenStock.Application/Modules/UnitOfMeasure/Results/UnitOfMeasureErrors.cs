using FluentResults;

namespace KitchenStock.Application.Modules.UnitOfMeasure.Results;
public static class UnitOfMeasureErrors
{
    public static class Validation
    {
        public static Error NameRequired =>
            new Error("Unit of measure name is required")
                .WithMetadata("ErrorCode", "UNIT_NAME_REQUIRED")
                .WithMetadata("Field", "Name")
                .WithMetadata("Type", "Validation");

        public static Error NameTooLong(int maxLength) =>
            new Error($"Unit name must not exceed {maxLength} characters")
                .WithMetadata("ErrorCode", "UNIT_NAME_TOO_LONG")
                .WithMetadata("Field", "Name")
                .WithMetadata("Type", "Validation");

        public static Error SymbolRequired =>
            new Error("Unit symbol is required")
                .WithMetadata("ErrorCode", "UNIT_SYMBOL_REQUIRED")
                .WithMetadata("Field", "Symbol")
                .WithMetadata("Type", "Validation");

        public static Error SymbolTooLong(int maxLength) =>
            new Error($"Unit symbol must not exceed {maxLength} characters")
                .WithMetadata("ErrorCode", "UNIT_SYMBOL_TOO_LONG")
                .WithMetadata("Field", "Symbol")
                .WithMetadata("Type", "Validation");

        public static Error InvalidSymbolFormat(string symbol) =>
            new Error($"Unit symbol '{symbol}' contains invalid characters. Only letters and numbers allowed.")
                .WithMetadata("ErrorCode", "UNIT_INVALID_SYMBOL_FORMAT")
                .WithMetadata("Field", "Symbol")
                .WithMetadata("Symbol", symbol)
                .WithMetadata("Type", "Validation");
    }

    public static class BusinessRules
    {
        public static Error SymbolAlreadyExists(string symbol) =>
            new Error($"Unit symbol '{symbol}' already exists")
                .WithMetadata("ErrorCode", "UNIT_SYMBOL_EXISTS")
                .WithMetadata("Field", "Symbol")
                .WithMetadata("Symbol", symbol)
                .WithMetadata("Type", "BusinessRule");

        public static Error NameAlreadyExists(string name) =>
            new Error($"Unit name '{name}' already exists")
                .WithMetadata("ErrorCode", "UNIT_NAME_EXISTS")
                .WithMetadata("Field", "Name")
                .WithMetadata("UnitName", name)
                .WithMetadata("Type", "BusinessRule");

        public static Error CannotDeleteUsedInIngredients(string unitName, int ingredientCount) =>
            new Error($"Cannot delete unit '{unitName}' used by {ingredientCount} ingredients")
                .WithMetadata("ErrorCode", "UNIT_USED_IN_INGREDIENTS")
                .WithMetadata("UnitName", unitName)
                .WithMetadata("IngredientCount", ingredientCount.ToString())
                .WithMetadata("Type", "BusinessRule");

        public static Error SystemUnitCannotBeDeleted(string unitName) =>
            new Error($"Cannot delete system unit '{unitName}'")
                .WithMetadata("ErrorCode", "UNIT_SYSTEM_UNIT_CANNOT_DELETE")
                .WithMetadata("UnitName", unitName)
                .WithMetadata("Type", "BusinessRule");
    }

    public static Error NotFound(Guid unitId) =>
        new Error($"Unit of measure with ID {unitId} not found")
            .WithMetadata("ErrorCode", "UNIT_NOT_FOUND")
            .WithMetadata("UnitId", unitId.ToString())
            .WithMetadata("Type", "NotFound");

    public static Error DatabaseError(string operation, Exception? ex = null) =>
        new Error($"Database error during {operation}")
            .WithMetadata("ErrorCode", "UNIT_DATABASE_ERROR")
            .WithMetadata("Operation", operation)
            .WithMetadata("Type", "Internal")
            .CausedBy(ex);

    public static Error UnexpectedError(string operation, Exception? ex = null) =>
        new Error($"Unexpected error during {operation}")
            .WithMetadata("ErrorCode", "UNIT_UNEXPECTED_ERROR")
            .WithMetadata("Operation", operation)
            .WithMetadata("Type", "Internal")
            .CausedBy(ex);
}
