namespace Chat.Domain.Exceptions;

public class UserNotFoundException : NotFoundException
{
    public UserNotFoundException(string emailOrUsernameOrId) : base($" User '{emailOrUsernameOrId}' does not exist") { }
}
