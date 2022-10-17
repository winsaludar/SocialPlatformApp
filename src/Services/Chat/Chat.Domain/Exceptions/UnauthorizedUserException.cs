namespace Chat.Domain.Exceptions;

public class UnauthorizedUserException : UnauthorizedAccessException
{
    public UnauthorizedUserException(string emailOrId) : base($"User '{emailOrId}' is unauthorized to perform the action") { }
}
