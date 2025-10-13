using Microsoft.EntityFrameworkCore;
using PurchaseService.Domain.Aggregate;

namespace PurchaseService.Infrastructure;

public class PurchaseDbContext:DbContext 
{
    public PurchaseDbContext(DbContextOptions<PurchaseDbContext> options) : base(options)
    {
    }
    
    public DbSet<PurchaseBook>  PurchaseBooks { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.Entity<PurchaseBook>(entity =>
        {
            entity.Property(p => p.FinalCost).HasColumnType("decimal(18,2)");
        });
    }
}