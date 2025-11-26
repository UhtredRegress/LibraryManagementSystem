using BookService.Application.DTO;
using BookService.Domain.Model;
using Microsoft.AspNetCore.Http;

namespace BookService.Application;

public class BookAddDTO
{
    public IFormFile Cover { get; set; }
    public IFormFile? File { get; set; }
    public string Title { get; set; }
    public IEnumerable<int> Author { get; set; }
    public string Publisher { get; set; }
    public string Description { get; set; }
    public DateTime PublishDate { get; set; }
    public int Type { get; set; }
    public IEnumerable<int> Category { get; set; }
    public int? Stock { get; set; }
}


public class BookResultDTO
{
    public int Id { get; set; }
    public string Title { get; set; }
    public IEnumerable<AuthorResponseDTO> Author { get; set; }
    public IEnumerable<CategoryDTO> Category { get; set; }
    public string Publisher { get; set; }
    public int Type { get; set; }
    public int? Stock { get; set; }
    public string? FileAddress { get; set; }

    public BookResultDTO() {}
    public BookResultDTO(int id, string title, IEnumerable<Author> author, IEnumerable<BookCategory> categories, string publisher, int type, int? stock,
        string? fileAddress)
    {
        Id = id;
        Title = title;
        Author = author.Select(x => new AuthorResponseDTO() {Id = x.Id, Name = x.Name });
        Category = categories.Select(x => new CategoryDTO() {Id = x.CategoryId, Name = x.Category.Name });
        Publisher = publisher;
        Type = type;
        Stock = stock;
        FileAddress = fileAddress;
    }
} 

public class BookInfoResultDTO
{
    public int Id { get; set; }
    public string Title { get; set; }
    public IEnumerable<AuthorResponseDTO> Author { get; set; }
    public IEnumerable<CategoryDTO> Category { get; set; }
    public string Publisher { get; set; }
    public int Type { get; set; }
    public int? Stock { get; set; }
    public string? CoverAddress { get; set; }

    public BookInfoResultDTO() {}       
    public BookInfoResultDTO(int id, string title, IEnumerable<Author> author, IEnumerable<BookCategory> categories, string publisher, int type, int? stock,
        string? coverAddress)
    {
        Id = id;
        Title = title;
        Author = author.Select(x => new AuthorResponseDTO() {Id = x.Id, Name = x.Name });
        Category = categories.Select(x => new CategoryDTO() {Id = x.CategoryId, Name = x.Category.Name });
        Publisher = publisher;
        Type = type;
        Stock = stock;
        CoverAddress = coverAddress;
    }
} 

public class BookTypeDTO
{
    public int Id { get; set; }
    public string Name { get; set; }

    public BookTypeDTO(int id, string name)
    {
        Id = id;
        Name = name;
    }
}

public class BookUpdateInformationDTO
{
    public string Title { get; set; }
    public IEnumerable<int> Author { get; set; }
    public string Publisher { get; set; }
    public string Description { get; set; }
    public DateTime PublishDate { get; set; }
    public IEnumerable<int> Category { get; set; }
}
