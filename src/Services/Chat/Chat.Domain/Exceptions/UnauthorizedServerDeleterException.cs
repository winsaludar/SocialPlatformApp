namespace Chat.Domain.Exceptions;

public class UnauthorizedServerDeleterException : UnauthorizedAccessException
{
    public UnauthorizedServerDeleterException(string email) : base($"Email '{email}' is unauthorized to delete the server") { }
}
