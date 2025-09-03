using Microsoft.EntityFrameworkCore;

namespace LMS.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Role> Roles { get; set; }

    protected override void OnModelCreateing(ModelBuilder modelBuilder)
    {
        base.OnModelCreateing(modelBuilder); 
    }
}