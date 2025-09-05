using LMS.Domain.Model;
using Microsoft.EntityFrameworkCore;

namespace LMS.Infrastructure;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<Book> Books { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>()
            .HasIndex(u => u.Username)
            .IsUnique();

        modelBuilder.Entity<Role>()
            .HasIndex(r => r.Title)
            .IsUnique();

        modelBuilder.Entity<Book>(entity =>
            entity.Property(b => b.PublishDate).HasColumnType("date")
        );
    }
}