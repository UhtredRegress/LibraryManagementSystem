using BookService.Infrastructure.Interface;
using Microsoft.AspNetCore.Mvc;

namespace BookService.Presentation.Controllers;

[ApiController]
[Route("api/prices")]
public class BookPriceController : ControllerBase
{
    private readonly ;

    public BookPriceController(IBookPriceRepository bookPriceRepository)
    {
        _bookPriceRepository = bookPriceRepository;
    }

    [HttpPost]
    public async Task<IActionResult> AddBookPrice(BookPriceDTO bookPriceDto)
    {
        try
        {
            
        }
    }
}