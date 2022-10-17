namespace Chat.Domain.Exceptions;

public class UnauthorizedServerDeleterException : UnauthorizedAccessException
{
    public UnauthorizedServerDeleterException(string emailOrId) : base($"User '{emailOrId}' is unauthorized to delete the server") { }
}
