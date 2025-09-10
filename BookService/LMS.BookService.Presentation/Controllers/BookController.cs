using LMS.BookService.Application.IService;
using LMS.BookService.Domain.Enum;
using LMS.BookService.Domain.Model;
using LMS.Shared.Exception;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LMS.BookService.Presentation.Controllers;

[Authorize(Policy = "NumericRolePolicy")]
[ApiController]
[Route("api/book")]
public class BookController : ControllerBase
{
    private readonly IBookService _bookService;

    public BookController(IBookService bookService)
    {
        _bookService = bookService;
    }

    [HttpPost]
    public async Task<IActionResult> AddBook([FromBody] Book book)
    {
        try
        {
            var result = await _bookService.AddBookAsync(book);
            return Ok(result);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpPut("/update/{id}")]
    public async Task<IActionResult> UpdateBook(int id, Book book)
    {
        try
        {
            var result = await _bookService.UpdateBookAsync(id, book);
            return Ok( new {Result = result});
        }
        catch (NotFoundDataException e)
        {
            return NotFound(e.Message);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpDelete("/delete/{id}")]
    public async Task<IActionResult> DeleteBook(int id)
    {
        try
        {
            var result = await _bookService.DeleteBookAsync(id);
            return Ok(result);
        }
        catch (NotFoundDataException e)
        {
            return NotFound(e.Message);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpGet("/filter/published")]
    public async Task<IActionResult> GetPublishedDateBooks(DateTime startDate, DateTime endDate)
    {
        try
        {
            var result = await _bookService.GetBooksByPublishedDate(startDate, endDate);
            return Ok(result);
        }
        catch (NotFoundDataException e)
        {
            return NotFound(e.Message);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message); 
        }
    }

    [HttpGet("/filter/availability")]
    public async Task<IActionResult> GetBooksByAvailability(Availability availability)
    {
        try
        {
            var result = await _bookService.GetBooksByAvailability(availability);
            return Ok(result);
        }
        catch (NotFoundDataException e)
        {
            return NotFound(e.Message);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpGet("/filter/title")]
    public async Task<IActionResult> GetBooksByTitle(string title)
    {
        try
        {
            var result = await _bookService.GetBooksByTitle(title);
            return Ok(result);
        }
        catch (NotFoundDataException e)
        {
            return NotFound(e.Message);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
}