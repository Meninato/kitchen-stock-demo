using FluentResults;

namespace KitchenStock.Application.Modules.Stock.Results;

public static class StockErrors
{
    public static class Validation
    {
        public static Error QuantityZero =>
            new Error("Stock quantity cannot be zero")
                .WithMetadata("ErrorCode", "STOCK_QUANTITY_ZERO")
                .WithMetadata("Field", "Quantity")
                .WithMetadata("Type", "Validation");

        public static Error InvalidQuantity(decimal quantity) =>
            new Error($"Invalid stock quantity: {quantity}")
                .WithMetadata("ErrorCode", "STOCK_INVALID_QUANTITY")
                .WithMetadata("Field", "Quantity")
                .WithMetadata("Quantity", quantity.ToString())
                .WithMetadata("Type", "Validation");

        public static Error UnitPriceRequiredForPurchase =>
            new Error("Unit price is required for purchase entries")
                .WithMetadata("ErrorCode", "STOCK_UNIT_PRICE_REQUIRED")
                .WithMetadata("Field", "UnitPrice")
                .WithMetadata("Type", "Validation");

        public static Error InvalidUnitPrice(decimal price) =>
            new Error($"Unit price must be greater than zero: {price}")
                .WithMetadata("ErrorCode", "STOCK_INVALID_UNIT_PRICE")
                .WithMetadata("Field", "UnitPrice")
                .WithMetadata("UnitPrice", price.ToString())
                .WithMetadata("Type", "Validation");

        public static Error ReasonTooLong(int maxLength) =>
            new Error($"Reason must not exceed {maxLength} characters")
                .WithMetadata("ErrorCode", "STOCK_REASON_TOO_LONG")
                .WithMetadata("Field", "Reason")
                .WithMetadata("Type", "Validation");

        public static Error ReferenceTooLong(int maxLength) =>
            new Error($"Reference must not exceed {maxLength} characters")
                .WithMetadata("ErrorCode", "STOCK_REFERENCE_TOO_LONG")
                .WithMetadata("Field", "Reference")
                .WithMetadata("Type", "Validation");

        public static Error InvalidMovementType(string movementType) =>
            new Error($"Invalid stock movement type: {movementType}")
                .WithMetadata("ErrorCode", "STOCK_INVALID_MOVEMENT_TYPE")
                .WithMetadata("Field", "MovementType")
                .WithMetadata("MovementType", movementType)
                .WithMetadata("Type", "Validation");
    }

    public static class BusinessRules
    {
        public static Error CannotReduceStockBelowZero(string ingredientName, decimal currentStock, decimal requestedReduction) =>
            new Error($"Cannot reduce stock of '{ingredientName}' by {requestedReduction}. Current stock: {currentStock}")
                .WithMetadata("ErrorCode", "STOCK_CANNOT_REDUCE_BELOW_ZERO")
                .WithMetadata("IngredientName", ingredientName)
                .WithMetadata("CurrentStock", currentStock.ToString())
                .WithMetadata("RequestedReduction", requestedReduction.ToString())
                .WithMetadata("Type", "BusinessRule");

        public static Error SupplierNotFound(Guid supplierId) =>
            new Error($"Supplier with ID {supplierId} not found")
                .WithMetadata("ErrorCode", "STOCK_SUPPLIER_NOT_FOUND")
                .WithMetadata("SupplierId", supplierId.ToString())
                .WithMetadata("Type", "BusinessRule");

        public static Error CannotDeleteUsedInCalculations(Guid stockEntryId) =>
            new Error($"Cannot delete stock entry {stockEntryId} as it's used in cost calculations")
                .WithMetadata("ErrorCode", "STOCK_ENTRY_USED_IN_CALCULATIONS")
                .WithMetadata("StockEntryId", stockEntryId.ToString())
                .WithMetadata("Type", "BusinessRule");

        public static Error MovementDateInFuture =>
            new Error("Stock movement date cannot be in the future")
                .WithMetadata("ErrorCode", "STOCK_MOVEMENT_DATE_FUTURE")
                .WithMetadata("Field", "MovementDate")
                .WithMetadata("Type", "BusinessRule");
    }

    public static class Authorization
    {
        public static Error NotFound(Guid stockEntryId) =>
            new Error($"Stock entry with ID {stockEntryId} not found")
                .WithMetadata("ErrorCode", "STOCK_ENTRY_NOT_FOUND")
                .WithMetadata("StockEntryId", stockEntryId.ToString())
                .WithMetadata("Type", "NotFound");

        public static Error IngredientAccessDenied(Guid ingredientId, Guid userId) =>
            new Error($"User does not have access to ingredient {ingredientId}")
                .WithMetadata("ErrorCode", "STOCK_INGREDIENT_ACCESS_DENIED")
                .WithMetadata("IngredientId", ingredientId.ToString())
                .WithMetadata("UserId", userId.ToString())
                .WithMetadata("Type", "Forbidden");
    }

    public static Error CalculationError(string calculation, Exception? ex = null) =>
        new Error($"Error calculating {calculation}")
            .WithMetadata("ErrorCode", "STOCK_CALCULATION_ERROR")
            .WithMetadata("Calculation", calculation)
            .WithMetadata("Type", "Internal")
            .CausedBy(ex);

    public static Error DatabaseError(string operation, Exception? ex = null) =>
        new Error($"Database error during {operation}")
            .WithMetadata("ErrorCode", "STOCK_DATABASE_ERROR")
            .WithMetadata("Operation", operation)
            .WithMetadata("Type", "Internal")
            .CausedBy(ex);

    public static Error UnexpectedError(string operation, Exception? ex = null) =>
        new Error($"Unexpected error during {operation}")
            .WithMetadata("ErrorCode", "STOCK_UNEXPECTED_ERROR")
            .WithMetadata("Operation", operation)
            .WithMetadata("Type", "Internal")
            .CausedBy(ex);
}