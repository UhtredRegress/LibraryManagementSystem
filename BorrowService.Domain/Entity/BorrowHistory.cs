using BorrowService.Domain.ValueObject;

namespace BorrowService.Domain.Entity;

public class BorrowHistory
{
    public int Id { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public BorrowStatus Status { get; set; }
    
    public int BorrowerId { get; set; }
    public Borrower Borrower { get; set; }
    
    public int BookId { get; set; }
    public DateTime? ReturnDate { get; private set; }
    public int? ReturnedConfirmBy { get; private set; }

    public BorrowHistory() {}
    public BorrowHistory(int borrowerId, int bookId, int days)
    {
        StartDate = DateTime.UtcNow;
        EndDate = DateTime.UtcNow.AddDays(days);
        BorrowerId = borrowerId;
        BookId = bookId;
        Status = BorrowStatus.Approved;
        ReturnDate = null;
        ReturnedConfirmBy = null;
    }

    public BorrowHistory UpdateStatus(BorrowStatus borrowStatus)
    {
        Status = borrowStatus;
        return this;
    }

    public void UpdateReturnInformation(int userId)
    {
        Status = BorrowStatus.Done;
        ReturnDate = DateTime.UtcNow;
        ReturnedConfirmBy = userId;
    }
}