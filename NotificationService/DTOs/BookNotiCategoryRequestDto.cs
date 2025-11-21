using NotificationService.Models;

namespace NotificationService.DTOs;

public class BookNotiCategoryRequestDto
{
    public int BookCategoryId { get; set; }
    public int UserId { get; set; }
}

public class BookNotiCategoryResponseDto
{
    public UserDto User { get; set; }
    public CategoryDto Category { get; set; }
}

public record UserDto(int Id, string Name, string Email, string Phone);
public record CategoryDto(int Id, string Name);