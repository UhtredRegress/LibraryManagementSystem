using BookService.Application;
using BookService.Application.DTO;
using BookService.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookService.Presentation.Controllers;

[Authorize]
[ApiController]
[Route("categories")]
public class CategoryController:ControllerBase
{
    private readonly ILogger<CategoryController> _logger;
    private readonly ICategoryService _categoryService;

    public CategoryController(ILogger<CategoryController> logger, ICategoryService categoryService)
    {
        _logger = logger;
        _categoryService = categoryService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateCategory([FromBody] string categoryName)
    {
        _logger.LogInformation("Receive request to add new category");
        try
        {
            var result = await _categoryService.CreateCategoryAsync(categoryName);
            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (InvalidDataException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return BadRequest("There was error with server please try again later");
        }
    }

    [HttpPut("update/{id:int}")]
    public async Task<IActionResult> UpdateCategory([FromRoute] int id, [FromBody] string categoryName)
    {
        _logger.LogInformation("Receive request to update category");
        try
        {
            var result = await _categoryService.UpdateCategoryAsync(id, categoryName);
            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (InvalidDataException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return BadRequest("There was error with server please try again later");
        }
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteCategory([FromRoute] int id)
    {
        _logger.LogInformation("Receive request to delete category");
        try
        {
            await _categoryService.DeleteCategoryAsync(id);
            return Ok("You have successfully deleted the category");
        }
        catch (InvalidDataException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return BadRequest("There was error with server please try again later");
        }
    }
}