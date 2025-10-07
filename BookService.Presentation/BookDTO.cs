using BookService.Domain.Model;

namespace BookService.Presentation;

public class BookAddDTO
{
    public IFormFile? File { get; set; }
    public Book Book { get; set; }
}