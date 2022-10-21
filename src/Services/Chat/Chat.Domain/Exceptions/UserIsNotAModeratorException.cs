namespace Chat.Domain.Exceptions;

public class UserIsNotAModeratorException : BadRequestException
{
    public UserIsNotAModeratorException(string usernameOrId) : base($"User '{usernameOrId}' is not a moderator") { }
}
