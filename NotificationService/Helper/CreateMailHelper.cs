using System.Text;
using Lirabry.Grpc;

namespace NotificationService.Helper;

public static class CreateMailHelper
{
    public static string CreateMailToNotifyBorrowStatus(string username, DateTime startDate, DateTime returnDate,
        GetBookInfoResponse response)
    {
        var emailBody = new StringBuilder();
        emailBody.AppendLine($"Dear {username}");
        emailBody.AppendLine("You have successfully borrow these books from the Library Management System");
        foreach (var book in response.Books)
        {
            emailBody.AppendLine($"Title: {book.Title}     Author: {book.Author}        Publisher: {book.Publisher} ");
        }

        emailBody.AppendLine($"You have borrowed on the day {startDate.ToString("dd-MM-yyyy")}");
        emailBody.AppendLine($"Please returned the book right on {returnDate.ToString("dd-MM-yyyy")}");
        emailBody.AppendLine("Thank you for your borrowing books.");
        return emailBody.ToString();
    }
}