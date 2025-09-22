using KitchenStock.Domain.Enums;
using KitchenStock.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using KitchenStock.Domain.ValueObjects;

namespace KitchenStock.Infrastructure.Persistence.Seeding;

public class DatabaseSeeder
{
    public static async Task SeedAsync(KitchenStockDbContext context, ILogger<DatabaseSeeder>? logger = null)
    {
        // Ensure database is created
        await context.Database.EnsureCreatedAsync();

        // Check if already seeded
        if (await context.Users.AnyAsync())
        {
            logger?.LogDebug("Database already seeded. Skipping...");
            return;
        }

        using var transaction = await context.Database.BeginTransactionAsync();

        try
        {
            // ==========================================
            // 1. SEED UNITS OF MEASURE
            // ==========================================
            var units = new List<UnitOfMeasureEntity>
            {
                new() { Name = "Quilograma", Symbol = "KG" },
                new() { Name = "Grama", Symbol = "G" },
                new() { Name = "Litro", Symbol = "L" },
                new() { Name = "Mililitro", Symbol = "ML" },
                new() { Name = "Unidade", Symbol = "UN" },
                new() { Name = "Dozen", Symbol = "DZ" },
                new() { Name = "Pack", Symbol = "PCT" },
                new() { Name = "Box", Symbol = "CX" },
                new() { Name = "Can", Symbol = "LT" },
                new() { Name = "Bottle", Symbol = "GAR" }
            };

            await context.UnitsOfMeasure.AddRangeAsync(units);
            await context.SaveChangesAsync();

            // ==========================================
            // 2. SEED USERS
            // ==========================================
            var users = new List<UserEntity>
            {
                new()
                {
                    Name = "Fernando Mendes",
                    Email = "fernando@example.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Test123!"),
                    Plan = UserPlan.Premium,
                    CreatedAt = DateTime.UtcNow.AddMonths(-6)
                },
                new()
                {
                    Name = "Maria Silva",
                    Email = "maria@example.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Test123!"),
                    Plan = UserPlan.Basic,
                    CreatedAt = DateTime.UtcNow.AddMonths(-3)
                },
                new()
                {
                    Name = "Demo User",
                    Email = "demo@kitchenstock.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Demo123!"),
                    Plan = UserPlan.Premium,
                    CreatedAt = DateTime.UtcNow.AddMonths(-1)
                }
            };

            await context.Users.AddRangeAsync(users);
            await context.SaveChangesAsync();

            var fernando = users[0];
            var maria = users[1];
            var demo = users[2];

            // ==========================================
            // 3. SEED KITCHENS
            // ==========================================
            var kitchens = new List<KitchenEntity>
            {
                // Fernando's kitchens (Premium - multiple)
                new()
                {
                    Name = "Sorvetes FM",
                    Description = "Produção artesanal de sorvetes e gelatos",
                    OwnerId = fernando.Id,
                    CreatedAt = DateTime.UtcNow.AddMonths(-5)
                },
                new()
                {
                    Name = "Padaria FM",
                    Description = "Pães artesanais e confeitaria",
                    OwnerId = fernando.Id,
                    CreatedAt = DateTime.UtcNow.AddMonths(-4)
                },
                // Maria's kitchen (Basic - single)
                new()
                {
                    Name = "Cozinha da Maria",
                    Description = "Marmitas fitness e comida caseira",
                    OwnerId = maria.Id,
                    CreatedAt = DateTime.UtcNow.AddMonths(-2)
                },
                // Demo kitchen
                new()
                {
                    Name = "Demo Restaurant",
                    Description = "Full-featured demo kitchen for testing",
                    OwnerId = demo.Id,
                    CreatedAt = DateTime.UtcNow.AddDays(-30)
                }
            };

            await context.Kitchens.AddRangeAsync(kitchens);
            await context.SaveChangesAsync();

            var sorveteria = kitchens[0];
            var padaria = kitchens[1];
            var mariaKitchen = kitchens[2];
            var demoKitchen = kitchens[3];

            // ==========================================
            // 4. SEED SUPPLIERS
            // ==========================================
            var suppliers = new List<SupplierEntity>
            {
                // Sorveteria suppliers
                new()
                {
                    Name = "Laticínios Vale Verde",
                    SupplierContact = new Contact("contato@valeverde.com", "(11) 98765-4321"),
                    SupplierAddress = new Address("Rua das Vacas Felizes, 123", "Matão", "São Paulo", "15990000", "Brasil"),
                    KitchenId = sorveteria.Id
                },
                new()
                {
                    Name = "Frutas Tropicais LTDA",
                    SupplierContact = new Contact("vendas@frutastropicais.com", "(11) 91234-5678"),
                    SupplierAddress = new Address("CEAGESP Box 42", "Matão", "São Paulo", "15990000", "Brasil"),
                    KitchenId = sorveteria.Id
                },
                // Padaria suppliers
                new()
                {
                    Name = "Moinho São Paulo",
                    SupplierContact = new Contact("vendas@moinhosp.com", "(11) 3333-4444"),
                    SupplierAddress = new Address("Av. Industrial, 500", "São Paulo", "São Paulo", "01000000", "Brasil"),
                    KitchenId = padaria.Id
                },
                new()
                {
                    Name = "Ovos Caipira da Serra",
                    SupplierContact = new Contact("pedidos@ovoscaipira.com", "(11) 99988-7766"),
                    SupplierAddress = new Address("Estrada da Serra, KM 45", "São Paulo", "São Paulo", "01000000", "Brasil"),
                    KitchenId = padaria.Id
                },
                // Maria's suppliers
                new()
                {
                    Name = "Mercado Central",
                    SupplierContact = new Contact("compras@mercadocentral.com", "(11) 2222-3333"),
                    SupplierAddress = new Address("Rua do Comércio, 100", "São Paulo", "São Paulo", "01000000", "Brasil"),
                    KitchenId = mariaKitchen.Id
                },
                // Demo suppliers
                new()
                {
                    Name = "Premium Foods Distributor",
                    SupplierContact = new Contact("sales@premiumfoods.com", "(11) 5555-1234"),
                    SupplierAddress = new Address("Distribution Center, 999", "São Paulo", "São Paulo", "01000000", "Brasil"),
                    KitchenId = demoKitchen.Id
                },
                new()
                {
                    Name = "Fresh Produce Co.",
                    SupplierContact = new Contact("orders@freshproduce.com", "(11) 5555-5678"),
                    SupplierAddress = new Address("Market Street, 200", "São Paulo", "São Paulo", "01000000", "Brasil"),
                    KitchenId = demoKitchen.Id
                }
            };

            await context.Suppliers.AddRangeAsync(suppliers);
            await context.SaveChangesAsync();

            // ==========================================
            // 5. SEED INGREDIENTS
            // ==========================================
            var ingredients = new List<IngredientEntity>
            {
                // Sorveteria ingredients
                new()
                {
                    Name = "Leite Integral",
                    Description = "Leite integral pasteurizado tipo A",
                    UnitOfMeasureId = units[2].Id, // Liter
                    CurrentStock = 50,
                    MinimumStock = 20,
                    KitchenId = sorveteria.Id
                },
                new()
                {
                    Name = "Creme de Leite Fresco",
                    Description = "Creme de leite 35% gordura",
                    UnitOfMeasureId = units[2].Id, // Liter
                    CurrentStock = 15,
                    MinimumStock = 10,
                    KitchenId = sorveteria.Id
                },
                new()
                {
                    Name = "Açúcar Cristal",
                    Description = "Açúcar cristal especial",
                    UnitOfMeasureId = units[0].Id, // KG
                    CurrentStock = 25,
                    MinimumStock = 10,
                    KitchenId = sorveteria.Id
                },
                new()
                {
                    Name = "Morango Fresco",
                    Description = "Morangos frescos selecionados",
                    UnitOfMeasureId = units[0].Id, // KG
                    CurrentStock = 5,
                    MinimumStock = 3,
                    KitchenId = sorveteria.Id
                },
                new()
                {
                    Name = "Chocolate em Pó 50%",
                    Description = "Cacau em pó alcalino",
                    UnitOfMeasureId = units[0].Id, // KG
                    CurrentStock = 8,
                    MinimumStock = 5,
                    KitchenId = sorveteria.Id
                },
                
                // Padaria ingredients
                new()
                {
                    Name = "Farinha de Trigo Especial",
                    Description = "Farinha para pães tipo 1",
                    UnitOfMeasureId = units[0].Id, // KG
                    CurrentStock = 100,
                    MinimumStock = 50,
                    KitchenId = padaria.Id
                },
                new()
                {
                    Name = "Fermento Biológico Seco",
                    Description = "Fermento instantâneo para pães",
                    UnitOfMeasureId = units[1].Id, // Gram
                    CurrentStock = 500,
                    MinimumStock = 200,
                    KitchenId = padaria.Id
                },
                new()
                {
                    Name = "Ovos Grandes",
                    Description = "Ovos tipo grande",
                    UnitOfMeasureId = units[5].Id, // Dozen
                    CurrentStock = 10,
                    MinimumStock = 5,
                    KitchenId = padaria.Id
                },
                new()
                {
                    Name = "Manteiga Sem Sal",
                    Description = "Manteiga extra sem sal",
                    UnitOfMeasureId = units[0].Id, // KG
                    CurrentStock = 5,
                    MinimumStock = 3,
                    KitchenId = padaria.Id
                },
                new()
                {
                    Name = "Leite Integral",
                    Description = "Leite integral tipo A",
                    UnitOfMeasureId = units[2].Id, // Liter
                    CurrentStock = 20,
                    MinimumStock = 10,
                    KitchenId = padaria.Id
                },
                
                // Maria's ingredients
                new()
                {
                    Name = "Frango Desfiado",
                    Description = "Peito de frango cozido e desfiado",
                    UnitOfMeasureId = units[0].Id, // KG
                    CurrentStock = 10,
                    MinimumStock = 5,
                    KitchenId = mariaKitchen.Id
                },
                new()
                {
                    Name = "Arroz Integral",
                    Description = "Arroz integral tipo 1",
                    UnitOfMeasureId = units[0].Id, // KG
                    CurrentStock = 20,
                    MinimumStock = 10,
                    KitchenId = mariaKitchen.Id
                },
                new()
                {
                    Name = "Brócolis",
                    Description = "Brócolis fresco",
                    UnitOfMeasureId = units[0].Id, // KG
                    CurrentStock = 3,
                    MinimumStock = 2,
                    KitchenId = mariaKitchen.Id
                },
                new()
                {
                    Name = "Batata Doce",
                    Description = "Batata doce rosada",
                    UnitOfMeasureId = units[0].Id, // KG
                    CurrentStock = 15,
                    MinimumStock = 8,
                    KitchenId = mariaKitchen.Id
                },
                
                // Demo ingredients (low stock examples)
                new()
                {
                    Name = "Premium Olive Oil",
                    Description = "Extra virgin olive oil",
                    UnitOfMeasureId = units[2].Id, // Liter
                    CurrentStock = 2, // Low stock
                    MinimumStock = 5,
                    KitchenId = demoKitchen.Id
                },
                new()
                {
                    Name = "Sea Salt",
                    Description = "Coarse sea salt",
                    UnitOfMeasureId = units[0].Id, // KG
                    CurrentStock = 1, // Low stock
                    MinimumStock = 3,
                    KitchenId = demoKitchen.Id
                },
                new()
                {
                    Name = "Black Pepper",
                    Description = "Whole black peppercorns",
                    UnitOfMeasureId = units[1].Id, // Gram
                    CurrentStock = 100, // Low stock
                    MinimumStock = 250,
                    KitchenId = demoKitchen.Id
                }
            };

            await context.Ingredients.AddRangeAsync(ingredients);
            await context.SaveChangesAsync();

            // ==========================================
            // 6. SEED STOCK ENTRIES (Price History)
            // ==========================================
            var stockEntries = new List<StockEntryEntity>
            {
                // Sorveteria - Leite (showing price variation)
                new()
                {
                    IngredientId = ingredients[0].Id,
                    MovementType = StockMovementType.Purchase,
                    Quantity = 100,
                    UnitPrice = 4.50m,
                    SupplierId = suppliers[0].Id,
                    Reason = "Compra mensal",
                    Reference = "NF-001",
                    MovementDate = DateTime.UtcNow.AddDays(-60)
                },
                new()
                {
                    IngredientId = ingredients[0].Id,
                    MovementType = StockMovementType.Usage,
                    Quantity = -30,
                    Reason = "Produção sorvete de chocolate",
                    MovementDate = DateTime.UtcNow.AddDays(-55)
                },
                new()
                {
                    IngredientId = ingredients[0].Id,
                    MovementType = StockMovementType.Purchase,
                    Quantity = 50,
                    UnitPrice = 4.75m, // Price increase
                    SupplierId = suppliers[0].Id,
                    Reason = "Reposição de estoque",
                    Reference = "NF-045",
                    MovementDate = DateTime.UtcNow.AddDays(-30)
                },
                new()
                {
                    IngredientId = ingredients[0].Id,
                    MovementType = StockMovementType.Usage,
                    Quantity = -20,
                    Reason = "Produção sorvete de morango",
                    MovementDate = DateTime.UtcNow.AddDays(-25)
                },
                new()
                {
                    IngredientId = ingredients[0].Id,
                    MovementType = StockMovementType.Purchase,
                    Quantity = 30,
                    UnitPrice = 5.00m, // Another price increase
                    SupplierId = suppliers[0].Id,
                    Reason = "Compra emergencial",
                    Reference = "NF-089",
                    MovementDate = DateTime.UtcNow.AddDays(-7)
                },
                
                // Morango - showing seasonal price variation
                new()
                {
                    IngredientId = ingredients[3].Id,
                    MovementType = StockMovementType.Purchase,
                    Quantity = 10,
                    UnitPrice = 8.00m,
                    SupplierId = suppliers[1].Id,
                    Reason = "Compra semanal",
                    Reference = "NF-F001",
                    MovementDate = DateTime.UtcNow.AddDays(-14)
                },
                new()
                {
                    IngredientId = ingredients[3].Id,
                    MovementType = StockMovementType.Usage,
                    Quantity = -5,
                    Reason = "Produção sorvete de morango",
                    MovementDate = DateTime.UtcNow.AddDays(-10)
                },
                new()
                {
                    IngredientId = ingredients[3].Id,
                    MovementType = StockMovementType.Purchase,
                    Quantity = 8,
                    UnitPrice = 12.00m, // Seasonal price increase
                    SupplierId = suppliers[1].Id,
                    Reason = "Fora de temporada",
                    Reference = "NF-F015",
                    MovementDate = DateTime.UtcNow.AddDays(-3)
                },
                
                // Padaria - Farinha
                new()
                {
                    IngredientId = ingredients[5].Id,
                    MovementType = StockMovementType.Purchase,
                    Quantity = 150,
                    UnitPrice = 3.20m,
                    SupplierId = suppliers[2].Id,
                    Reason = "Compra mensal",
                    Reference = "NF-M001",
                    MovementDate = DateTime.UtcNow.AddDays(-45)
                },
                new()
                {
                    IngredientId = ingredients[5].Id,
                    MovementType = StockMovementType.Usage,
                    Quantity = -50,
                    Reason = "Produção pão francês",
                    MovementDate = DateTime.UtcNow.AddDays(-40)
                },
                new()
                {
                    IngredientId = ingredients[5].Id,
                    MovementType = StockMovementType.Purchase,
                    Quantity = 100,
                    UnitPrice = 3.35m,
                    SupplierId = suppliers[2].Id,
                    Reason = "Reposição",
                    Reference = "NF-M025",
                    MovementDate = DateTime.UtcNow.AddDays(-15)
                },
                
                // Maria's kitchen - showing waste entry
                new()
                {
                    IngredientId = ingredients[12].Id, // Brócolis
                    MovementType = StockMovementType.Purchase,
                    Quantity = 10,
                    UnitPrice = 6.50m,
                    SupplierId = suppliers[4].Id,
                    Reason = "Compra semanal",
                    Reference = "CF-001",
                    MovementDate = DateTime.UtcNow.AddDays(-7)
                },
                new()
                {
                    IngredientId = ingredients[12].Id,
                    MovementType = StockMovementType.Usage,
                    Quantity = -5,
                    Reason = "Produção marmitas",
                    MovementDate = DateTime.UtcNow.AddDays(-5)
                },
                new()
                {
                    IngredientId = ingredients[12].Id,
                    MovementType = StockMovementType.Waste,
                    Quantity = -2,
                    Reason = "Produto vencido",
                    MovementDate = DateTime.UtcNow.AddDays(-1)
                },
                
                // Demo kitchen - recent purchases
                new()
                {
                    IngredientId = ingredients[14].Id, // Olive Oil
                    MovementType = StockMovementType.Purchase,
                    Quantity = 12,
                    UnitPrice = 45.00m,
                    SupplierId = suppliers[5].Id,
                    Reason = "Monthly purchase",
                    Reference = "INV-2024-001",
                    MovementDate = DateTime.UtcNow.AddDays(-30)
                },
                new()
                {
                    IngredientId = ingredients[14].Id,
                    MovementType = StockMovementType.Usage,
                    Quantity = -10,
                    Reason = "Daily operations",
                    MovementDate = DateTime.UtcNow.AddDays(-15)
                }
            };

            await context.StockEntries.AddRangeAsync(stockEntries);
            await context.SaveChangesAsync();

            // ==========================================
            // 7. SEED RECIPES
            // ==========================================
            var recipes = new List<RecipeEntity>
            {
                // Sorveteria recipes
                new()
                {
                    Name = "Sorvete de Chocolate",
                    Description = "Sorvete cremoso de chocolate 50% cacau",
                    Yield = 10, // 10 porções
                    PrepTimeMinutes = 45,
                    Instructions = "1. Aquecer o leite até 85°C\n2. Adicionar chocolate em pó\n3. Misturar creme e açúcar\n4. Resfriar e bater na sorveteira",
                    SellingPrice = 8.50m,
                    KitchenId = sorveteria.Id
                },
                new()
                {
                    Name = "Sorvete de Morango",
                    Description = "Sorvete artesanal com morangos frescos",
                    Yield = 8,
                    PrepTimeMinutes = 40,
                    Instructions = "1. Processar morangos frescos\n2. Misturar com leite e creme\n3. Adicionar açúcar\n4. Bater na sorveteira",
                    SellingPrice = 9.00m,
                    KitchenId = sorveteria.Id
                },
                
                // Padaria recipes
                new()
                {
                    Name = "Pão Francês",
                    Description = "Pão francês tradicional crocante",
                    Yield = 50, // 50 unidades
                    PrepTimeMinutes = 180,
                    Instructions = "1. Misturar farinha, água, fermento e sal\n2. Sovar por 10 minutos\n3. Descanso de 1 hora\n4. Modelar e fermentar\n5. Assar a 200°C",
                    SellingPrice = 0.75m, // por unidade
                    KitchenId = padaria.Id
                },
                new()
                {
                    Name = "Croissant de Manteiga",
                    Description = "Croissant folhado artesanal",
                    Yield = 20,
                    PrepTimeMinutes = 360,
                    Instructions = "1. Fazer massa base\n2. Laminar com manteiga\n3. Dobras sucessivas\n4. Modelar crescentes\n5. Fermentar e assar",
                    SellingPrice = 6.50m,
                    KitchenId = padaria.Id
                },
                
                // Maria's recipes
                new()
                {
                    Name = "Marmita Fitness Frango",
                    Description = "Marmita balanceada com frango, arroz integral e brócolis",
                    Yield = 5,
                    PrepTimeMinutes = 60,
                    Instructions = "1. Cozinhar arroz integral\n2. Grelhar frango temperado\n3. Cozinhar brócolis no vapor\n4. Montar marmitas",
                    SellingPrice = 18.00m,
                    KitchenId = mariaKitchen.Id
                },
                new()
                {
                    Name = "Bowl Energia",
                    Description = "Bowl com batata doce, frango e vegetais",
                    Yield = 4,
                    PrepTimeMinutes = 45,
                    Instructions = "1. Assar batata doce\n2. Preparar frango grelhado\n3. Saltear vegetais\n4. Montar bowls",
                    SellingPrice = 22.00m,
                    KitchenId = mariaKitchen.Id
                }
            };

            await context.Recipes.AddRangeAsync(recipes);
            await context.SaveChangesAsync();

            // ==========================================
            // 8. SEED RECIPE INGREDIENTS
            // ==========================================
            var recipeIngredients = new List<RecipeIngredientEntity>
            {
                // Sorvete de Chocolate
                new() { RecipeId = recipes[0].Id, IngredientId = ingredients[0].Id, Quantity = 2, Notes = "Leite integral" },
                new() { RecipeId = recipes[0].Id, IngredientId = ingredients[1].Id, Quantity = 1, Notes = "Creme fresco" },
                new() { RecipeId = recipes[0].Id, IngredientId = ingredients[2].Id, Quantity = 0.5m, Notes = "Açúcar" },
                new() { RecipeId = recipes[0].Id, IngredientId = ingredients[4].Id, Quantity = 0.3m, Notes = "Cacau em pó" },
                
                // Sorvete de Morango
                new() { RecipeId = recipes[1].Id, IngredientId = ingredients[0].Id, Quantity = 1.5m, Notes = "" },
                new() { RecipeId = recipes[1].Id, IngredientId = ingredients[1].Id, Quantity = 0.8m, Notes = "" },
                new() { RecipeId = recipes[1].Id, IngredientId = ingredients[2].Id, Quantity = 0.4m, Notes = "" },
                new() { RecipeId = recipes[1].Id, IngredientId = ingredients[3].Id, Quantity = 1, Notes = "Morangos frescos" },
                
                // Pão Francês
                new() { RecipeId = recipes[2].Id, IngredientId = ingredients[5].Id, Quantity = 5, Notes = "Farinha especial" },
                new() { RecipeId = recipes[2].Id, IngredientId = ingredients[6].Id, Quantity = 30, Notes = "Fermento seco" },
                
                // Croissant
                new() { RecipeId = recipes[3].Id, IngredientId = ingredients[5].Id, Quantity = 3, Notes = "" },
                new() { RecipeId = recipes[3].Id, IngredientId = ingredients[6].Id, Quantity = 20, Notes = "" },
                new() { RecipeId = recipes[3].Id, IngredientId = ingredients[8].Id, Quantity = 0.5m, Notes = "Para laminação" },
                new() { RecipeId = recipes[3].Id, IngredientId = ingredients[9].Id, Quantity = 0.5m, Notes = "" },
                new() { RecipeId = recipes[3].Id, IngredientId = ingredients[7].Id, Quantity = 0.5m, Notes = "Para pincelar" },
                
                // Marmita Fitness
                new() { RecipeId = recipes[4].Id, IngredientId = ingredients[10].Id, Quantity = 1, Notes = "Frango desfiado" },
                new() { RecipeId = recipes[4].Id, IngredientId = ingredients[11].Id, Quantity = 0.75m, Notes = "Arroz cozido" },
                new() { RecipeId = recipes[4].Id, IngredientId = ingredients[12].Id, Quantity = 0.5m, Notes = "Brócolis no vapor" },
                
                // Bowl Energia
                new() { RecipeId = recipes[5].Id, IngredientId = ingredients[10].Id, Quantity = 0.8m, Notes = "" },
                new() { RecipeId = recipes[5].Id, IngredientId = ingredients[13].Id, Quantity = 1.2m, Notes = "Batata assada" },
                new() { RecipeId = recipes[5].Id, IngredientId = ingredients[12].Id, Quantity = 0.4m, Notes = "" }
            };

            await context.RecipeIngredients.AddRangeAsync(recipeIngredients);
            await context.SaveChangesAsync();

            await transaction.CommitAsync();

            logger?.LogDebug("✅ Database seeded successfully!");
            logger?.LogDebug($"  - {users.Count} users created");
            logger?.LogDebug($"  - {kitchens.Count} kitchens created");
            logger?.LogDebug($"  - {units.Count} units of measure created");
            logger?.LogDebug($"  - {ingredients.Count} ingredients created");
            logger?.LogDebug($"  - {suppliers.Count} suppliers created");
            logger?.LogDebug($"  - {stockEntries.Count} stock entries created");
            logger?.LogDebug($"  - {recipes.Count} recipes created");
            logger?.LogDebug($"  - {recipeIngredients.Count} recipe ingredients created");
            logger?.LogDebug("\n📧 Test accounts:");
            logger?.LogDebug("  Email: fernando@example.com | Password: Test123! (Premium)");
            logger?.LogDebug("  Email: maria@example.com | Password: Test123! (Basic)");
            logger?.LogDebug("  Email: demo@kitchenstock.com | Password: Demo123! (Premium)");
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            logger?.LogDebug($"❌ Error seeding database: {ex.Message}");
            throw;
        }
    }
}
