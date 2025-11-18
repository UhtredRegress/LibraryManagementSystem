using System.Text.RegularExpressions;
using BookService.Application.DTO;
using BookService.Domain.Model;
using BookService.Infrastructure;
using Microsoft.Extensions.Logging;

namespace BookService.Application;

public class CategoryService: ICategoryService
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly ILogger<CategoryService> _logger;

    public CategoryService(ICategoryRepository categoryRepository, ILogger<CategoryService> logger )
    {
        _categoryRepository = categoryRepository;
        _logger = logger;
    }
    
    public async Task<CategoryDTO> CreateCategoryAsync(string categoryName)
    {
        _logger.LogInformation("Start create new book category {CategoryName}", categoryName);
        
        _logger.LogInformation("Validate category name request");
        if (string.IsNullOrEmpty(categoryName) || string.IsNullOrWhiteSpace(categoryName))
        {
            _logger.LogInformation("Category name is empty or missing");
            throw new ArgumentException("Category name is empty or missing");
        }
        
        var regex = new Regex(@"^[A-Za-z0-9\s&'\/\-]+$");
        if (!regex.IsMatch(categoryName))
        {
            _logger.LogInformation("Category name {CategoryName} contains invalid character", categoryName);
            throw new ArgumentException("Category name contains invalid character");
        }
        
        _logger.LogInformation("Check whether the category name is already existed in database");
        var foundCategory = await _categoryRepository.FindAsync(c => c.Name == categoryName);
        if (foundCategory != null)
        {
            _logger.LogInformation("Category {CategoryName} is already existed in database", foundCategory.Name);
            throw new InvalidDataException("Category name already existed in database");
        }

        _logger.LogInformation("Create new book category {CategoryName}", categoryName);
        var newCategory = new Category(categoryName);
        
        _logger.LogInformation("Start store newly created category into database");
        await _categoryRepository.CreateAsync(newCategory);
        
        _logger.LogInformation("Map to DTO and return this to caller");
        var resultDTO = new CategoryDTO();
        resultDTO.Id = newCategory.Id;
        resultDTO.Name = newCategory.Name;
        
        return resultDTO;
    }   

    public async Task<CategoryDTO> UpdateCategoryAsync(int id, string categoryName)
    {
        _logger.LogInformation("Start update category with id {Id}", id);
        
        _logger.LogInformation("Find category need to be update in database");
        var foundCategory = await _categoryRepository.FindAsync(c => c.Id == id);
        if (foundCategory == null)
        {
            _logger.LogInformation("Category {Id} does not exist in database", id);
            throw new InvalidDataException("Category request to update does not exist in database");
        }
        
        _logger.LogInformation("Validate category name request");
        if (string.IsNullOrEmpty(categoryName) || string.IsNullOrWhiteSpace(categoryName))
        {
            _logger.LogInformation("Category name is empty or missing");
            throw new ArgumentException("Category name is empty or missing");
        }
        
        var regex = new Regex(@"^[A-Za-z0-9\s&'\/\-]+$");
        if (!regex.IsMatch(categoryName))
        {
            _logger.LogInformation("Category name {CategoryName} contains invalid character", categoryName);
            throw new ArgumentException("Category name contains invalid character");
        }

        if (categoryName == foundCategory.Name)
        {
            _logger.LogInformation("Category {CategoryName} is the same as old name", categoryName);
            throw new ArgumentException("Category name is the same as old name");
        }
        
        _logger.LogInformation("Check whether the category name is already existed in database");
        var foundCategoryName = await _categoryRepository.FindAsync(c => c.Name == categoryName);
        if (foundCategoryName != null)
        {
            _logger.LogInformation("Category {CategoryName} is already existed in database", foundCategory.Name);
            throw new InvalidDataException("Category name already existed in database");
        }
        
        _logger.LogInformation("Start update book category {CategoryName} and then store in database", categoryName);
        foundCategory.UpdateName(categoryName);
        await _categoryRepository.UpdateAsync(foundCategory);
        
        _logger.LogInformation("Map to DTO and return this to caller");
        
        var resultDTO = new CategoryDTO();
        resultDTO.Id = foundCategory.Id;
        resultDTO.Name = foundCategory.Name;
        return resultDTO;
    }

    public async Task DeleteCategoryAsync(int id)
    {
        _logger.LogInformation("Start delete category with id {Id}", id);
        
        _logger.LogInformation("Find category with id {Id} in database", id);
        var foundCategory = await _categoryRepository.FindAsync(c => c.Id == id);

        if (foundCategory == null)
        {
            _logger.LogInformation("Category {Id} does not exist in database", id);
            throw new InvalidDataException("Category request to delete does not exist in database");
        }
        
        _logger.LogInformation("Start delete category in database");
        await _categoryRepository.DeleteAsync(foundCategory);
    }
}