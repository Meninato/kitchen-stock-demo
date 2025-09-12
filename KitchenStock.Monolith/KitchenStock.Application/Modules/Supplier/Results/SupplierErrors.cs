using FluentResults;

namespace KitchenStock.Application.Modules.Supplier.Results;

public static class SupplierErrors
{
    public static class Validation
    {
        public static Error NameRequired =>
            new Error("Supplier name is required")
                .WithMetadata("ErrorCode", "SUPPLIER_NAME_REQUIRED")
                .WithMetadata("Field", "Name")
                .WithMetadata("Type", "Validation");

        public static Error NameTooLong(int maxLength) =>
            new Error($"Supplier name must not exceed {maxLength} characters")
                .WithMetadata("ErrorCode", "SUPPLIER_NAME_TOO_LONG")
                .WithMetadata("Field", "Name")
                .WithMetadata("Type", "Validation");

        public static Error PhoneTooLong(int maxLength) =>
            new Error($"Phone number must not exceed {maxLength} characters")
                .WithMetadata("ErrorCode", "SUPPLIER_PHONE_TOO_LONG")
                .WithMetadata("Field", "Phone")
                .WithMetadata("Type", "Validation");

        public static Error InvalidPhoneFormat(string phone) =>
            new Error($"Phone number '{phone}' has invalid format")
                .WithMetadata("ErrorCode", "SUPPLIER_INVALID_PHONE")
                .WithMetadata("Field", "Phone")
                .WithMetadata("Phone", phone)
                .WithMetadata("Type", "Validation");

        public static Error EmailTooLong(int maxLength) =>
            new Error($"Email must not exceed {maxLength} characters")
                .WithMetadata("ErrorCode", "SUPPLIER_EMAIL_TOO_LONG")
                .WithMetadata("Field", "Email")
                .WithMetadata("Type", "Validation");

        public static Error InvalidEmailFormat(string email) =>
            new Error($"Email '{email}' has invalid format")
                .WithMetadata("ErrorCode", "SUPPLIER_INVALID_EMAIL")
                .WithMetadata("Field", "Email")
                .WithMetadata("Email", email)
                .WithMetadata("Type", "Validation");

        public static Error AddressTooLong(string addressField, int maxLength) =>
            new Error($"{addressField} must not exceed {maxLength} characters")
                .WithMetadata("ErrorCode", $"SUPPLIER_ADDRESS_{addressField.ToUpper()}_TOO_LONG")
                .WithMetadata("Field", addressField)
                .WithMetadata("Type", "Validation");

        public static Error InvalidAddressLatitude(double latitude) =>
            new Error($"Address latitude '{latitude}' is invalid")
                .WithMetadata("ErrorCode", $"SUPPLIER_INVALID_ADDRESS_LATITUDE")
                .WithMetadata("Field", "Latitude")
                .WithMetadata("Type", "Validation");

        public static Error InvalidAddressLongitude(double longitude) =>
            new Error($"Address longitude '{longitude}' is invalid")
                .WithMetadata("ErrorCode", $"SUPPLIER_INVALID_ADDRESS_LONGITUDE")
                .WithMetadata("Field", "Longitude")
                .WithMetadata("Type", "Validation");
    }

    public static class BusinessRules
    {
        public static Error AlreadyExistsInKitchen(string name, Guid kitchenId) =>
            new Error($"Supplier '{name}' already exists in this kitchen")
                .WithMetadata("ErrorCode", "SUPPLIER_ALREADY_EXISTS")
                .WithMetadata("Field", "Name")
                .WithMetadata("SupplierName", name)
                .WithMetadata("KitchenId", kitchenId.ToString())
                .WithMetadata("Type", "BusinessRule");

        public static Error CannotDeleteWithActiveStockEntries(string supplierName, int entryCount) =>
            new Error($"Cannot delete supplier '{supplierName}' with {entryCount} active stock entries")
                .WithMetadata("ErrorCode", "SUPPLIER_HAS_STOCK_ENTRIES")
                .WithMetadata("SupplierName", supplierName)
                .WithMetadata("EntryCount", entryCount.ToString())
                .WithMetadata("Type", "BusinessRule");

        public static Error EmailAlreadyUsedInKitchen(string email, Guid kitchenId) =>
            new Error($"Email '{email}' is already used by another supplier in this kitchen")
                .WithMetadata("ErrorCode", "SUPPLIER_EMAIL_EXISTS")
                .WithMetadata("Field", "Email")
                .WithMetadata("Email", email)
                .WithMetadata("KitchenId", kitchenId.ToString())
                .WithMetadata("Type", "BusinessRule");

        public static Error PhoneAlreadyUsedInKitchen(string phone, Guid kitchenId) =>
            new Error($"Phone '{phone}' is already used by another supplier in this kitchen")
                .WithMetadata("ErrorCode", "SUPPLIER_PHONE_EXISTS")
                .WithMetadata("Field", "Phone")
                .WithMetadata("Phone", phone)
                .WithMetadata("KitchenId", kitchenId.ToString())
                .WithMetadata("Type", "BusinessRule");
    }

    public static class Authorization
    {
        public static Error NotFound(Guid supplierId) =>
            new Error($"Supplier with ID {supplierId} not found")
                .WithMetadata("ErrorCode", "SUPPLIER_NOT_FOUND")
                .WithMetadata("SupplierId", supplierId.ToString())
                .WithMetadata("Type", "NotFound");

        public static Error KitchenAccessDenied(Guid kitchenId, Guid userId) =>
            new Error($"User does not have access to kitchen {kitchenId}")
                .WithMetadata("ErrorCode", "SUPPLIER_KITCHEN_ACCESS_DENIED")
                .WithMetadata("KitchenId", kitchenId.ToString())
                .WithMetadata("UserId", userId.ToString())
                .WithMetadata("Type", "Forbidden");
    }

    public static Error DatabaseError(string operation, Exception? ex = null) =>
        new Error($"Database error during {operation}")
            .WithMetadata("ErrorCode", "SUPPLIER_DATABASE_ERROR")
            .WithMetadata("Operation", operation)
            .WithMetadata("Type", "Internal")
            .CausedBy(ex);

    public static Error UnexpectedError(string operation, Exception? ex = null) =>
        new Error($"Unexpected error during {operation}")
            .WithMetadata("ErrorCode", "SUPPLIER_UNEXPECTED_ERROR")
            .WithMetadata("Operation", operation)
            .WithMetadata("Type", "Internal")
            .CausedBy(ex);
}
