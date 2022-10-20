namespace Chat.Domain.Exceptions;

public class UserIsAlreadyAModeratorException : BadRequestException
{
    public UserIsAlreadyAModeratorException(string usernameOrId) : base($"User '{usernameOrId}' is already a moderator") { }
}
