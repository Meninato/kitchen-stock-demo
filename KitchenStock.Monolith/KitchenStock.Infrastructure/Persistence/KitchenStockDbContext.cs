using KitchenStock.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace KitchenStock.Infrastructure.Persistence;

public class KitchenStockDbContext : BaseDbContext
{
    public DbSet<UserEntity> Users { get; set; }
    public DbSet<KitchenEntity> Kitchens { get; set; }
    public DbSet<IngredientEntity> Ingredients { get; set; }
    public DbSet<RecipeEntity> Recipes { get; set; }
    public DbSet<RecipeIngredientEntity> RecipeIngredients { get; set; }
    public DbSet<StockEntryEntity> StockEntries { get; set; }
    public DbSet<UnitOfMeasureEntity> UnitsOfMeasure { get; set; }
    public DbSet<SupplierEntity> Suppliers { get; set; }

    public KitchenStockDbContext(DbContextOptions<KitchenStockDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ConfigureUsers(modelBuilder);
        ConfigureKitchens(modelBuilder);
        ConfigureIngredients(modelBuilder);
        ConfigureRecipes(modelBuilder);
        ConfigureStockEntries(modelBuilder);
        ConfigureSuppliers(modelBuilder);
        ConfigureUnitsOfMeasure(modelBuilder);

        base.OnModelCreating(modelBuilder);
    }

    private void ConfigureUsers(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserEntity>(entity =>
        {
            entity.HasKey(u => u.Id);
            entity.HasIndex(u => u.Email).IsUnique();

            entity.Property(u => u.Email)
                .IsRequired()
                .HasMaxLength(254);

            entity.Property(u => u.Name)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(u => u.PasswordHash)
                .IsRequired()
                .HasMaxLength(256);

            entity.Property(u => u.Plan)
                .IsRequired()
                .HasConversion<string>();
        });
    }

    private void ConfigureKitchens(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<KitchenEntity>(entity =>
        {
            entity.HasKey(k => k.Id);

            entity.Property(k => k.Name)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(k => k.Description)
                .HasMaxLength(500);

            entity.HasOne(k => k.Owner)
                .WithMany(u => u.Kitchens)
                .HasForeignKey(k => k.OwnerId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }

    private void ConfigureIngredients(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<IngredientEntity>(entity =>
        {
            entity.HasKey(i => i.Id);

            entity.Property(i => i.Name)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(i => i.Description)
                .HasMaxLength(500);

            entity.Property(i => i.CurrentStock)
                .HasPrecision(10, 4);

            entity.Property(i => i.MinimumStock)
                .HasPrecision(10, 4);

            entity.HasOne(i => i.UnitOfMeasure)
                .WithMany(u => u.Ingredients)
                .HasForeignKey(i => i.UnitOfMeasureId);

            entity.HasOne(i => i.Kitchen)
                .WithMany(k => k.Ingredients)
                .HasForeignKey(i => i.KitchenId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }

    private void ConfigureRecipes(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<RecipeEntity>(entity =>
        {
            entity.HasKey(r => r.Id);

            entity.Property(r => r.Name)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(r => r.Description)
                .HasMaxLength(1000);

            entity.Property(r => r.Yield)
                .HasPrecision(8, 2);

            entity.Property(r => r.SellingPrice)
                .HasPrecision(10, 2);

            entity.HasOne(r => r.Kitchen)
                .WithMany(k => k.Recipes)
                .HasForeignKey(r => r.KitchenId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Configuração da tabela de junção Recipe-Ingredient
        modelBuilder.Entity<RecipeIngredientEntity>(entity =>
        {
            entity.HasKey(r => r.Id);

            entity.HasIndex(ri => new { ri.RecipeId, ri.IngredientId }).IsUnique();

            entity.Property(ri => ri.Quantity)
                .HasPrecision(10, 4);

            entity.Property(ri => ri.Notes)
                .HasMaxLength(200);

            entity.HasOne(ri => ri.Recipe)
                .WithMany(r => r.RecipeIngredients)
                .HasForeignKey(ri => ri.RecipeId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(ri => ri.Ingredient)
                .WithMany(i => i.RecipeIngredients)
                .HasForeignKey(ri => ri.IngredientId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }

    private void ConfigureStockEntries(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<StockEntryEntity>(entity =>
        {
            entity.HasKey(se => se.Id);

            entity.Property(se => se.Quantity)
                .HasPrecision(10, 4);

            entity.Property(se => se.UnitPrice)
                .HasPrecision(10, 4);

            entity.Property(se => se.MovementType)
                .IsRequired()
                .HasConversion<string>();

            entity.Property(se => se.Reason)
                .HasMaxLength(200);

            entity.Property(se => se.Reference)
                .HasMaxLength(50);

            entity.HasOne(se => se.Ingredient)
                .WithMany(i => i.StockEntries)
                .HasForeignKey(se => se.IngredientId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(se => se.Supplier)
                .WithMany(s => s.StockEntries)
                .HasForeignKey(se => se.SupplierId)
                .OnDelete(DeleteBehavior.SetNull);
        });
    }

    private void ConfigureSuppliers(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<SupplierEntity>(entity =>
        {
            entity.HasKey(s => s.Id);

            entity.Property(s => s.Name)
                .IsRequired()
                .HasMaxLength(100);

            entity.OwnsOne(s => s.SupplierAddress, address =>
            {
                address.Property(a => a.Street).HasMaxLength(255);
                address.Property(a => a.City).HasMaxLength(100);
                address.Property(a => a.State).HasMaxLength(100);
                address.Property(a => a.PostalCode).HasMaxLength(20);
                address.Property(a => a.Country).HasMaxLength(100);
            });

            entity.OwnsOne(s => s.SupplierContact, contact => 
            {
                contact.Property(c => c.Email).HasMaxLength(254);
                contact.Property(c => c.Phone).HasMaxLength(50);
            });

            entity.HasOne(s => s.Kitchen)
                .WithMany(k => k.Suppliers)
                .HasForeignKey(s => s.KitchenId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }

    private void ConfigureUnitsOfMeasure(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UnitOfMeasureEntity>(entity =>
        {
            entity.HasKey(u => u.Id);

            entity.Property(u => u.Name)
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(u => u.Symbol)
                .IsRequired()
                .HasMaxLength(10);

            entity.HasIndex(u => u.Symbol).IsUnique();
        });
    }
}
