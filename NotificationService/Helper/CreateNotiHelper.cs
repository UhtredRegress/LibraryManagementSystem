using System.Text;

namespace NotificationService.Helper;

public static class CreateNotiHelper
{
    public static string CreateNotiBody(string title, string author, string category)
    {
        var notiBody = new StringBuilder();
        notiBody.AppendLine("There are new book added to Library"); 
        notiBody.AppendLine("Title: " + title);
        notiBody.AppendLine("Author: " + author);
        notiBody.AppendLine("Category: " + category);
        return notiBody.ToString();
    }

    public static string CreateNotiSubject(string category)
    {
        var notiSubject = new StringBuilder();
        notiSubject.Append($"There are new book of category {category} added to Library");
        return notiSubject.ToString();
    }
}