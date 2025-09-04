namespace LMS.Shared.Exception;

public class DuplicateDataException : System.Exception
{
    public DuplicateDataException() : base() { }
    public DuplicateDataException(string message) : base(message) { }
    public DuplicateDataException(string message, System.Exception inner) : base(message, inner) { }
}