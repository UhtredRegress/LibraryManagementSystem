using NotificationService.DTOs;
using NotificationService.Models;

namespace NotificationService.Service.Interface;

public interface ISubscribeNotiService
{
    Task<BookNotiCategoryResponseDto> SubscribeNotiForCategory(string username, string email, string phone, int categoryId);
}