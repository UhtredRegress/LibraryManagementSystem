using Microsoft.EntityFrameworkCore;
using NotificationService.Models;

namespace NotificationService.DbContext;

public class NotiDbContext : Microsoft.EntityFrameworkCore.DbContext
{
    public NotiDbContext(DbContextOptions<NotiDbContext> options) : base(options)
    {
        
    }
    
    public DbSet<User> Users { get; set; }
    public DbSet<BookCategory> BookCategories { get; set; }
    public DbSet<UserNotiSubscription>  NotiSubscriptions { get; set; }
    public DbSet<Notification> Notifications { get; set; }
    public DbSet<NotificationStatus> NotificationStatuses { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserNotiSubscription>()
            .HasKey(sub => new { sub.UserId, sub.BookCategoryId });
        modelBuilder.Entity<UserNotiSubscription>() 
            .HasOne(sub => sub.User)
            .WithMany(u => u.UserNotiSubscriptions)
            .HasForeignKey(sub => sub.UserId);
        modelBuilder.Entity<UserNotiSubscription>()
            .HasOne(sub => sub.BookCategory)
            .WithMany(c => c.UserNotiSubscriptions)
            .HasForeignKey(sub => sub.BookCategoryId);
        
        modelBuilder.Entity<NotificationStatus>()
            .HasKey(ns => new { ns.NotificationId, ns.UserId });
        modelBuilder.Entity<NotificationStatus>()
            .HasOne(ns => ns.Notification)
            .WithMany(n => n.NotificationStatuses)
            .HasForeignKey(ns => ns.NotificationId);
        modelBuilder.Entity<NotificationStatus>()
            .HasOne(ns => ns.User)
            .WithMany(u => u.NotificationStatuses)
            .HasForeignKey(ns => ns.UserId);
    }
}