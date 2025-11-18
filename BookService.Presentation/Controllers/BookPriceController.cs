using BookService.Application;
using BookService.Application.IService;
using Microsoft.AspNetCore.Mvc;

namespace BookService.Presentation.Controllers;

[ApiController]
[Route("api/prices")]
public class BookPriceController : ControllerBase
{
    private readonly IBookPriceService _bookPriceService;

    public BookPriceController(IBookPriceService bookPriceService)
    {
        _bookPriceService = bookPriceService;
    }

    [HttpPost]
    public async Task<IActionResult> AddBookPrice(BookPriceDTO bookPriceDto)
    {
        try
        {
            var bookPrice = await _bookPriceService.AddBookPriceAsync(bookPriceDto);
            return Ok(bookPrice);
        }
        catch (Exception ex)
        {
            return BadRequest(new {Error = ex.Message});
        }
    }
}