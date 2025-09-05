using Microsoft.EntityFrameworkCore;
using Shared.Common.Persistence.Abstractions;
using User.Domain.Entities;

namespace User.Infrastructure;

public class UserDbContext : BaseDbContext
{
    public DbSet<UserEntity> Users { get; set; }

    public UserDbContext(DbContextOptions<UserDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserEntity>(entity =>
        {
            entity.HasKey(u => u.Id);

            entity.HasIndex(u => u.Email)
                .IsUnique();

            entity.Property(u => u.Email)
                .IsRequired()
                .HasMaxLength(254);

            entity.Property(u => u.Name)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(u => u.Plan)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(50);

            entity.Property(u => u.PasswordHash)
                .IsRequired()
                .HasMaxLength(256);
        });

        base.OnModelCreating(modelBuilder);
    }
}
