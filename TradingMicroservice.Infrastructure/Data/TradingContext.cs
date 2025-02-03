using Microsoft.EntityFrameworkCore;
using TradingMicroservice.Core.Models;

namespace TradingMicroservice.Infrastructure.Data;

public class TradingContext(DbContextOptions<TradingContext> options) : DbContext(options)
{
    public DbSet<Trade> Trades { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Trade>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Symbol)
                .IsRequired()
                .HasMaxLength(10);

            entity.Property(e => e.ClientId)
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(e => e.Price)
                .HasPrecision(18, 8);

            entity.Property(e => e.Quantity)
                .HasPrecision(18, 8);

            entity.Property(e => e.ExecutionTime)
                .IsRequired();

            entity.HasIndex(e => e.ClientId);
            entity.HasIndex(e => e.ExecutionTime);
        });
    }
}
