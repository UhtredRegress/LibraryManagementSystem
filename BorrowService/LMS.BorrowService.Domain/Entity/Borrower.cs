namespace LMS.BorrowService.Domain.Entity;

public class Borrower
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
    public string Address { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    
    public Borrower() {}

    public Borrower(string name, string email, string phone, string address)
    {
        Name = name;
        Email = email;
        Phone = phone;
        Address = address;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public Borrower CreateNewBorrower(string name, string email, string phone, string address)
    {
        Borrower newBorrower = new Borrower();
        
        newBorrower.Name = name;
        newBorrower.Email = email;
        newBorrower.Phone = phone;
        newBorrower.Address = address;
        newBorrower.CreatedAt = DateTime.UtcNow;
        newBorrower.UpdatedAt = DateTime.UtcNow;
        
        return newBorrower;
    }
}