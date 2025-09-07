using Kitchen.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Shared.Common.Persistence.Abstractions;

namespace Kitchen.Infrastructure;

public class KitchenDbContext : BaseDbContext
{
    public DbSet<KitchenEntity> Kitchens { get; set; }

    public KitchenDbContext(DbContextOptions<KitchenDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<KitchenEntity>(entity =>
        {
            entity.HasKey(k => k.Id);

            entity.HasIndex(k => k.OwnerId);
            entity.HasIndex(k => new { k.OwnerId, k.IsActive });

            entity.Property(k => k.Name)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(k => k.Description)
                .HasMaxLength(500);

            // Índices para performance
            entity.HasIndex(k => k.IsActive);
            entity.HasIndex(k => k.Name).IsUnique();
        });

        base.OnModelCreating(modelBuilder);
    }
}
