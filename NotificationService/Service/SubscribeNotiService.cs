using Microsoft.EntityFrameworkCore;
using NotificationService.Controller;
using NotificationService.DbContext;
using NotificationService.DTOs;
using NotificationService.Models;
using NotificationService.Service.Interface;

namespace NotificationService.Service;

public class SubscribeNotiService : ISubscribeNotiService
{
    private readonly ILogger<SubscribeNotiController> _logger; 
    private readonly NotiDbContext _dbContext;

    public SubscribeNotiService(ILogger<SubscribeNotiController> logger, NotiDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }
    
    public async Task<BookNotiCategoryResponseDto> SubscribeNotiForCategory(string username, string email, string phone, int categoryId)
    {
        _logger.LogInformation("Start retrieve user in database with username {username}", username);
        var foundUser = await _dbContext.Users.FirstOrDefaultAsync(u => u.Username == username);

        if (foundUser == null)
        {
            _logger.LogInformation("User not found in database create new user with information {username} - {email} - {phone}", username, email, phone); 
            var newUser = new User(username: username, email: email, phoneNumber: phone);
            var tempUser = await _dbContext.Users.AddAsync(newUser);
            await _dbContext.SaveChangesAsync();
            _logger.LogInformation("Persistence data for new user successfully");
            foundUser = tempUser.Entity;
        }
        
        _logger.LogInformation("Check book id {categoryId} in database", categoryId);
        var foundCategory = await _dbContext.BookCategories.FirstOrDefaultAsync(c => c.Id == categoryId);
        if (foundCategory == null)
        {
            _logger.LogInformation("Category not found in database throwing exception");
            throw new ArgumentException($"Category id {categoryId} does not exist in database");
        } 
        
        _logger.LogInformation("Check whether user has already subscribed for this category");
        var foundRelation = _dbContext.NotiSubscriptions.FirstOrDefault(n => n.UserId == foundUser.Id && n.BookCategoryId == categoryId);
        if (foundRelation != null)
        {
            _logger.LogInformation("Found user has already subscribed for this category throwing exception");
            throw new ArgumentException($"User {username} has already been subscribed to receive notification for this category id {categoryId}");
        }
        _logger.LogInformation("Persistence data for new user subscription");
        var newRelation = new UserNotiSubscription(user: foundUser, bookCategory: foundCategory);
        await _dbContext.NotiSubscriptions.AddAsync(newRelation);
        await _dbContext.SaveChangesAsync();
        _logger.LogInformation("Handler for this request successfully operate");
        return new BookNotiCategoryResponseDto { User = new UserDto(foundUser.Id, foundUser.Username, foundUser.Email, foundUser.PhoneNumber), Category = new CategoryDto(foundCategory.Id, foundCategory.Name)};
    }
}