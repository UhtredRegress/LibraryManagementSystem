using LMS.BookService.Domain.Model;

using Microsoft.EntityFrameworkCore;

namespace LMS.BookService.Infrastructure;

public class BookServiceDbContext:DbContext
{
    public BookServiceDbContext(DbContextOptions<BookServiceDbContext> options) : base(options)
    {
    }


    public DbSet<Book> Books { get; set; }
 

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.Entity<Book>()
            .HasIndex(b => b.Title);
        modelBuilder.Entity<Book>()
            .HasIndex(b => b.Author);
        modelBuilder.Entity<Book>()
            .HasIndex(b => b.PublishDate);
        
        modelBuilder.Entity<Book>(entity =>
            entity.Property(b => b.PublishDate).HasColumnType("date")
        );
    }}