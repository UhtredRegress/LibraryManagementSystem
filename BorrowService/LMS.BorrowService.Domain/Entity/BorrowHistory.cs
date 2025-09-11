using LMS.BorrowService.Domain.ValueObject;

namespace LMS.BorrowService.Domain.Entity;

public class BorrowHistory
{
    public int Id { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public BorrowStatus Status { get; set; }
    
    public int BorrowerId { get; set; }
    public Borrower Borrower { get; set; }
    
    public int BookId { get; set; }

    public BorrowHistory() {}
    public BorrowHistory(int borrowerId, int bookId, int days)
    {
        StartDate = DateTime.UtcNow;
        EndDate = DateTime.UtcNow.AddDays(days);
        BorrowerId = borrowerId;
        BookId = bookId;
        Status = BorrowStatus.Approved;
    }
}