using BookService.Application;
using BookService.Application.IService;
using BookService.Domain.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Enum;
using Shared.Exception;

namespace BookService.Presentation.Controllers;

[Authorize(Policy = "LibrarianNumericRolePolicy")]
[ApiController]
[Route("api/books")]
public class BookController : ControllerBase
{
    private readonly IBookService _bookService;
    

    public BookController(IBookService bookService)
    {
        _bookService = bookService;
    }

    [HttpPost]
    public async Task<IActionResult> AddBook(BookAddDTO BookDto)
    {
        try
        {
            var addedBook = await _bookService.AddBookAsync(BookDto);

            var resultAddDTO = new BookResultDTO(addedBook.Id, addedBook.Title, addedBook.Authors, addedBook.BookCategories,
                addedBook.Publisher, stock: addedBook.Stock, fileAddress: addedBook.FileAddress,
                type: addedBook.Type);

            return Ok(resultAddDTO);
        }
        catch (Exception e)
        {
            return BadRequest(new {Error = e.Message});
        }
    }

    [HttpPut("/update/{id}")]
    public async Task<IActionResult> UpdateBook([FromRoute]int id,[FromForm] BookAddDTO book)
    {
        try
        {
            var result = await _bookService.UpdateBookAsync(id, book);
            return Ok(new { Result = result });
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

    [HttpPost("upload/file/{id:int}")]
    public async Task<IActionResult> AddFileForBook(int id, IFormFile file)
    {
        try
        {
            var result = await _bookService.UpdateFileForBookId(id, file);
            return Ok(result);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpGet("types")]
    public async Task<IActionResult> GetBookTypes()
    {
        var result = Enum.GetValues(typeof(BookType)).Cast<BookType>().Select(x => new BookTypeDTO((int)x, x.ToString()));
        return Ok(result);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetBookById(int id)
    {
        try
        {
            var result = await _bookService.GetBookById(id);
            return Ok(result);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
    
    
}