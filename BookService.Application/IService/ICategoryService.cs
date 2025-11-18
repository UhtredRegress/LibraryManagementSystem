using BookService.Application.DTO;

namespace BookService.Application;

public interface ICategoryService
{
    Task<CategoryDTO> CreateCategoryAsync(string categoryName);
    Task<CategoryDTO> UpdateCategoryAsync(int id, string categoryName);
    Task DeleteCategoryAsync(int id);
}