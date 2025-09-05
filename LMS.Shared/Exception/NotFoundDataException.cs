namespace LMS.Shared.Exception;

public class NotFoundDataException : System.Exception
{
    public NotFoundDataException() : base() { }
    public NotFoundDataException(string message) : base(message) { }
    public NotFoundDataException(string message, System.Exception innerException) : base(message, innerException) { }
}