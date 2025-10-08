using BookService.Domain.Model;

using Microsoft.EntityFrameworkCore;

namespace BookService.Infrastructure;

public class BookServiceDbContext:DbContext
{
    public BookServiceDbContext(DbContextOptions<BookServiceDbContext> options) : base(options)
    {
    }


    public DbSet<Book> Books { get; set; }
    public DbSet<BookPrice> BookPrices { get; set; }

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

        modelBuilder.Entity<BookPrice>(entity =>
            entity.Property(b => b.PriceUnit).HasColumnType("decimal(18,2)"));
        
        modelBuilder.Entity<BookPrice>().HasKey(price => new { price.BookId, price.BookType });


    }}