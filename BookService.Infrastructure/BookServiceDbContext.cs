using BookService.Domain.Model;

using Microsoft.EntityFrameworkCore;

namespace BookService.Infrastructure;

public class BookServiceDbContext:DbContext
{
    public BookServiceDbContext(DbContextOptions<BookServiceDbContext> options) : base(options)
    {
    }
    
    
    public DbSet<Author> Authors { get; set; }
    public DbSet<Book> Books { get; set; }
    public DbSet<BookPrice> BookPrices { get; set; }
    public DbSet<Category> Categories { get; set; }
    

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.Entity<Book>()
            .HasIndex(b => b.Title);
        modelBuilder.Entity<Book>()
            .HasIndex(b => b.PublishDate);
        
        modelBuilder.Entity<Book>(entity =>
            entity.Property(b => b.PublishDate).HasColumnType("date")
        );

        modelBuilder.Entity<BookPrice>(entity =>
            entity.Property(b => b.PriceUnit).HasColumnType("decimal(18,2)"));
        
        modelBuilder.Entity<BookPrice>().HasKey(price => new { price.BookId, price.BookType });
        
        modelBuilder.Entity<BookCategory>().HasKey(bc => new { bc.BookId, bc.CategoryId });
        modelBuilder.Entity<BookCategory>().HasOne(bc => bc.Book).WithMany(b => b.BookCategories).HasForeignKey(bc => bc.BookId);
        modelBuilder.Entity<BookCategory>().HasOne(bc => bc.Category).WithMany(b => b.BookCategories).HasForeignKey(bc => bc.CategoryId);
        
        modelBuilder.Entity<Category>().HasIndex(c => c.Name).IsUnique();
        modelBuilder.Entity<Author>().HasIndex(a => a.Name).IsUnique();
    }}