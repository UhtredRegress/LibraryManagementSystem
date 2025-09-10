using LMS.BorrowService.Domain.Entity;
using Microsoft.EntityFrameworkCore;

namespace LMS.BorrowService.Infrastructure;

public class BorrowDbContext : DbContext
{
    public BorrowDbContext(DbContextOptions<BorrowDbContext> options) : base(options)
    {

    }

    public DbSet<Borrower> Borrowers { get; set; }
    public DbSet<BorrowHistory> BorrowHistories { get; set; }
}