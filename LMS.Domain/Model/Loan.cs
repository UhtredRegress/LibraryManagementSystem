namespace LMS.Domain.Model;

public class Loan
{
    public int Id { get; set; }

    public int UserId { get; set; }
    public User User { get; set; }

    public int BookId { get; set; }
    public Book Book { get; set; }
    
    public DateTime LoanDate { get; set; }
    public DateTime? ReturnDate { get; set; }
}