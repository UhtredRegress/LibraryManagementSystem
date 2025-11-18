namespace Shared.Exception;

public class UnauthenticationUserException : System.Exception
{
    public UnauthenticationUserException() : base() { }
    public UnauthenticationUserException(string message) : base(message) { }
    public UnauthenticationUserException(string message, System.Exception innerException) { }
}