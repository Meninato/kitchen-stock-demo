using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Shared.Common.Persistence.Abstractions;

public abstract class BaseDbContext : DbContext
{
    protected BaseDbContext(DbContextOptions options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ApplyBaseConfiguration(modelBuilder);
    }

    public override int SaveChanges()
    {
        HandleEntityState();
        return base.SaveChanges();
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        HandleEntityState();
        return await base.SaveChangesAsync(cancellationToken);
    }

    public int SaveChangesWithHardDelete()
    {
        HandleEntityState(isHardDelete: true);
        return base.SaveChanges();
    }

    public async Task<int> SaveChangesWithHardDeleteAsync(CancellationToken cancellationToken = default)
    {
        HandleEntityState(isHardDelete: true);
        return await base.SaveChangesAsync(cancellationToken);
    }

    private void ApplyBaseConfiguration(ModelBuilder modelBuilder)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(BaseEntity).IsAssignableFrom(entityType.ClrType))
            {
                var parameter = Expression.Parameter(entityType.ClrType, "e");
                var property = Expression.Property(parameter, nameof(BaseEntity.IsDeleted));
                var filter = Expression.Lambda(Expression.Equal(property, Expression.Constant(false)), parameter);

                entityType.SetQueryFilter(filter);
            }
        }
    }

    private void HandleEntityState(bool isHardDelete = false)
    {
        var entries = ChangeTracker.Entries<BaseEntity>();

        foreach (var entry in entries)
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedAt = DateTime.UtcNow;
                    entry.Entity.UpdatedAt = DateTime.UtcNow;
                    break;

                case EntityState.Modified:
                    entry.Entity.UpdatedAt = DateTime.UtcNow;
                    break;

                case EntityState.Deleted:
                    if (!isHardDelete)
                    {
                        entry.State = EntityState.Modified;
                        entry.Entity.IsDeleted = true;
                        entry.Entity.UpdatedAt = DateTime.UtcNow;
                    }
                    break;
            }
        }
    }
}