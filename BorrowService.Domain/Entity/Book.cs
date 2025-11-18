namespace BorrowService.Domain.Entity;

public class Book
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string Author { get; set; }
    public string Publisher { get; set; }
    public int Avalability { get; set; }
}