using BookService.Domain.Model;
using Microsoft.AspNetCore.Http;

namespace BookService.Application;

public class BookAddDTO
{
    public IFormFile File { get; set; }
    public string Title { get; set; }
    public IEnumerable<int> Author { get; set; }
    public string Publisher { get; set; }
    public string Description { get; set; }
    public DateTime PublishDate { get; set; }
    public int Type { get; set; }
    public int? Stock { get; set; }
}

public class BookResultDTO
{
    public int Id { get; set; }
    public string Title { get; set; }
    public IEnumerable<string> Author { get; set; }
    public string Publisher { get; set; }
    public int Type { get; set; }
    public int? Stock { get; set; }
    public string? FileAddress { get; set; }

    public BookResultDTO() {}
    public BookResultDTO(int id, string title, IEnumerable<Author> author, string publisher, int type, int? stock,
        string? fileAddress)
    {
        Id = id;
        Title = title;
        Author = author.Select(x => x.Name);
        Publisher = publisher;
        Type = type;
        Stock = stock;
        FileAddress = fileAddress;
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
}
